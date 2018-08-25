// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class PESection : LazyStructuralNode
	{
		public PESection(IStructuralNode parent, PEStructuralNodeProvider provider, int offset)
			: base(parent)
		{
			_provider = provider;
			_tableReccordOffset = offset;
		}

		public override string Name
		{
			get
			{
				if (_name == null)
				{
					_name = Helper.ReadASCIIStringNode(
						_provider.Data,
						this._tableReccordOffset + Constants.Section_Name_Offset,
						Constants.Section_Name_Length);
				}

				return _name;
			}
		}

		public override Range ByteRange
		{
			get
			{
				if (_byteRange == null)
				{
					_byteRange = new Range(
						_provider.Data.Read<int>(_tableReccordOffset + Constants.Section_PointerToRawData_Offset),
						_provider.Data.Read<int>(_tableReccordOffset + Constants.Section_SizeOfRawData_Offset));
				}

				return _byteRange;
			}
		}

		protected override IList<IStructuralNode> CreateChildNodes()
		{
			var sectionOffset = _provider.Data.Read<int>(_tableReccordOffset + Constants.Section_VirtualAddress_Offset);
			var sectionLength = _provider.Data.Read<int>(_tableReccordOffset + Constants.Section_VirtualSize_Offset);
			var sectionFileOffset = _provider.Data.Read<int>(_tableReccordOffset + Constants.Section_PointerToRawData_Offset);
			var sectionFileSize = _provider.Data.Read<int>(_tableReccordOffset + Constants.Section_SizeOfRawData_Offset);

			var list = new List<IStructuralNode>(_provider.DirectoryCount);
			var directoryOffset = _provider.DirectoryOffset;

			for (var i = 0; i < _provider.DirectoryCount; i++)
			{
				var offset = _provider.Data.Read<int>(directoryOffset + Constants.Directory_RVAOffset);
				var length = _provider.Data.Read<int>(directoryOffset + Constants.Directory_LengthOffset);

				if (length != 0 && offset >= sectionOffset && offset < sectionOffset + sectionLength)
				{
					var fileOffset = offset - sectionOffset + sectionFileOffset;
					var memSectionFollowByte = offset + length - sectionOffset;
					var fileLength = Math.Min(memSectionFollowByte, sectionFileSize) - fileOffset + sectionFileOffset;
					list.Add(NewSectionNode(i, fileOffset, fileLength));
				}

				directoryOffset += Constants.Directory_Length;
			}

			return list.ToArray();
		}

		IStructuralNode NewSectionNode(int dictionaryIndex, int fileOffset, int fileLength)
		{
			var name = Constants.GetDirectoryName(dictionaryIndex);

			switch (dictionaryIndex)
			{
				case Constants.Directory_Index_Resources:
					return new ResourceDirectoryNode(_provider, this, fileOffset, fileOffset, 0);

				case Constants.Directory_Index_CLI:
					return new TemplatedStructuralNode(_provider.Data, this, name, PETemplates.CLIHeaderTemplate, fileOffset);

				default:
					return new TemplatedStructuralNode(_provider.Data, this, name, StandardTemplates.Blob(fileLength), fileOffset);
			}
		}

		string _name;
		Range _byteRange;
		readonly PEStructuralNodeProvider _provider;
		readonly long _tableReccordOffset;
	}
}
