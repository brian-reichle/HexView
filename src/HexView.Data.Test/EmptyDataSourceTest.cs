// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace HexView.Data.Test;

[TestFixture]
class EmptyDataSourceTest
{
	[Test]
	public void ReadValue()
	{
		Assert.That(EmptyDataSource.Instance.ByteCount, Is.Zero);
	}
}
