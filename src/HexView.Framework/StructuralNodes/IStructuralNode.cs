// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;

namespace HexView.Framework
{
	public interface IStructuralNode
	{
		IStructuralNode? Parent { get; }
		IReadOnlyList<IStructuralNode> Children { get; }

		string Name { get; }
		Range? ByteRange { get; }
		object? Value { get; }
	}
}
