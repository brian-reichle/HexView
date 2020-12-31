// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class ResourceDirectoryNode : LazyStructuralNode
	{
		public ResourceDirectoryNode(PEStructuralNodeProvider provider, IStructuralNode parent, long resourceBase, long offset, int depth)
			: base(parent)
		{
			_provider = provider;
			_resourceBase = resourceBase;
			_offset = offset;
			_depth = depth;
		}

		public override string Name => "Resource Directory";

		public override ByteRange ByteRange
		{
			get
			{
				if (_range == null)
				{
					var nameEntryCount = _provider.Data.Read<ushort>(_offset + 12);
					var idEntryCount = _provider.Data.Read<ushort>(_offset + 14);
					var entryCount = nameEntryCount + idEntryCount;

					_range = new ByteRange(_offset, PETemplates.ResourceDirectoryHeader.Width + entryCount * 8);
				}

				return _range;
			}
		}

		protected override IList<IStructuralNode> CreateChildNodes()
		{
			var nameEntryCount = _provider.Data.Read<ushort>(_offset + 12);
			var idEntryCount = _provider.Data.Read<ushort>(_offset + 14);

			var result = new IStructuralNode[nameEntryCount + idEntryCount + 1];
			result[0] = new TemplatedStructuralNode(_provider.Data, this, "Header", PETemplates.ResourceDirectoryHeader, _offset);

			var offset = _offset + PETemplates.ResourceDirectoryHeader.Width;
			var index = 1;

			for (var i = 0; i < nameEntryCount; i++)
			{
				result[index++] = new ResourceDirectoryEntryNode(_provider, this, _resourceBase, offset, true, _depth);
				offset += 8;
			}

			for (var i = 0; i < idEntryCount; i++)
			{
				result[index++] = new ResourceDirectoryEntryNode(_provider, this, _resourceBase, offset, false, _depth);
				offset += 8;
			}

			return result;
		}

		readonly long _resourceBase;
		readonly long _offset;
		readonly int _depth;
		readonly PEStructuralNodeProvider _provider;
		ByteRange? _range;
	}
}
