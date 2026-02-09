// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Collections.Generic;
using HexView.Framework;
using HexView.Plugins.Sample;
using HexView.Plugins.Sample.PE;

[assembly: Plugin(typeof(SamplePlugin))]

namespace HexView.Plugins.Sample;

public sealed class SamplePlugin : IPlugin
{
	public IEnumerable<IFormatReader> Readers => new[] { new PEFormatReader() };
}
