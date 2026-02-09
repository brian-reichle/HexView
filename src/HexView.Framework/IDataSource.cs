// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;

namespace HexView.Framework;

public interface IDataSource
{
	long ByteCount { get; }

	void CopyTo(long offset, Span<byte> buffer);
	string ReadText(long offset, int length, Encoding encoding);
}
