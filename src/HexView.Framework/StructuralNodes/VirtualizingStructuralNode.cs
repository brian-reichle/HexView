// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace HexView.Framework
{
	public abstract class VirtualizingStructuralNode : IStructuralNode, IList<IStructuralNode>, IList, IReadOnlyList<IStructuralNode>
	{
		protected VirtualizingStructuralNode(IStructuralNode? parent)
		{
			Parent = parent;
		}

		public IStructuralNode? Parent { get; }
		public abstract string Name { get; }
		public abstract ByteRange? ByteRange { get; }
		protected abstract int Count { get; }
		public virtual object? Value => null;
		public IReadOnlyList<IStructuralNode> Children => this;

		protected abstract IStructuralNode CreateChildNode(int index);

		IEnumerator<IStructuralNode> GetEnumerator()
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				yield return CreateChildNode(i);
			}
		}

		int IndexOf(IStructuralNode node)
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				if (CreateChildNode(i) == node)
				{
					return i;
				}
			}

			return -1;
		}

		#region IReadOnlyList<IStructuralNode> Members

		IStructuralNode IReadOnlyList<IStructuralNode>.this[int index] => CreateChildNode(index);

		#endregion

		#region IReadOnlyCollection<IStructuralNode> Members

		int IReadOnlyCollection<IStructuralNode>.Count => Count;

		#endregion

		#region IList<IStructuralNode> Members

		void IList<IStructuralNode>.Insert(int index, IStructuralNode item) => throw new NotSupportedException();
		void IList<IStructuralNode>.RemoveAt(int index) => throw new NotSupportedException();

		int IList<IStructuralNode>.IndexOf(IStructuralNode item) => IndexOf(item);

		IStructuralNode IList<IStructuralNode>.this[int index]
		{
			[DebuggerStepThrough]
			get => CreateChildNode(index);
			set => throw new NotSupportedException();
		}

		#endregion

		#region IList Members

		int IList.Add(object value) => throw new NotSupportedException();
		void IList.Clear() => throw new NotSupportedException();
		void IList.Insert(int index, object value) => throw new NotSupportedException();
		void IList.Remove(object value) => throw new NotSupportedException();
		void IList.RemoveAt(int index) => throw new NotSupportedException();

		bool IList.Contains(object value) => IndexOf((IStructuralNode)value) >= 0;
		int IList.IndexOf(object value) => IndexOf((IStructuralNode)value);

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsFixedSize => true;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool IList.IsReadOnly => true;

		object IList.this[int index]
		{
			get => CreateChildNode(index);
			set => throw new NotSupportedException();
		}

		#endregion

		#region ICollection<IStructuralNode> Members

		void ICollection<IStructuralNode>.Add(IStructuralNode item) => throw new NotSupportedException();
		void ICollection<IStructuralNode>.Clear() => throw new NotSupportedException();
		bool ICollection<IStructuralNode>.Remove(IStructuralNode item) => throw new NotSupportedException();

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int ICollection<IStructuralNode>.Count => Count;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection<IStructuralNode>.IsReadOnly => true;

		bool ICollection<IStructuralNode>.Contains(IStructuralNode item) => IndexOf(item) >= 0;

		void ICollection<IStructuralNode>.CopyTo(IStructuralNode[] array, int arrayIndex)
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				array[i] = CreateChildNode(arrayIndex + i);
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo(Array array, int index)
		{
			var count = Count;

			for (var i = 0; i < count; i++)
			{
				array.SetValue(CreateChildNode(i), index + i);
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		int ICollection.Count => Count;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		bool ICollection.IsSynchronized => false;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		object ICollection.SyncRoot => this;

		#endregion

		#region IEnumerable Members

		[DebuggerStepThrough]
		IEnumerator<IStructuralNode> IEnumerable<IStructuralNode>.GetEnumerator() => GetEnumerator();

		#endregion

		#region IEnumerable Members

		[DebuggerStepThrough]
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}
