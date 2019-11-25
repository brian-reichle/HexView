// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HexView.Framework
{
	public abstract class LazyStructuralNode : IStructuralNode
	{
		protected LazyStructuralNode(IStructuralNode? parent)
		{
			Parent = parent;
		}

		public abstract string Name { get; }
		public abstract Range? ByteRange { get; }
		public IStructuralNode? Parent { get; }
		public virtual object? Value => null;

		public IReadOnlyList<IStructuralNode> Children
		{
			get
			{
				if (_children == null)
				{
					var tmp = CreateChildNodes();

					if (tmp == null || tmp.Count == 0)
					{
						_children = Array.Empty<IStructuralNode>();
					}
					else
					{
						_children = new ReadOnlyCollection<IStructuralNode>(tmp);
					}
				}

				return _children;
			}
		}

		protected abstract IList<IStructuralNode> CreateChildNodes();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IReadOnlyList<IStructuralNode>? _children;
	}
}
