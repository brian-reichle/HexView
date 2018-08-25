// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;
using System.Text;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class MetaDataRootNode : LazyStructuralNode
	{
		public MetaDataRootNode(PEStructuralNodeProvider provider, IStructuralNode parent, int length)
			: base(parent)
		{
			_provider = provider;
			ByteRange = new Range(_provider.MetaDataOffset, length);
		}

		public override string Name => "MetaData";
		public override Range ByteRange { get; }

		protected override IList<IStructuralNode> CreateChildNodes()
		{
			var data = _provider.Data;
			var offset = _provider.MetaDataOffset;
			var length = data.Read<int>(offset + 12);

			var headerTemplate = new CompoundNodeTemplate()
			{
				{ "Signature", StandardTemplates.UInt32 },
				{ "Major Version", StandardTemplates.UInt16 },
				{ "Minor Version", StandardTemplates.UInt16 },
				{ "Reserved", StandardTemplates.UInt32 },
				{ "Length", StandardTemplates.UInt32 },
				{ "Version", StandardTemplates.Text(length, Encoding.UTF8) },
				{ "Flags", StandardTemplates.UInt16 },
				{ "Streams", StandardTemplates.UInt16 },
			};

			return new IStructuralNode[]
			{
				new TemplatedStructuralNode(_provider.Data, this, "Header", headerTemplate, _provider.MetaDataOffset),
				new StreamHeaderListNode(_provider, this),
				new StreamListNode(_provider, this),
			};
		}

		readonly PEStructuralNodeProvider _provider;
	}
}
