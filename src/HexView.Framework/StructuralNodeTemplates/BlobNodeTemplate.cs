// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;

namespace HexView.Framework
{
	sealed class BlobNodeTemplate : IStructuralNodeTemplate
	{
		public BlobNodeTemplate(long width)
		{
			Width = width;
		}

		public long Width { get; }
		public object? GetValue(IDataSource data, long offset) => null;
		public IReadOnlyList<Component> Components => [];
	}
}
