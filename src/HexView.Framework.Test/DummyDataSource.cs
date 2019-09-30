// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace HexView.Framework.Test
{
	sealed class DummyDataSource : IDataSource
	{
		public DummyDataSource(int length)
		{
			ByteCount = length;
		}

		public long ByteCount { get; }

		public void CopyTo(long offset, Span<byte> buffer)
		{
			GetBytes(offset, buffer.Length).AsSpan().CopyTo(buffer);
		}

		public void Set<T>(long offset, T value)
			where T : unmanaged
		{
			var data = new byte[Unsafe.SizeOf<T>()];
			Ref<T>(data) = value;
			Set(offset, data);
		}

		public void Set(long offset, byte[] data)
		{
			_expectations[(int)offset] = data;
		}

		public string ReadText(long offset, int length, Encoding encoding)
			=> encoding.GetString(GetBytes(offset, length));

		byte[] GetBytes(long offset, int length)
		{
			if (!_expectations.TryGetValue((int)offset, out var data))
			{
				throw new ArgumentException("Unexpected offset.");
			}
			else if (data.Length != length)
			{
				throw new ArgumentException("Length mismatch.");
			}

			return data;
		}

		static ref T Ref<T>(byte[] data)
			where T : unmanaged
		{
			return ref Unsafe.As<byte, T>(ref data[0]);
		}

		readonly Dictionary<int, byte[]> _expectations = new Dictionary<int, byte[]>();
	}
}
