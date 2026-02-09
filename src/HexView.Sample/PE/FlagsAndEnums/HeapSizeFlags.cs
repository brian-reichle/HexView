// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView.Plugins.Sample.PE;

[Flags]
enum HeapSizeFlags : byte
{
	None = 0x00,
	STRING_HEAP_USES_LARGE_INDEXES = 0x01,
	GUID_HEAP_USES_LARGE_INDEXES = 0x02,
	BLOB_HEAP_USES_LARGE_INDEXES = 0x03,
}
