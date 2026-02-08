// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace HexView.Framework
{
	public sealed class RepeatingNodeTemplate : IStructuralNodeTemplate, IList<Component>, IReadOnlyList<Component>
	{
		public RepeatingNodeTemplate(IStructuralNodeTemplate template, int itemCount, Func<int, long, string> getName)
			: this(template, itemCount, getName, template.Width)
		{
		}

		public RepeatingNodeTemplate(IStructuralNodeTemplate itemTemplate, int itemCount, Func<int, long, string> getName, long itemWidthOverride)
		{
			ArgumentNullException.ThrowIfNull(itemTemplate);
			ArgumentNullException.ThrowIfNull(getName);

			_itemTemplate = itemTemplate;
			_getName = getName;
			_itemCount = itemCount;
			_itemWidth = itemWidthOverride;
		}

		public long Width => _itemCount * _itemWidth;
		public object? GetValue(IDataSource data, long offset) => null;
		Component GetComponent(int index) => GetComponent(index, index * _itemWidth);
		Component GetComponent(int index, long offset) => new Component(_getName(index, offset), _itemTemplate, offset);

		IEnumerator<Component> GetEnumerator()
		{
			var offset = 0L;

			for (var i = 0; i < _itemCount; i++)
			{
				yield return GetComponent(i, offset);
				offset += _itemWidth;
			}
		}

		int IndexOf(Component component)
		{
			if (component.Template != _itemTemplate) return -1;
			if (component.Offset % _itemWidth != 0) return -1;

			var longIndex = component.Offset / _itemWidth;
			if (longIndex < 0 || longIndex >= _itemCount) return -1;

			var index = (int)longIndex;
			if (_getName(index, component.Offset) != component.Name) return -1;
			return index;
		}

		#region IReadOnlyList<Component> Members

		Component IReadOnlyList<Component>.this[int index] => GetComponent(index);

		#endregion

		#region IReadOnlyCollection<Component> Members

		int IReadOnlyCollection<Component>.Count => _itemCount;

		#endregion

		#region IStructuralNodeTemplate Members

		IReadOnlyList<Component> IStructuralNodeTemplate.Components => this;

		#endregion

		#region IList<Component> Members

		void IList<Component>.Insert(int index, Component item) => throw new NotSupportedException();
		void IList<Component>.RemoveAt(int index) => throw new NotSupportedException();
		int IList<Component>.IndexOf(Component item) => IndexOf(item);

		Component IList<Component>.this[int index]
		{
			get => GetComponent(index);
			set => throw new NotSupportedException();
		}

		#endregion

		#region ICollection<Component> Members

		void ICollection<Component>.Add(Component item) => throw new NotSupportedException();
		void ICollection<Component>.Clear() => throw new NotSupportedException();
		void ICollection<Component>.CopyTo(Component[] array, int arrayIndex) => throw new NotSupportedException();
		bool ICollection<Component>.Remove(Component item) => throw new NotSupportedException();

		bool ICollection<Component>.Contains(Component item) => IndexOf(item) >= 0;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int ICollection<Component>.Count => _itemCount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<Component>.IsReadOnly => true;

		#endregion

		#region IEnumerable<Component> Members

		IEnumerator<Component> IEnumerable<Component>.GetEnumerator() => GetEnumerator();

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly int _itemCount;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly long _itemWidth;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly IStructuralNodeTemplate _itemTemplate;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Func<int, long, string> _getName;
	}
}
