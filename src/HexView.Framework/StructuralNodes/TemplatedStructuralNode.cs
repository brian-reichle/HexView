// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView.Framework
{
	public sealed class TemplatedStructuralNode : VirtualizingStructuralNode
	{
		public TemplatedStructuralNode(IDataSource data, IStructuralNode? parent, string name, IStructuralNodeTemplate template, long offset)
			: base(parent)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			if (template == null) throw new ArgumentNullException(nameof(template));

			_data = data;
			Name = name;
			_template = template;
			_offset = offset;
		}

		public override string Name { get; }
		public override Range ByteRange => new Range(_offset, _template.Width);
		protected override int Count => _template.Components.Count;
		public override object? Value => _template.GetValue(_data, _offset);

		protected override IStructuralNode CreateChildNode(int index)
		{
			var component = _template.Components[index];
			return new TemplatedStructuralNode(_data, this, component.Name, component.Template, component.Offset + _offset);
		}

		readonly IDataSource _data;
		readonly IStructuralNodeTemplate _template;
		readonly long _offset;
	}
}
