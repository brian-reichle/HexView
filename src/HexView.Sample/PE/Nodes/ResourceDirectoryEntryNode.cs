// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Text;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class ResourceDirectoryEntryNode : LazyStructuralNode
	{
		public ResourceDirectoryEntryNode(PEStructuralNodeProvider provider, IStructuralNode parent, long resourceBase, long offset, bool named, int depth)
			: base(parent)
		{
			_provider = provider;
			_resourceBase = resourceBase;
			_offset = offset;
			_named = named;
			_depth = depth;
		}

		public override string Name
		{
			get
			{
				if (_name == null)
				{
					var id = _provider.Data.Read<int>(_offset);

					if (_named)
					{
						const int OffsetMask = 0x7FFFFFFF;
						_name = GetResourceString(_resourceBase + (id & OffsetMask));
					}
					else if (_depth == 0 && Enum.IsDefined(typeof(ResourceTypes), id))
					{
						_name = "#" + id + " (" + (ResourceTypes)id + ")";
					}
					else
					{
						_name = "#" + id;
					}
				}

				return _name;
			}
		}

		public override ByteRange ByteRange => new ByteRange(_offset, 8);

		protected override IList<IStructuralNode> CreateChildNodes()
		{
			const int IsDirectoryFlag = unchecked((int)0x80000000);

			var id = _provider.Data.Read<int>(_offset + 4);
			var rvaTarget = _resourceBase + (id & ~IsDirectoryFlag);

			var keyNode = new TemplatedStructuralNode(_provider.Data, this, _named ? "Name RVA" : "ID", StandardTemplates.Int32, _offset);

			if ((id & IsDirectoryFlag) != 0)
			{
				return new IStructuralNode[]
				{
					keyNode,
					new TemplatedStructuralNode(_provider.Data, this, "Subdirectory RVA", StandardTemplates.Int32, _offset + 4),
					new ResourceDirectoryNode(_provider, this, _resourceBase, rvaTarget, _depth + 1),
				};
			}
			else
			{
				var dataRVA = _provider.Data.Read<int>(rvaTarget);
				var dataSize = _provider.Data.Read<int>(rvaTarget + 4);

				var dataFilePos = _provider.MapRVAtoFile(dataRVA);

				return new IStructuralNode[]
				{
					keyNode,
					new TemplatedStructuralNode(_provider.Data, this, "Data Entry RVA", StandardTemplates.Int32, _offset + 4),
					new TemplatedStructuralNode(_provider.Data, this, "Data Entry", PETemplates.ResourceDataEntry, rvaTarget),
					new TemplatedStructuralNode(_provider.Data, this, "Data", StandardTemplates.Blob(dataSize), dataFilePos),
				};
			}
		}

		string GetResourceString(long offset)
		{
			var length = _provider.Data.Read<ushort>(offset);
			return _provider.Data.ReadText(offset + 2, length * 2, Encoding.Unicode);
		}

		readonly PEStructuralNodeProvider _provider;
		readonly bool _named;
		readonly long _resourceBase;
		readonly long _offset;
		readonly int _depth;
		string? _name;
	}
}
