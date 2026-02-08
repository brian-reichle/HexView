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
			=> _name ??= Helper.ReadASCIIStringNode(
				_provider.Data,
				_tableReccordOffset + Constants.Section_Name_Offset,
				Constants.Section_Name_Length);

		public override ByteRange ByteRange
			=> _byteRange ??= new ByteRange(
				_provider.Data.Read<int>(_tableReccordOffset + Constants.Section_PointerToRawData_Offset),
				_provider.Data.Read<int>(_tableReccordOffset + Constants.Section_SizeOfRawData_Offset));

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

			return [.. list];
		}

		IStructuralNode NewSectionNode(int dictionaryIndex, int fileOffset, int fileLength)
		{
			var name = Constants.GetDirectoryName(dictionaryIndex);

			return dictionaryIndex switch
			{
				Constants.Directory_Index_Resources => new ResourceDirectoryNode(_provider, this, fileOffset, fileOffset, 0),
				Constants.Directory_Index_CLI => new TemplatedStructuralNode(_provider.Data, this, name, PETemplates.CLIHeaderTemplate, fileOffset),
				_ => new TemplatedStructuralNode(_provider.Data, this, name, StandardTemplates.Blob(fileLength), fileOffset),
			};
		}

		string? _name;
		ByteRange? _byteRange;
		readonly PEStructuralNodeProvider _provider;
		readonly long _tableReccordOffset;
	}
}
