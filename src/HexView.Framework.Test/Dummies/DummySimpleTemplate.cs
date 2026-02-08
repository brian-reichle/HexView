using System.Collections.Generic;

namespace HexView.Framework.Test
{
	sealed class DummySimpleTemplate : IStructuralNodeTemplate
	{
		public DummySimpleTemplate(int width)
		{
			Width = width;
		}

		public long Width { get; }
		public IReadOnlyList<Component> Components => [];
		public object? GetValue(IDataSource data, long offset) => null;
	}
}
