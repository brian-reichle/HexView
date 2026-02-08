// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;
using NUnit.Framework;

namespace HexView.Framework.Test.StructuralNodeTemplate
{
	[TestFixture]
	class StandardTemplatesTest
	{
		[Test]
		public void BlobTemplate()
		{
			var template = StandardTemplates.Blob(100);
			Assert.That(template, Is.InstanceOf<BlobNodeTemplate>());
			Assert.That(template.Width, Is.EqualTo(100));
		}

		[Test]
		public void SimpleTemplates()
		{
			using (Assert.EnterMultipleScope())
			{
				Assert.That(StandardTemplates.Double, Is.InstanceOf<SimpleNodeTemplate<double>>());
				Assert.That(StandardTemplates.Guid, Is.InstanceOf<SimpleNodeTemplate<Guid>>());
				Assert.That(StandardTemplates.Int16, Is.InstanceOf<SimpleNodeTemplate<short>>());
				Assert.That(StandardTemplates.Int32, Is.InstanceOf<SimpleNodeTemplate<int>>());
				Assert.That(StandardTemplates.Int64, Is.InstanceOf<SimpleNodeTemplate<long>>());
				Assert.That(StandardTemplates.Int8, Is.InstanceOf<SimpleNodeTemplate<sbyte>>());
				Assert.That(StandardTemplates.Single, Is.InstanceOf<SimpleNodeTemplate<float>>());
				Assert.That(StandardTemplates.UInt16, Is.InstanceOf<SimpleNodeTemplate<ushort>>());
				Assert.That(StandardTemplates.UInt32, Is.InstanceOf<SimpleNodeTemplate<uint>>());
				Assert.That(StandardTemplates.UInt64, Is.InstanceOf<SimpleNodeTemplate<ulong>>());
				Assert.That(StandardTemplates.UInt8, Is.InstanceOf<SimpleNodeTemplate<byte>>());
			}
		}

		[Test]
		public void EnumTemplate()
		{
			Assert.That(StandardTemplates.Enum<TypeCode>(), Is.InstanceOf<SimpleNodeTemplate<TypeCode>>());
		}

		[Test]
		public void TextTemplate()
		{
			var template = StandardTemplates.Text(40, Encoding.UTF8);
			Assert.That(template, Is.InstanceOf<TextNodeTemplate>());
		}
	}
}
