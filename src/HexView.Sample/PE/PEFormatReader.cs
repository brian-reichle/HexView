// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class PEFormatReader : IFormatReader
	{
		public string Name => "PE File";

		public IStructuralNodeProvider Read(IDataSource data) => new PEStructuralNodeProvider(data);
	}
}
