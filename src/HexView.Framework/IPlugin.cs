// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;

namespace HexView.Framework;

public interface IPlugin
{
	IEnumerable<IFormatReader> Readers { get; }
}
