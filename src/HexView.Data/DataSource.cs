// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using HexView.Framework;

namespace HexView.Data
{
	public abstract class DataSource : IDataSource, IDisposable
	{
		public static DataSource Empty => EmptyDataSource.Instance;

		public static DataSource Load(string filename)
		{
			const MemoryMappedFileAccess access = MemoryMappedFileAccess.Read;
			const long SizeCutoff = 1024L * 1024L; // 1 megabyte

			using var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

			var length = file.Length;

			if (length <= 0)
			{
				return EmptyDataSource.Instance;
			}
			else if (length > SizeCutoff)
			{
				return new PagedMemoryMappedDataSource(MemoryMappedFile.CreateFromFile(file, null, length, access, HandleInheritability.None, leaveOpen: true), length);
			}
			else
			{
				using var map = MemoryMappedFile.CreateFromFile(file, null, length, access, HandleInheritability.None, leaveOpen: true);
				return new MemoryMappedDataSource(map.CreateViewAccessor(0, length, access), length);
			}
		}

		~DataSource()
		{
			Dispose(false);
		}

		public abstract long ByteCount { get; }

		public abstract void CopyTo(long offset, Span<byte> buffer);

		public abstract string ReadText(long offset, int length, Encoding encoding);

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
		}
	}
}
