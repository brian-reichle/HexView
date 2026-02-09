// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace HexView.Framework;

public sealed class CompoundNodeTemplate : IStructuralNodeTemplate, IEnumerable<Component>
{
	public CompoundNodeTemplate()
	{
		_list = [];
		_components = new ReadOnlyCollection<Component>(_list);
		Width = 0;
	}

	public long Width { get; private set; }
	public object? GetValue(IDataSource data, long offset) => null;
	public IReadOnlyList<Component> Components => _components;
	public void OverrideWidth(int width) => Width = width;

	public void Add(string name, IStructuralNodeTemplate template) => Add(PositionMode.RelativeToLast, 0, name, template);

	public void Add(PositionMode mode, long offset, string name, IStructuralNodeTemplate template)
	{
		ArgumentNullException.ThrowIfNull(template);

		long relativeOffset;

		switch (mode)
		{
			case PositionMode.RelativeToLast:
				if (_list.Count == 0)
				{
					relativeOffset = 0;
				}
				else
				{
					var index = _list.Count - 1;
					var previousComponent = _list[index];
					relativeOffset = previousComponent.Offset + previousComponent.Template.Width;
				}
				break;

			case PositionMode.RelativeToParent:
				relativeOffset = 0;
				break;

			default: throw new ArgumentOutOfRangeException(nameof(mode));
		}

		relativeOffset += offset;

		_list.Add(new Component(name, template, relativeOffset));

		relativeOffset += template.Width;

		if (relativeOffset > Width)
		{
			Width = relativeOffset;
		}
	}

	public void RoundWidthUpToBoundary(int width)
	{
		if (width == 0 || ((width - 1) & width) != 0) throw new ArgumentOutOfRangeException(nameof(width));

		var mask = width - 1;

		Width = (Width + mask) & ~mask;
	}

	#region IEnumerable<Component> Members

	IEnumerator<Component> IEnumerable<Component>.GetEnumerator() => Components.GetEnumerator();

	#endregion

	#region IEnumerable Members

	IEnumerator IEnumerable.GetEnumerator() => Components.GetEnumerator();

	#endregion

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	readonly ReadOnlyCollection<Component> _components;
	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	readonly List<Component> _list;
}
