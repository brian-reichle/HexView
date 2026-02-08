// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace HexView.Framework.Test
{
	[TestFixture]
	class TemplatedStructuralNodeTest
	{
		[Test]
		public void Children()
		{
			var data = new DummyDataSource(100);

			var childTemplate = new DummySimpleTemplate(4);

			var template = new DummySimpleTemplate(
				12,
				[
					new Component("A", childTemplate, 0),
					new Component("B", childTemplate, 4),
					new Component("C", childTemplate, 8),
				]);

			var node = new TemplatedStructuralNode(data, null, "root", template, 100);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(node.Children.Select(x => x.Name), Is.EqualTo(["A", "B", "C"]));
				Assert.That(node.Children.Select(x => x.ByteRange!.Offset), Is.EqualTo([100, 104, 108]));
				Assert.That(node.Children.Select(x => x.ByteRange!.Length), Has.All.EqualTo(4));
				Assert.That(node.Children.Select(x => x.Parent), Has.All.EqualTo(node));
			}
		}

		[Test]
		public void ChildrenEmpty()
		{
			var data = new DummyDataSource(100);

			var node = new TemplatedStructuralNode(
				data,
				null,
				"<name>",
				new DummySimpleTemplate(10),
				100);

			using (Assert.EnterMultipleScope())
			{
				Assert.That(node.ByteRange.Offset, Is.EqualTo(100));
				Assert.That(node.ByteRange.Length, Is.EqualTo(10));
				Assert.That(node.Children, Is.Empty);
			}
		}

		[Test]
		public void Value()
		{
			var data = new DummyDataSource(200);

			var template = new DummySimpleTemplate(
				10,
				valueAccessor: (data, pos) =>
				{
					if (pos != 100)
					{
						throw new UnreachableException();
					}

					return 42;
				});

			var node = new TemplatedStructuralNode(
				data,
				null,
				"<name>",
				template,
				100);

			Assert.That(node.Value, Is.EqualTo(42));
		}
	}
}
