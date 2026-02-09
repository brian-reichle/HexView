// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace HexView.Data;

static class SafeBufferExtensions
{
	public static unsafe Span<byte> AsSpan(this SafeBuffer buffer)
	{
		return new Span<byte>(
			(void*)buffer.DangerousGetHandle(),
			(int)buffer.ByteLength);
	}
}
