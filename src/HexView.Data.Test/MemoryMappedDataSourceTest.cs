// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Text;
using NUnit.Framework;

namespace HexView.Data.Test
{
	[TestFixture]
	class MemoryMappedDataSourceTest
	{
		[Test]
		public void ByteCount()
		{
			Assert.That(_dataSource.ByteCount, Is.EqualTo(Length));
		}

		[Test]
		public void ReadValue()
		{
			Assert.That(_dataSource.Read<byte>(16), Is.EqualTo(16));
			Assert.That(_dataSource.Read<ushort>(16), Is.EqualTo(0x1110));
			Assert.That(_dataSource.Read<int>(16), Is.EqualTo(0x13121110));
		}

		[Test]
		public void ReadText()
		{
			Assert.That(_dataSource.ReadText(65, 10, Encoding.ASCII), Is.EqualTo("ABCDEFGHIJ"));
		}

		[OneTimeSetUp]
		protected void FixtureSetup()
		{
			_dataSource = CreateDataSource();
		}

		[OneTimeTearDown]
		protected void FixtureTearDown()
		{
			_dataSource?.Dispose();
		}

		static DataSource CreateDataSource()
		{
			using (var file = MapFactory.CreatePatterenedMap(Length))
			{
				return new MemoryMappedDataSource(file.CreateViewAccessor(), Length);
			}
		}

		const int Length = 0x100;
		DataSource _dataSource;
	}
}
