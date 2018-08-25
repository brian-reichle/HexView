// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Text;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class StreamHeaderListNode : VirtualizingStructuralNode
	{
		public StreamHeaderListNode(PEStructuralNodeProvider provider, IStructuralNode parent)
			: base(parent)
		{
			_provider = provider;
		}

		public override string Name => "Stream Headers";
		public override Range ByteRange => null;
		protected override int Count => _provider.StreamHeaderOffsets.Count;

		protected override IStructuralNode CreateChildNode(int index)
		{
			var pair = _provider.StreamHeaderOffsets[index];
			var nameLen = pair.Key.Length;
			var stringBytes = nameLen < 32 ? nameLen + 1 : 32;

			var template = new CompoundNodeTemplate()
			{
				{ "Offset", StandardTemplates.UInt32 },
				{ "Size", StandardTemplates.UInt32 },
				{ "Name", StandardTemplates.Text(stringBytes, Encoding.ASCII) },
			};

			template.RoundWidthUpToBoundary(4);
			return new TemplatedStructuralNode(_provider.Data, this, pair.Key, template, _provider.MetaDataOffset + pair.Value);
		}

		readonly PEStructuralNodeProvider _provider;
	}
}
