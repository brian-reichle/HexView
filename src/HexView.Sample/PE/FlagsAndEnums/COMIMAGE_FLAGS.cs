// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView.Plugins.Sample.PE;

[Flags]
enum COMIMAGE_FLAGS : uint
{
	COMIMAGE_FLAGS_ILONLY = 0x00000001,
	COMIMAGE_FLAGS_32BITREQUIRED = 0x00000002,
	COMIMAGE_FLAGS_STRONGNAMESIGNED = 0x00000008,
	COMIMAGE_FLAGS_TRACKDEBUGDATA = 0x00010000,
}
