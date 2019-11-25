// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Text;
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate
{
	[TestFixture]
	class TextNodeTemplateTest
	{
		[Test]
		public void Width()
		{
			var template = new TextNodeTemplate(42, Encoding.UTF8);
			Assert.That(template.Width, Is.EqualTo(42));
		}

		[Test]
		public void Components()
		{
			var template = new TextNodeTemplate(42, Encoding.UTF8);
			Assert.That(template.Components, Is.Empty);
		}

		[Test]
		public void GetValue()
		{
			const string Text = "<enter-witty-remark-here>";
			var data = Encoding.UTF8.GetBytes(Text);
			var dummyData = new DummyDataSource(200);
			dummyData.Set(100, data);

			var template = new TextNodeTemplate(data.Length, Encoding.UTF8);
			Assert.That(template.GetValue(dummyData, 100), Is.EqualTo(Text));
		}
	}
}
