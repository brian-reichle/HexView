// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HexView.Framework
{
	sealed class SimpleNodeTemplate<T> : IStructuralNodeTemplate
		where T : unmanaged
	{
		public SimpleNodeTemplate()
		{
			var type = typeof(T);

			if (type.IsEnum)
			{
				type = Enum.GetUnderlyingType(type);
			}

			Width = Marshal.SizeOf(type);
		}

		public long Width { get; }
		public IReadOnlyList<Component> Components => Array.Empty<Component>();
		public object GetValue(IDataSource data, long offset) => data.Read<T>(offset);
	}
}
