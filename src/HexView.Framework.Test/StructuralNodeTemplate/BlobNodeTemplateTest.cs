// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using Moq;
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
			Assert.That(template.Components, Is.Null);
		}

		[Test]
		public void GetValue()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var template = new BlobNodeTemplate(42);
			Assert.That(template.GetValue(mockData.Object, 100), Is.Null);
		}
	}
}
