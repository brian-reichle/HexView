// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate
{
	[TestFixture]
	class BlobNodeTemplateTest
	{
		[Test]
		public void Width()
		{
			var template = new BlobNodeTemplate(42);
			Assert.That(template.Width, Is.EqualTo(42L));
		}

		[Test]
		public void Components()
		{
			var template = new BlobNodeTemplate(42);
			Assert.That(template.Components, Is.Empty);
		}

		[Test]
		public void GetValue()
		{
			var data = new DummyDataSource(200);
			data.Set(100, new byte[42]);

			var template = new BlobNodeTemplate(42);
			Assert.That(template.GetValue(data, 100), Is.Null);
		}
	}
}
