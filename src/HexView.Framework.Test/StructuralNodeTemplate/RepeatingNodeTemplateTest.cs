// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate
{
	[TestFixture]
	class RepeatingNodeTemplateTest
	{
		[Test]
		public void Children()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var mockChild = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild.Setup(x => x.Width).Returns(4);

			var template = (IStructuralNodeTemplate)new RepeatingNodeTemplate(
				mockChild.Object,
				5,
				(index, offset) => "(" + index + ", " + offset + ")");

			using (Assert.EnterMultipleScope())
			{
				Assert.That(template.Width, Is.EqualTo(20));
				Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["(0, 0)", "(1, 4)", "(2, 8)", "(3, 12)", "(4, 16)"]));
				Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0, 4, 8, 12, 16]));
				Assert.That(template.Components.Select(x => x.Template), Has.All.EqualTo(mockChild.Object));
			}
		}

		[Test]
		public void ChildrenEmpty()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var mockChild = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild.Setup(x => x.Width).Returns(4);

			var template = (IStructuralNodeTemplate)new RepeatingNodeTemplate(
				mockChild.Object,
				0,
				(index, offset) => throw new InvalidOperationException());

			using (Assert.EnterMultipleScope())
			{
				Assert.That(template.Width, Is.Zero);
				Assert.That(template.Components, Is.Empty);
			}
		}

		[Test]
		public void Value()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var mockChild = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild.Setup(x => x.Width).Returns(4);

			var template = (IStructuralNodeTemplate)new RepeatingNodeTemplate(
				mockChild.Object,
				1,
				(index, offset) => throw new InvalidOperationException());

			Assert.That(template.GetValue(mockData.Object, 100), Is.Null);
		}
	}
}
