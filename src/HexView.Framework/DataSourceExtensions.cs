// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HexView.Framework;

public static class DataSourceExtensions
{
	public static T Read<T>(this IDataSource source, long offset)
		where T : unmanaged
	{
		ArgumentNullException.ThrowIfNull(source);

		var value = default(T);
		source.CopyTo(offset, MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1)));
		return value;
	}
}
