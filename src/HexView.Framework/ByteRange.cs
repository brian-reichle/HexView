// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Diagnostics;

namespace HexView.Framework;

[DebuggerDisplay("({Offset}, {Length}")]
public sealed class ByteRange
{
	public ByteRange(long offset, long length)
	{
		Offset = offset;
		Length = length;
	}

	public long Offset { get; }
	public long Length { get; }
}
