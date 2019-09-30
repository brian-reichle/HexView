// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace HexView.Data
{
	[DebuggerDisplay("Count = {ByteCount}")]
	sealed class PagedMemoryMappedDataSource : DataSource
	{
		const long PageSize = 0x10000;
		const long AccessorOffsetMask = 0xFFFF;
		const long AccessorPageMask = ~AccessorOffsetMask;

		public PagedMemoryMappedDataSource(MemoryMappedFile file, long length)
		{
			_length = length;
			_file = file;
			_mru = new LinkedList<PageData>();
		}

		public override long ByteCount => _length;

		public override void CopyTo(long offset, Span<byte> buffer)
		{
			while (buffer.Length > 0)
			{
				var accessor = GetAccessor(offset);
				var span = accessor.SafeMemoryMappedViewHandle.AsSpan().Slice((int)(offset & AccessorOffsetMask));

				if (span.Length >= buffer.Length)
				{
					span = span.Slice(0, buffer.Length);
				}

				span.CopyTo(buffer);
				buffer = buffer.Slice(span.Length);
				offset += span.Length;
			}
		}

		public override string ReadText(long offset, int length, Encoding encoding)
		{
			var buffer = ArrayPool<byte>.Shared.Rent(length);
			ReadBytes(offset, length, buffer);
			var result = encoding.GetString(buffer, 0, length);
			ArrayPool<byte>.Shared.Return(buffer);
			return result;
		}

		void ReadBytes(long offset, int length, byte[] buffer)
		{
			var bufferOffset = 0;

			while (true)
			{
				var accessor = GetAccessor(offset);
				var blockSize = Math.Min(RemainingPageBytes(offset), length);

				accessor.ReadArray(offset & AccessorOffsetMask, buffer, bufferOffset, blockSize);
				length -= blockSize;

				if (length == 0) break;

				bufferOffset += blockSize;
				offset += blockSize;
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				for (var node = _mru.First; node != null; node = node.Next)
				{
					node.Value.Accessor.Dispose();
				}

				_mru.Clear();
				_file.Dispose();
			}

			base.Dispose(isDisposing);
		}

		MemoryMappedViewAccessor GetAccessor(long address)
		{
			var pageBaseAddress = address & AccessorPageMask;

			for (var node = _mru.First; node != null; node = node.Next)
			{
				var value = node.Value;

				if (value.BaseAddress == pageBaseAddress)
				{
					if (node.Previous != null)
					{
						_mru.Remove(node);
						_mru.AddFirst(node);
					}

					return value.Accessor;
				}
			}

			var newAccessor = _file.CreateViewAccessor(pageBaseAddress, Math.Min(pageBaseAddress + PageSize, _length) - pageBaseAddress, MemoryMappedFileAccess.Read);

			if (_mru.Count < 4)
			{
				_mru.AddFirst(new PageData(pageBaseAddress, newAccessor));
			}
			else
			{
				var node = _mru.Last;
				_mru.Remove(node);
				node.Value.Accessor.Dispose();
				node.Value = new PageData(pageBaseAddress, newAccessor);
				_mru.AddFirst(node);
			}

			return newAccessor;
		}

		static int RemainingPageBytes(long offset) => (int)(PageSize - (offset & AccessorOffsetMask));

		readonly long _length;
		readonly MemoryMappedFile _file;
		readonly LinkedList<PageData> _mru;

		struct PageData
		{
			public PageData(long baseAddress, MemoryMappedViewAccessor accessor)
			{
				BaseAddress = baseAddress;
				Accessor = accessor;
			}

			public readonly long BaseAddress;
			public readonly MemoryMappedViewAccessor Accessor;
		}
	}
}
