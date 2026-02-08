// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Linq;
using Moq;
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate
{
	[TestFixture]
	class CompoundNodeTemplateTest
	{
		[Test]
		public void Children()
		{
			var mockChild1 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild1.Setup(x => x.Width).Returns(4);

			var mockChild2 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild2.Setup(x => x.Width).Returns(2);

			var mockChild3 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild3.Setup(x => x.Width).Returns(2);

			var template = new CompoundNodeTemplate()
			{
				{ "A", mockChild1.Object },
				{ "B", mockChild2.Object },
				{ "C", mockChild3.Object },
			};

			Assert.That(template.Width, Is.EqualTo(8));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A", "B", "C"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0, 4, 6]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([mockChild1.Object, mockChild2.Object, mockChild3.Object]));
		}

		[Test]
		public void PositionOverride()
		{
			var mockChild1 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild1.Setup(x => x.Width).Returns(4);

			var mockChild2 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild2.Setup(x => x.Width).Returns(2);

			var mockChild3 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild3.Setup(x => x.Width).Returns(2);

			var template = new CompoundNodeTemplate()
			{
				{ "A", mockChild1.Object },
				{ PositionMode.RelativeToParent, 16, "B", mockChild2.Object },
				{ PositionMode.RelativeToLast, 2, "C", mockChild3.Object },
			};

			Assert.That(template.Width, Is.EqualTo(22));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A", "B", "C"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0, 16, 20]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([mockChild1.Object, mockChild2.Object, mockChild3.Object]));
		}

		[Test]
		public void RoundToMultiple()
		{
			var mockChild1 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild1.Setup(x => x.Width).Returns(1);

			var mockChild2 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild2.Setup(x => x.Width).Returns(4);

			var template = new CompoundNodeTemplate()
			{
				{ "A", mockChild1.Object },
				{ "B", mockChild2.Object },
			};

			template.RoundWidthUpToBoundary(4);

			Assert.That(template.Width, Is.EqualTo(8));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A", "B"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0, 1]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([mockChild1.Object, mockChild2.Object]));
		}

		[Test]
		public void OverrideWidth()
		{
			var mockChild1 = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild1.Setup(x => x.Width).Returns(1);

			var template = new CompoundNodeTemplate()
			{
				{ "A", mockChild1.Object },
			};

			template.OverrideWidth(4);

			Assert.That(template.Width, Is.EqualTo(4));
			Assert.That(template.Components.Select(x => x.Name), Is.EqualTo(["A"]));
			Assert.That(template.Components.Select(x => x.Offset), Is.EqualTo([0]));
			Assert.That(template.Components.Select(x => x.Template), Is.EqualTo([mockChild1.Object]));
		}

		[Test]
		public void GetValue()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var mockChild = new Mock<IStructuralNodeTemplate>(MockBehavior.Strict);
			mockChild.Setup(x => x.Width).Returns(1);

			var template = new CompoundNodeTemplate()
			{
				{ "A", mockChild.Object },
			};

			template.OverrideWidth(4);

			Assert.That(template.GetValue(mockData.Object, 100), Is.Null);
		}
	}
}
