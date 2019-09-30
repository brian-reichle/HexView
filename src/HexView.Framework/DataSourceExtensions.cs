// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.CompilerServices;

namespace HexView.Framework
{
	public static class DataSourceExtensions
	{
		public static T Read<T>(this IDataSource source, long offset)
			where T : unmanaged
		{
			if (source == null) throw new ArgumentNullException(nameof(source));

			var value = default(T);
			source.CopyTo(offset, AsSpan(ref value));
			return value;
		}

		static unsafe Span<byte> AsSpan<T>(ref T field)
			where T : unmanaged
		{
			fixed (T* ptr = &field)
			{
				return new Span<byte>(ptr, Unsafe.SizeOf<T>());
			}
		}
	}
}
