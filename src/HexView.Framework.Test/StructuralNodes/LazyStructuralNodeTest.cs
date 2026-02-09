// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace HexView.Framework.Test;

[TestFixture]
class LazyStructuralNodeTest
{
	[Test]
	public void EmptyChildren()
	{
		var node = new Dummy(
			null,
			"<name>",
			new ByteRange(100, 50),
			() => []);

		Assert.That(node.Children, Is.SameAs(Array.Empty<IStructuralNode>()));
	}

	[Test]
	public void NullChildren()
	{
		var node = new Dummy(
			null,
			"<name>",
			new ByteRange(100, 50),
			() => null!);

		Assert.That(node.Children, Is.SameAs(Array.Empty<IStructuralNode>()));
	}

	[Test]
	public void Children()
	{
		var child1 = new Dummy(null, "<child1>", new ByteRange(100, 25), CreateForbiddenFactory());
		var child2 = new Dummy(null, "<child2>", new ByteRange(125, 25), CreateForbiddenFactory());

		var node = new Dummy(
			null,
			"<name>",
			new ByteRange(100, 50),
			() => [child1, child2]);

		Assert.That(node.Children, Is.EquivalentTo([child1, child2]));
	}

	[Test]
	public void CacheChildren()
	{
		IStructuralNode[]? children = null;
		var count = 0;

		var node = new Dummy(
			null,
			"<name>",
			new ByteRange(100, 50),
			() =>
			{
				count++;
				return children ?? throw new UnreachableException();
			});

		children =
		[
			new Dummy(node, "<child1>", new ByteRange(100, 25), CreateForbiddenFactory()),
			new Dummy(node, "<child2>", new ByteRange(125, 25), CreateForbiddenFactory())
		];

		using (Assert.EnterMultipleScope())
		{
			var resultChildren = node.Children;
			Assert.That(node.Children, Is.SameAs(resultChildren), "Second call to node.Children should have returned the same collection.");
			Assert.That(count, Is.EqualTo(1), "Should have only hit the factory once.");
		}
	}

	static Func<IList<IStructuralNode>> CreateForbiddenFactory()
		=> () => throw new UnreachableException();

	sealed class Dummy : LazyStructuralNode
	{
		public Dummy(IStructuralNode? parent, string name, ByteRange byteRange, Func<IList<IStructuralNode>> childFactory)
			: base(parent)
		{
			Name = name;
			ByteRange = byteRange;
			_childFactory = childFactory;
		}

		public override string Name { get; }
		public override ByteRange ByteRange { get; }

		protected override IList<IStructuralNode> CreateChildNodes() => _childFactory();

		readonly Func<IList<IStructuralNode>> _childFactory;
	}
}
