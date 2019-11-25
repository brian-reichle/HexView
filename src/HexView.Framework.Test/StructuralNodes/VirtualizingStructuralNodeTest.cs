// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.CompilerServices;
using Moq;
using NUnit.Framework;

namespace HexView.Framework.Test
{
	[TestFixture]
	class VirtualizingStructuralNodeTest
	{
		[Test]
		public void LazyCreateChildren()
		{
			var mockFactory = new Mock<Func<int, IStructuralNode>>(MockBehavior.Strict);
			var node = new Dummy(null, "<name>", new Range(100, 50), 2, mockFactory.Object);
			var children = node.Children;

			Assert.That(children.Count, Is.EqualTo(2));

			mockFactory
				.Setup(x => x(1))
				.Returns((int index) => CreateChild(1));

			Assert.That(children[1].Name, Is.EqualTo("<child1>"));

			mockFactory
				.Setup(x => x(0))
				.Returns((int index) => CreateChild(0));

			Assert.That(children[0].Name, Is.EqualTo("<child0>"));
		}

		[Test]
		public void CreateItemEachTime()
		{
			var mockFactory = new Mock<Func<int, IStructuralNode>>(MockBehavior.Strict);
			var node = new Dummy(null, "<name>", new Range(100, 50), 2, mockFactory.Object);
			var children = node.Children;

			Assert.That(children.Count, Is.EqualTo(2));

			mockFactory
				.Setup(x => x(0))
				.Returns((int index) => CreateChild(1));

			Assert.That(children[0], Is.Not.SameAs(children[0]));
		}

		[Test]
		public void DontHoldAReferenceToChildren()
		{
			var node = new Dummy(null, "<name>", new Range(100, 50), 2, (int index) => CreateChild(0));
			var weakChild = GetWeakItem(node, 0);

			GC.Collect();
			Assert.That(weakChild.TryGetTarget(out _), Is.False);
		}

		static Dummy CreateChild(int index) => new Dummy(
			null,
			"<child" + index + ">",
			new Range(100 + index * 25, 25),
			0,
			x => throw new InvalidOperationException());

		[MethodImpl(MethodImplOptions.NoInlining)]
		static WeakReference<IStructuralNode> GetWeakItem(IStructuralNode parent, int index) => new WeakReference<IStructuralNode>(parent.Children[index]);

		sealed class Dummy : VirtualizingStructuralNode
		{
			public Dummy(IStructuralNode? parent, string name, Range byteRange, int count, Func<int, IStructuralNode> factory)
				: base(parent)
			{
				Name = name;
				ByteRange = byteRange;
				Count = count;
				_factory = factory;
			}

			public override string Name { get; }
			public override Range ByteRange { get; }
			protected override int Count { get; }

			protected override IStructuralNode CreateChildNode(int index) => _factory(index);

			readonly Func<int, IStructuralNode> _factory;
		}
	}
}
