// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace HexView.Framework.Test
{
	[TestFixture]
	class DataSourceExtensionsTest
	{
		[Test]
		public void ReadValue()
		{
			var dummyData = new DummyDataSource(200);
			dummyData.Set(100, 0x11223344);

			Assert.That(dummyData.Read<int>(100), Is.EqualTo(0x11223344));
		}
	}
}
