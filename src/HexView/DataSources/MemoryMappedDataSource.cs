// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Buffers;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace HexView
{
	[DebuggerDisplay("Count = {ByteCount}")]
	sealed class MemoryMappedDataSource : DataSource
	{
		public MemoryMappedDataSource(MemoryMappedViewAccessor accessor, long length)
		{
			ByteCount = length;
			_accessor = accessor;
		}

		public override long ByteCount { get; }

		public override T Read<T>(long offset)
		{
			_accessor.Read<T>(offset, out var result);
			return result;
		}

		public override string ReadText(long offset, int length, Encoding encoding)
		{
			var buffer = ArrayPool<byte>.Shared.Rent(length);
			_accessor.ReadArray(offset, buffer, 0, length);
			var result = encoding.GetString(buffer, 0, length);
			ArrayPool<byte>.Shared.Return(buffer);
			return result;
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				_accessor.Dispose();
			}

			base.Dispose(isDisposing);
		}

		readonly MemoryMappedViewAccessor _accessor;
	}
}
