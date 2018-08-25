// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class PEStructuralNodeProvider : IStructuralNodeProvider
	{
		public PEStructuralNodeProvider(IDataSource data)
		{
			Data = data;
			_level = Setup();
		}

		public IMAGE_FILE_MAGIC Magic { get; private set; }
		public int PEFileHeaderOffset { get; private set; }
		public int PEFileOptionalHeaderOffset { get; private set; }
		public int PEFileOptionalHeaderSize { get; private set; }
		public int DirectoryOffset { get; private set; }
		public int DirectoryCount { get; private set; }
		public int PESectionTableOffset { get; private set; }
		public int PESectionCount { get; private set; }
		public int CLIOffset { get; private set; }
		public int MetaDataOffset { get; private set; }

		public IReadOnlyList<KeyValuePair<string, int>> StreamHeaderOffsets
		{
			get
			{
				if (_streamHeaderOffsets == null)
				{
					_streamHeaderOffsets = CalculateMetaDataOffsets();
				}

				return _streamHeaderOffsets;
			}
		}

		public IDataSource Data { get; }

		public long MapRVAtoFile(long rva)
		{
			var first = 0;
			var last = unchecked(PESectionCount - 1);

			while (first <= last)
			{
				var mid = (first + last) >> 1;
				var rowOffset = PESectionTableOffset + mid * Constants.Section_Length;
				var currentRVAOffset = Data.Read<int>(rowOffset + Constants.Section_VirtualAddress_Offset);

				if (rva < currentRVAOffset)
				{
					last = mid - 1;
					continue;
				}

				var currentRVASize = Data.Read<int>(rowOffset + Constants.Section_VirtualSize_Offset);

				if (rva >= currentRVAOffset + currentRVASize)
				{
					first = mid + 1;
					continue;
				}

				var currentFileOffset = Data.Read<int>(rowOffset + Constants.Section_PointerToRawData_Offset);

				return rva - currentRVAOffset + currentFileOffset;
			}

			return 0;
		}

		public IReadOnlyList<IStructuralNode> RootNodes
		{
			get
			{
				var result = new List<IStructuralNode>();

				if (_level == Level.DosExecutable)
				{
					result.Add(new TemplatedStructuralNode(Data, null, "MS-DOS Header", PETemplates.MSDosHeader, 0));
				}
				else if (_level == Level.PEFile)
				{
					var sectionHeaderList = new RepeatingNodeTemplate(
						PETemplates.SectionHeaderTemplate,
						PESectionCount,
						(index, offset) => Helper.ReadASCIIStringNode(Data, PESectionTableOffset + offset, Constants.Section_Name_Length));

					var optionalHeader = new CompoundNodeTemplate();

					switch (Magic)
					{
						case IMAGE_FILE_MAGIC.IMAGE_FILE_MAGIC_PE:
							optionalHeader.Add("Standard", PETemplates.StandardOptionalHeaderTemplate);
							optionalHeader.Add("NT Specific", PETemplates.NTSpecificOptionalHeaderTemplate);
							break;

						case IMAGE_FILE_MAGIC.IMAGE_FILE_MAGIC_PE_PLUS:
							optionalHeader.Add("Standard", PETemplates.StandardOptionalHeaderTemplate_Plus);
							optionalHeader.Add("NT Specific", PETemplates.NTSpecificOptionalHeaderTemplate_Plus);
							break;
					}

					optionalHeader.Add("Directory", new RepeatingNodeTemplate(PETemplates.DictionaryTemplate, DirectoryCount, (i, o) => Constants.GetDirectoryName(i)));
					optionalHeader.OverrideWidth(PEFileOptionalHeaderSize);

					result.Add(new TemplatedStructuralNode(Data, null, "MS-DOS Header", PETemplates.MSDosHeader, 0));
					result.Add(new TemplatedStructuralNode(Data, null, "PE Header", PETemplates.PEHeaderTemplate, PEFileHeaderOffset));
					result.Add(new TemplatedStructuralNode(Data, null, "PE Optional Header", optionalHeader, PEFileOptionalHeaderOffset));
					result.Add(new TemplatedStructuralNode(Data, null, "Section Table", sectionHeaderList, PESectionTableOffset));
					result.Add(new PESectionList(this, null));

					if (MetaDataOffset != 0)
					{
						result.Add(new MetaDataRootNode(this, null, Data.Read<int>(CLIOffset + Constants.CLIHeader_MetaDataLength_Offset)));
					}
				}

				if (_notifications != null)
				{
					result.Add(new NotificationNode(Data, null, _notifications));
				}

				return result;
			}
		}

		Level Setup()
		{
			if (Data.ByteCount < 10)
			{
				AddNotification(0, StandardTemplates.Blob(0), "File too short to be valid");
				return Level.Invalid;
			}
			else if (Data.Read<short>(0) != Constants.MSDos_Signature)
			{
				AddNotification(0, StandardTemplates.UInt16, "Invalid signature");
				return Level.Invalid;
			}

			int tmp;

			if ((tmp = Data.Read<ushort>(Constants.MSDos_DosHeaderSize)) < 4)
			{
				return Level.DosExecutable;
			}
			else if (Data.ByteCount <= (tmp << 4))
			{
				AddNotification(Constants.MSDos_DosHeaderSize, StandardTemplates.UInt16, "Specified dos header does not fit in file.");
				return Level.Invalid;
			}

			tmp = Data.Read<int>(Constants.MSDos_NewHeaderOffset);

			if (tmp == 0)
			{
				return Level.DosExecutable;
			}
			else if (tmp >= Data.ByteCount - Constants.Header_Length || Data.Read<int>(tmp) != Constants.Header_Signature)
			{
				AddNotification(Constants.MSDos_NewHeaderOffset, StandardTemplates.UInt32, "Extended header not within file.");
				return Level.Invalid;
			}

			PEFileHeaderOffset = tmp;
			PEFileOptionalHeaderOffset = PEFileHeaderOffset + Constants.Header_Length;
			PEFileOptionalHeaderSize = Data.Read<short>(PEFileHeaderOffset + Constants.Header_OptionalHeaderSizeOffset);

			if (PEFileOptionalHeaderSize > 2)
			{
				Magic = Data.Read<IMAGE_FILE_MAGIC>(PEFileOptionalHeaderOffset);
			}

			switch (Magic)
			{
				case IMAGE_FILE_MAGIC.IMAGE_FILE_MAGIC_PE:
					DirectoryOffset = PEFileOptionalHeaderOffset + Constants.OptHeader_DirectoryOffset;
					DirectoryCount = Data.Read<int>(PEFileOptionalHeaderOffset + Constants.OptHeader_DirectoryCountOffset);
					break;

				case IMAGE_FILE_MAGIC.IMAGE_FILE_MAGIC_PE_PLUS:
					DirectoryOffset = PEFileOptionalHeaderOffset + Constants.OptHeaderPlus_DirectoryOffset;
					DirectoryCount = Data.Read<int>(PEFileOptionalHeaderOffset + Constants.OptHeaderPlus_DirectoryCountOffset);
					break;
			}

			PESectionTableOffset = PEFileOptionalHeaderOffset + PEFileOptionalHeaderSize;
			PESectionCount = Data.Read<short>(PEFileHeaderOffset + Constants.Header_SectionCountOffset);

			tmp = Data.Read<int>(DirectoryOffset + (Constants.Directory_Index_CLI << 3));

			if (tmp != 0)
			{
				CLIOffset = (int)MapRVAtoFile(tmp);
				MetaDataOffset = (int)MapRVAtoFile(Data.Read<int>(CLIOffset + Constants.CLIHeader_MetaDataRVA_Offset));
			}

			return Level.PEFile;
		}

		IReadOnlyList<KeyValuePair<string, int>> CalculateMetaDataOffsets()
		{
			if (MetaDataOffset == 0)
			{
				return Array.Empty<KeyValuePair<string, int>>();
			}

			var length = Data.Read<int>(MetaDataOffset + 12);
			var streamCount = Data.Read<short>(MetaDataOffset + length + 18);

			var result = new KeyValuePair<string, int>[streamCount];
			var offset = length + 20;

			for (var i = 0; i < result.Length; i++)
			{
				var name = Helper.ReadASCIIStringNode(Data, MetaDataOffset + offset + 8, 32);
				var stringBytes = name.Length < 32 ? name.Length + 1 : 32;

				result[i] = new KeyValuePair<string, int>(name, offset);
				offset = (offset + stringBytes + 11) & 0xFC;
			}

			return new ReadOnlyCollection<KeyValuePair<string, int>>(result);
		}

		enum Level
		{
			Invalid,
			DosExecutable,
			PEFile,
		}

		void AddNotification(long offset, IStructuralNodeTemplate template, string message)
		{
			if (_notifications == null)
			{
				_notifications = new List<Notification>();
			}

			_notifications.Add(offset, template, message);
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Level _level;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		List<Notification> _notifications;
		IReadOnlyList<KeyValuePair<string, int>> _streamHeaderOffsets;
	}
}
