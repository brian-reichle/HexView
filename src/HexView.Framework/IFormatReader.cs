// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
namespace HexView.Framework
{
	public interface IFormatReader
	{
		string Name { get; }
		IStructuralNodeProvider? Read(IDataSource data);
	}
}
