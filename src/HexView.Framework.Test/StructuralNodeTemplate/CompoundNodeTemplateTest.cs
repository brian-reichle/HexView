// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Linq;
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate;

[TestFixture]
class CompoundNodeTemplateTest
{
	[Test]
	public void Children()
	{
		var child1 = new DummySimpleTemplate(4);
		var child2 = new DummySimpleTemplate(2);
		var child3 = new DummySimpleTemplate(2);

		var template = new CompoundNodeTemplate()
		{
			{ "A", child1 },
			{ "B", child2 },
			{ "C", child3 },
		};

		using (Assert.EnterMultipleScope())
		{
			Assert.That(template.Width, Is.EqualTo(8));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A", "B", "C"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0, 4, 6]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([child1, child2, child3]));
		}
	}

	[Test]
	public void PositionOverride()
	{
		var child1 = new DummySimpleTemplate(4);
		var child2 = new DummySimpleTemplate(2);
		var child3 = new DummySimpleTemplate(2);

		var template = new CompoundNodeTemplate()
		{
			{ "A", child1 },
			{ PositionMode.RelativeToParent, 16, "B", child2 },
			{ PositionMode.RelativeToLast, 2, "C", child3 },
		};

		using (Assert.EnterMultipleScope())
		{
			Assert.That(template.Width, Is.EqualTo(22));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A", "B", "C"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0, 16, 20]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([child1, child2, child3]));
		}
	}

	[Test]
	public void RoundToMultiple()
	{
		var child1 = new DummySimpleTemplate(1);
		var child2 = new DummySimpleTemplate(4);

		var template = new CompoundNodeTemplate()
		{
			{ "A", child1 },
			{ "B", child2 },
		};

		template.RoundWidthUpToBoundary(4);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(template.Width, Is.EqualTo(8));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A", "B"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0, 1]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([child1, child2]));
		}
	}

	[Test]
	public void OverrideWidth()
	{
		var child1 = new DummySimpleTemplate(1);

		var template = new CompoundNodeTemplate()
		{
			{ "A", child1 },
		};

		template.OverrideWidth(4);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(template.Width, Is.EqualTo(4));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([child1]));
		}
	}

	[Test]
	public void GetValue()
	{
		var data = new DummyDataSource(200);

		var template = new CompoundNodeTemplate()
		{
			{ "A", new DummySimpleTemplate(1) },
		};

		template.OverrideWidth(4);

		Assert.That(template.GetValue(data, 100), Is.Null);
	}
}
