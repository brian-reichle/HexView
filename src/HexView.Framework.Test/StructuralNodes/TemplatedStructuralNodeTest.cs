// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace HexView.Framework.Test
{
	[TestFixture]
	class TemplatedStructuralNodeTest
	{
		[Test]
		public void Children()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var mockChildTemplate = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChildTemplate.Setup(x => x.Width).Returns(4);

			var mockTemplate = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockTemplate.Setup(x => x.Width).Returns(12);
			mockTemplate.Setup(x => x.Components).Returns(new[]
			{
				new Component("A", mockChildTemplate.Object, 0),
				new Component("B", mockChildTemplate.Object, 4),
				new Component("C", mockChildTemplate.Object, 8),
			});

			var node = new TemplatedStructuralNode(mockData.Object, null, "root", mockTemplate.Object, 100);

			Assert.That(node.Children.Select(x => x.Name), Is.EqualTo(new[] { "A", "B", "C" }));
			Assert.That(node.Children.Select(x => x.ByteRange.Offset), Is.EqualTo(new[] { 100, 104, 108 }));
			Assert.That(node.Children.Select(x => x.ByteRange.Length), Has.All.EqualTo(4));
			Assert.That(node.Children.Select(x => x.Parent), Has.All.EqualTo(node));
		}

		[Test]
		public void ChildrenEmpty()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var mockTemplate = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockTemplate.Setup(x => x.Width).Returns(10);
			mockTemplate.Setup(x => x.Components).Returns(Array.Empty<Component>());

			var node = new TemplatedStructuralNode(
				mockData.Object,
				null,
				"<name>",
				mockTemplate.Object,
				100);

			Assert.That(node.ByteRange.Offset, Is.EqualTo(100));
			Assert.That(node.ByteRange.Length, Is.EqualTo(10));
			Assert.That(node.Children.Count, Is.EqualTo(0));
		}

		[Test]
		public void Value()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var mockTemplate = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockTemplate.Setup(x => x.Width).Returns(10);
			mockTemplate.Setup(x => x.GetValue(mockData.Object, 100)).Returns(42);

			var node = new TemplatedStructuralNode(
				mockData.Object,
				null,
				"<name>",
				mockTemplate.Object,
				100);

			Assert.That(node.Value, Is.EqualTo(42));
		}
	}
}
