// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using HexView.Framework;

namespace HexView.Data
{
	public sealed class NullFormatReader : IFormatReader
	{
		public static NullFormatReader Instance { get; } = new NullFormatReader();

		NullFormatReader()
		{
		}

		public string Name => string.Empty;
		IStructuralNodeProvider IFormatReader.Read(IDataSource data) => null;
	}
}
