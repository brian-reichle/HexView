// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class StreamListNode : VirtualizingStructuralNode
	{
		public StreamListNode(PEStructuralNodeProvider provider, IStructuralNode parent)
			: base(parent)
		{
			_provider = provider;
		}

		public override string Name => "Streams";
		public override Range ByteRange => null;
		protected override int Count => _provider.StreamHeaderOffsets.Count;

		protected override IStructuralNode CreateChildNode(int index)
		{
			var pair = _provider.StreamHeaderOffsets[index];
			var headerOffset = _provider.MetaDataOffset + pair.Value;

			var offset = _provider.Data.Read<int>(headerOffset);
			var length = _provider.Data.Read<int>(headerOffset + 4);

			return NewStreamNode(pair.Key, offset, length);
		}

		IStructuralNode NewStreamNode(string name, int offset, int length)
		{
			var fileOffset = _provider.MetaDataOffset + offset;

			switch (name)
			{
				case "#GUID":
					var template = new RepeatingNodeTemplate(StandardTemplates.Guid, (int)(length >> 4), (i, l) => (i + 1).ToString("X08"));
					return new TemplatedStructuralNode(_provider.Data, this, name, template, fileOffset);

				case "#~":
					return new TableStreamNode(_provider, this, name, fileOffset, length);

				default:
					return new TemplatedStructuralNode(_provider.Data, this, name, StandardTemplates.Blob(length), fileOffset);
			}
		}

		readonly PEStructuralNodeProvider _provider;
	}
}
