using System;
using System.Collections.Generic;

namespace HexView.Framework.Test
{
	sealed class DummySimpleTemplate : IStructuralNodeTemplate
	{
		public DummySimpleTemplate(int width, IReadOnlyList<Component>? components = null, Func<IDataSource, long, object?>? valueAccessor = null)
		{
			Width = width;
			Components = components ?? [];
			_valueAccessor = valueAccessor;
		}

		public long Width { get; }
		public IReadOnlyList<Component> Components { get; }
		public object? GetValue(IDataSource data, long offset) => _valueAccessor?.Invoke(data, offset);

		readonly Func<IDataSource, long, object?>? _valueAccessor;
	}
}
