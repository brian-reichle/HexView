// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate
{
	[TestFixture]
	class SimpleTemplateTest
	{
		[Test]
		public void Width()
		{
			using (Assert.EnterMultipleScope())
			{
				Assert.That(new SimpleNodeTemplate<int>().Width, Is.EqualTo(4));
				Assert.That(new SimpleNodeTemplate<long>().Width, Is.EqualTo(8));
				Assert.That(new SimpleNodeTemplate<Foo>().Width, Is.EqualTo(2));
			}
		}

		[Test]
		public void Components()
		{
			var template = new SimpleNodeTemplate<int>();
			Assert.That(template.Components, Is.Empty);
		}

		[Test]
		public void GetValue()
		{
			var dummyData = new DummyDataSource(200);
			dummyData.Set(100, 42);

			var template = new SimpleNodeTemplate<int>();
			Assert.That(template.GetValue(dummyData, 100), Is.EqualTo(42));
		}

		enum Foo : short
		{
			Unknown = 0,
		}
	}
}
