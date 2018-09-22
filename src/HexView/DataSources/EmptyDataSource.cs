// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;

namespace HexView
{
	sealed class EmptyDataSource : DataSource
	{
		public static DataSource Instance { get; } = new EmptyDataSource();

		EmptyDataSource()
		{
		}

		public override long ByteCount => 0;

		public override T Read<T>(long offset)
			=> throw new ArgumentOutOfRangeException(nameof(offset));

		public override string ReadText(long offset, int length, Encoding encoding)
			=> throw new ArgumentOutOfRangeException(nameof(offset));
	}
}
