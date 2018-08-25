// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using HexView.Framework;

namespace HexView
{
	sealed class NullFormatReader : IFormatReader
	{
		NullFormatReader()
		{
		}

		public static readonly NullFormatReader Instance = new NullFormatReader();
		public string Name => string.Empty;
		IStructuralNodeProvider IFormatReader.Read(IDataSource data) => null;
	}
}
