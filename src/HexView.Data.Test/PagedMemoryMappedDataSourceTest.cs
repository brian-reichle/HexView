// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.IO.MemoryMappedFiles;
using System.Text;
using NUnit.Framework;

namespace HexView.Data.Test
{
	[TestFixture]
	class PagedMemoryMappedDataSourceTest
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
		[Ignore("Doesn't work yet.")]
		public void ReadValueAcrossPages()
		{
			Assert.That(_dataSource.Read<long>(0x10000 - 4), Is.EqualTo(0x03020100FFFEFDFC));
		}

		[Test]
		public void ReadText()
		{
			Assert.That(_dataSource.ReadText(65, 10, Encoding.ASCII), Is.EqualTo("ABCDEFGHIJ"));
		}

		[Test]
		public void ReadTextAcrossPages()
		{
			Assert.That(_dataSource.ReadText(0x10000 - 4, 8, Encoding.GetEncoding("ISO8859-1")), Is.EqualTo("\xFC\xFD\xFE\xFF\x00\x01\x02\x03"));
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
			MemoryMappedFile file = null;

			try
			{
#pragma warning disable CA2000 // Dispose objects before losing scope
				file = MapFactory.CreatePatterenedMap(Length);
#pragma warning restore CA2000 // Dispose objects before losing scope
				return new PagedMemoryMappedDataSource(file, Length);
			}
			catch
			{
				file?.Dispose();
				throw;
			}
		}

		const int Length = 0x20000; // 2 pages.
		DataSource _dataSource;
	}
}
