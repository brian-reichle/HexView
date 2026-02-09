// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Text;
using NUnit.Framework;

namespace HexView.Data.Test;

[TestFixture]
class MemoryMappedDataSourceTest
{
	[Test]
	public void ByteCount()
	{
		Assert.That(_dataSource.ByteCount, Is.EqualTo(Length));
	}

	[Test]
	public void CopyTo()
	{
		var data = new byte[4];
		_dataSource.CopyTo(16, data);
		Assert.That(data, Is.EqualTo([0x10, 0x11, 0x12, 0x13]));
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
		using var file = MapFactory.CreatePatterenedMap(Length);
		return new MemoryMappedDataSource(file.CreateViewAccessor(), Length);
	}

	const int Length = 0x100;
#nullable disable
	DataSource _dataSource;
#nullable restore
}
