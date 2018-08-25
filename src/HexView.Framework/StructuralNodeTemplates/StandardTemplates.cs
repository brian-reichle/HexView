// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;

namespace HexView.Framework
{
	public static class StandardTemplates
	{
		public static readonly IStructuralNodeTemplate Int8 = new SimpleNodeTemplate<sbyte>();
		public static readonly IStructuralNodeTemplate Int16 = new SimpleNodeTemplate<short>();
		public static readonly IStructuralNodeTemplate Int32 = new SimpleNodeTemplate<int>();
		public static readonly IStructuralNodeTemplate Int64 = new SimpleNodeTemplate<long>();

		public static readonly IStructuralNodeTemplate UInt8 = new SimpleNodeTemplate<byte>();
		public static readonly IStructuralNodeTemplate UInt16 = new SimpleNodeTemplate<ushort>();
		public static readonly IStructuralNodeTemplate UInt32 = new SimpleNodeTemplate<uint>();
		public static readonly IStructuralNodeTemplate UInt64 = new SimpleNodeTemplate<ulong>();

		public static readonly IStructuralNodeTemplate Guid = new SimpleNodeTemplate<Guid>();

		public static IStructuralNodeTemplate Enum<T>()
			where T : struct
		{
			if (!typeof(T).IsEnum) return null;

			return new SimpleNodeTemplate<T>();
		}

		public static IStructuralNodeTemplate Text(int width, Encoding encoding) => new TextNodeTemplate(width, encoding);
		public static IStructuralNodeTemplate Blob(long width) => new BlobNodeTemplate(width);
	}
}
