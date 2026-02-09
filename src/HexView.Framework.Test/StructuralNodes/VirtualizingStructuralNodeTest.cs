// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace HexView.Framework.Test;

[TestFixture]
class VirtualizingStructuralNodeTest
{
	[Test]
	public void LazyCreateChildren()
	{
		var requests = new List<int>();

		var node = new Dummy(
			null,
			"<name>",
			new ByteRange(100, 50),
			2,
			index =>
			{
				requests.Add(index);
				return CreateChild(index);
			});

		var children = node.Children;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(children, Has.Count.EqualTo(2));
			Assert.That(requests, Is.Empty);
		}

		using (Assert.EnterMultipleScope())
		{
			Assert.That(children[1].Name, Is.EqualTo("<child1>"));
			Assert.That(requests, Is.EqualTo([1]));
		}

		requests.Clear();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(children[0].Name, Is.EqualTo("<child0>"));
			Assert.That(requests, Is.EqualTo([0]));
		}
	}

	[Test]
	public void CreateItemEachTime()
	{
		var requests = new List<int>();

		var node = new Dummy(
			null,
			"<name>",
			new ByteRange(100, 50),
			2,
			index =>
			{
				requests.Add(index);
				return CreateChild(index);
			});

		var children = node.Children;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(children, Has.Count.EqualTo(2));
			Assert.That(requests, Is.Empty);
		}

		var firstChild = children[0];

		using (Assert.EnterMultipleScope())
		{
			Assert.That(children[0], Is.Not.SameAs(firstChild), "Should have created a new instance.");
			Assert.That(requests, Is.EqualTo([0, 0]));
		}
	}

	[Test]
	public void DontHoldAReferenceToChildren()
	{
		var node = new Dummy(null, "<name>", new ByteRange(100, 50), 2, index => CreateChild(0));
		var weakChild = GetWeakItem(node, 0);

		GC.Collect();
		Assert.That(weakChild.TryGetTarget(out _), Is.False);
	}

	static Dummy CreateChild(int index) => new(
		null,
		"<child" + index + ">",
		new ByteRange(100 + index * 25, 25),
		0,
		x => throw new InvalidOperationException());

	[MethodImpl(MethodImplOptions.NoInlining)]
	static WeakReference<IStructuralNode> GetWeakItem(IStructuralNode parent, int index) => new(parent.Children[index]);

	sealed class Dummy : VirtualizingStructuralNode
	{
		public Dummy(IStructuralNode? parent, string name, ByteRange byteRange, int count, Func<int, IStructuralNode> factory)
			: base(parent)
		{
			Name = name;
			ByteRange = byteRange;
			Count = count;
			_factory = factory;
		}

		public override string Name { get; }
		public override ByteRange ByteRange { get; }
		protected override int Count { get; }

		protected override IStructuralNode CreateChildNode(int index) => _factory(index);

		readonly Func<int, IStructuralNode> _factory;
	}
}
