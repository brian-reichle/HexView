// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;

namespace HexView.Framework.Test
{
	[TestFixture]
	class LazyStructuralNodeTest
	{
		[Test]
		public void EmptyChildren()
		{
			var mock = new Mock<IList<IStructuralNode>>(MockBehavior.Strict);

			var node = new Dummy(
				null,
				"<name>",
				new Range(100, 50),
				() => mock.Object);

			mock.Setup(x => x.Count).Returns(0);

			Assert.That(node.Children, Is.SameAs(Array.Empty<IStructuralNode>()));
		}

		[Test]
		public void NullChildren()
		{
			var node = new Dummy(
				null,
				"<name>",
				new Range(100, 50),
				() => null);

			Assert.That(node.Children, Is.SameAs(Array.Empty<IStructuralNode>()));
		}

		[Test]
		public void Children()
		{
			var child1 = new Dummy(null, "<child1>", new Range(100, 25), CreateForbiddenFactory());
			var child2 = new Dummy(null, "<child2>", new Range(125, 25), CreateForbiddenFactory());

			var node = new Dummy(
				null,
				"<name>",
				new Range(100, 50),
				() => new[] { child1, child2 });

			Assert.That(node.Children, Is.EquivalentTo(new[] { child1, child2 }));
		}

		[Test]
		public void CacheChildren()
		{
			var mockFactory = new Mock<Func<IList<IStructuralNode>>>(MockBehavior.Strict);

			var node = new Dummy(null, "<name>", new Range(100, 50), mockFactory.Object);
			var child1 = new Dummy(node, "<child1>", new Range(100, 25), CreateForbiddenFactory());
			var child2 = new Dummy(node, "<child2>", new Range(125, 25), CreateForbiddenFactory());
			var children = new[] { child1, child2 };

			mockFactory
				.Setup(x => x())
				.Returns(() => children);

			Assert.That(node.Children, Is.SameAs(node.Children));

			mockFactory.Verify(x => x(), Times.Once);
		}

		static Func<IList<IStructuralNode>> CreateForbiddenFactory() => new Mock<Func<IList<IStructuralNode>>>(MockBehavior.Strict).Object;

		sealed class Dummy : LazyStructuralNode
		{
			public Dummy(IStructuralNode parent, string name, Range byteRange, Func<IList<IStructuralNode>> childFactory)
				: base(parent)
			{
				Name = name;
				ByteRange = byteRange;
				_childFactory = childFactory;
			}

			public override string Name { get; }
			public override Range ByteRange { get; }

			protected override IList<IStructuralNode> CreateChildNodes() => _childFactory();

			readonly Func<IList<IStructuralNode>> _childFactory;
		}
	}
}
