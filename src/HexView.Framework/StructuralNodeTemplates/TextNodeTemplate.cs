// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HexView.Framework
{
	sealed class TextNodeTemplate : IStructuralNodeTemplate
	{
		public TextNodeTemplate(int width, Encoding encoding)
		{
			if (encoding == null) throw new ArgumentNullException(nameof(encoding));

			_width = width;
			_encoding = encoding;
		}

		public long Width => _width;
		public IReadOnlyList<Component> Components => Array.Empty<Component>();
		public object? GetValue(IDataSource data, long offset) => data.ReadText(offset, _width, _encoding);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly int _width;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Encoding _encoding;
	}
}
