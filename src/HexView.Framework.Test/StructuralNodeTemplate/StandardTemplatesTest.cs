// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;
using Moq;
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate
{
	[TestFixture]
	class StandardTemplatesTest
	{
		[Test]
		public void BlobTemplate()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);

			var template = StandardTemplates.Blob(100);
			Assert.That(template.Width, Is.EqualTo(100));
			Assert.That(template.Components, Is.Null);
			Assert.That(template.GetValue(mockData.Object, 100), Is.Null);
		}

		[Test]
		public void DoubleTemplate()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<double>(100)).Returns(42d);

			Assert.That(StandardTemplates.Double.Width, Is.EqualTo(8));
			Assert.That(StandardTemplates.Double.Components, Is.Null);
			Assert.That(StandardTemplates.Double.GetValue(mockData.Object, 100), Is.EqualTo(42d));
		}

		[Test]
		public void EnumTemplate()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<TypeCode>(100)).Returns(TypeCode.Double);

			var template = StandardTemplates.Enum<TypeCode>();
			Assert.That(template.Width, Is.EqualTo(4));
			Assert.That(template.Components, Is.Null);
			Assert.That(template.GetValue(mockData.Object, 100), Is.EqualTo(TypeCode.Double));
		}

		[Test]
		public void GuidTemplate()
		{
			var value = Guid.NewGuid();
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<Guid>(100)).Returns(value);

			Assert.That(StandardTemplates.Guid.Width, Is.EqualTo(16));
			Assert.That(StandardTemplates.Guid.Components, Is.Null);
			Assert.That(StandardTemplates.Guid.GetValue(mockData.Object, 100), Is.EqualTo(value));
		}

		[Test]
		public void Int16Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<short>(100)).Returns(42);

			Assert.That(StandardTemplates.Int16.Width, Is.EqualTo(2));
			Assert.That(StandardTemplates.Int16.Components, Is.Null);
			Assert.That(StandardTemplates.Int16.GetValue(mockData.Object, 100), Is.EqualTo((short)42));
		}

		[Test]
		public void Int32Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<int>(100)).Returns(42);

			Assert.That(StandardTemplates.Int32.Width, Is.EqualTo(4));
			Assert.That(StandardTemplates.Int32.Components, Is.Null);
			Assert.That(StandardTemplates.Int32.GetValue(mockData.Object, 100), Is.EqualTo(42));
		}

		[Test]
		public void Int64Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<long>(100)).Returns(42L);

			Assert.That(StandardTemplates.Int64.Width, Is.EqualTo(8));
			Assert.That(StandardTemplates.Int64.Components, Is.Null);
			Assert.That(StandardTemplates.Int64.GetValue(mockData.Object, 100), Is.EqualTo(42L));
		}

		[Test]
		public void Int8Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<sbyte>(100)).Returns(42);

			Assert.That(StandardTemplates.Int8.Width, Is.EqualTo(1));
			Assert.That(StandardTemplates.Int8.Components, Is.Null);
			Assert.That(StandardTemplates.Int8.GetValue(mockData.Object, 100), Is.EqualTo((sbyte)42));
		}

		[Test]
		public void SingleTemplate()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<float>(100)).Returns(42);

			Assert.That(StandardTemplates.Single.Width, Is.EqualTo(4));
			Assert.That(StandardTemplates.Single.Components, Is.Null);
			Assert.That(StandardTemplates.Single.GetValue(mockData.Object, 100), Is.EqualTo(42f));
		}

		[Test]
		public void TextTemplate()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.ReadText(100, 5, Encoding.UTF8)).Returns("Text");

			var template = StandardTemplates.Text(5, Encoding.UTF8);
			Assert.That(template.Width, Is.EqualTo(5));
			Assert.That(template.Components, Is.Null);
			Assert.That(template.GetValue(mockData.Object, 100), Is.EqualTo("Text"));
		}

		[Test]
		public void UInt16Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<ushort>(100)).Returns(42);

			Assert.That(StandardTemplates.UInt16.Width, Is.EqualTo(2));
			Assert.That(StandardTemplates.UInt16.Components, Is.Null);
			Assert.That(StandardTemplates.UInt16.GetValue(mockData.Object, 100), Is.EqualTo((ushort)42));
		}

		[Test]
		public void UInt32Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<uint>(100)).Returns(42);

			Assert.That(StandardTemplates.UInt32.Width, Is.EqualTo(4));
			Assert.That(StandardTemplates.UInt32.Components, Is.Null);
			Assert.That(StandardTemplates.UInt32.GetValue(mockData.Object, 100), Is.EqualTo(42u));
		}

		[Test]
		public void UInt64Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<ulong>(100)).Returns(42ul);

			Assert.That(StandardTemplates.UInt64.Width, Is.EqualTo(8));
			Assert.That(StandardTemplates.UInt64.Components, Is.Null);
			Assert.That(StandardTemplates.UInt64.GetValue(mockData.Object, 100), Is.EqualTo(42ul));
		}

		[Test]
		public void UInt8Template()
		{
			var mockData = new Mock<IDataSource>(MockBehavior.Strict);
			mockData.Setup(x => x.Read<byte>(100)).Returns(42);

			Assert.That(StandardTemplates.UInt8.Width, Is.EqualTo(1));
			Assert.That(StandardTemplates.UInt8.Components, Is.Null);
			Assert.That(StandardTemplates.UInt8.GetValue(mockData.Object, 100), Is.EqualTo((byte)42));
		}
	}
}
