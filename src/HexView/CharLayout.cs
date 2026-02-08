// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using HexView.Framework;

namespace HexView
{
	sealed class CharLayout
	{
		public const int ByteSectionIndex = 8;
		const int CharSectionIndex = 40;

		const int Section_Index = 1;
		const int Section_Byte = 2;
		const int Section_Char = 3;

		public CharLayout(double cellWidth, double cellHeight)
		{
			CellHeight = cellHeight;
			CellWidth = cellWidth;
			_columnPositions = new double[_baseOffsets.Length];

			var x = 0d;

			for (var i = 0; i < _columnPositions.Length; i++)
			{
				_columnPositions[i] = x;
				x += _baseOffsets[i] * cellWidth;
			}

			RowWidth = x;
			_full = new CellSpacing(16, _columnPositions);
		}

		public double CellHeight { get; }
		public double CellWidth { get; }
		public double RowWidth { get; }

		public IList<double> GetSpacing(int bytes)
			=> bytes == 16 ? _full : new CellSpacing(bytes, _columnPositions);

		public Geometry? CreateSelectionGeometryFromByteRange(long start, long end, HexBoxDisplaySection section)
		{
			var firstLine = start >> 4;
			var lastLine = end >> 4;

			if (firstLine == lastLine)
			{
				return new RectangleGeometry(CreateSelectionRectangleFromCells(firstLine, start & 0xF, lastLine, end & 0xF, section));
			}

			Geometry? lead;
			Geometry? body;
			Geometry? trail;

			if ((start & 0xF) == 0)
			{
				lead = null;
			}
			else
			{
				lead = new RectangleGeometry(CreateSelectionRectangleFromCells(firstLine, start & 0xF, firstLine, 0xF, section));
				firstLine++;
			}

			if ((end & 0xF) == 0xF)
			{
				trail = null;
			}
			else
			{
				trail = new RectangleGeometry(CreateSelectionRectangleFromCells(lastLine, 0, lastLine, end & 0xF, section));
				lastLine--;
			}

			if (lastLine < firstLine)
			{
				if (lead == null) return trail;
				if (trail == null) return lead;
				return new CombinedGeometry(GeometryCombineMode.Union, lead, trail);
			}

			body = new RectangleGeometry(CreateSelectionRectangleFromCells(firstLine, 0, lastLine, 0xF, section));

			if (lead != null)
			{
				body = new CombinedGeometry(GeometryCombineMode.Union, lead, body);
			}

			if (trail != null)
			{
				body = new CombinedGeometry(GeometryCombineMode.Union, body, trail);
			}

			return body;
		}

		public Rect CreateBoundingBox(long start, long end, HexBoxDisplaySection section)
		{
			var firstRow = start >> 4;
			var lastRow = end >> 4;

			long firstColumn;
			long lastColumn;

			if (firstRow == lastRow)
			{
				firstColumn = start & 0xF;
				lastColumn = end & 0xF;
			}
			else
			{
				firstColumn = 0x0;
				lastColumn = 0xF;
			}

			return CreateSelectionRectangleFromCells(firstRow, firstColumn, lastRow, lastColumn, section);
		}

		public ByteRange? RangeFromDrag(Point fromPoint, Point toPoint)
		{
			var category = CategoriseDrag(fromPoint.X, toPoint.X);

			if (fromPoint.Y > toPoint.Y)
			{
				(toPoint, fromPoint) = (fromPoint, toPoint);
			}

			return category switch
			{
				Section_Index => RangeFromDrag_FullLines(fromPoint, toPoint),
				Section_Byte => RangeFromDrag_Bytes(fromPoint, toPoint),
				Section_Char => RangeFromDrag_Chars(fromPoint, toPoint),
				_ => null,
			};
		}

		ByteRange RangeFromDrag_FullLines(Point fromPoint, Point toPoint)
		{
			var line1 = (long)Math.Floor(fromPoint.Y / CellHeight);
			var line2 = (long)Math.Floor(toPoint.Y / CellHeight);
			line1 <<= 4;

			return new ByteRange(line1, (line2 << 4) - line1 + 0x10);
		}

		ByteRange RangeFromDrag_Bytes(Point fromPoint, Point toPoint)
		{
			var first = (long)Math.Floor(fromPoint.Y / CellHeight);
			var last = (long)Math.Floor(toPoint.Y / CellHeight);

			if (first == last && fromPoint.X > toPoint.X)
			{
				(toPoint, fromPoint) = (fromPoint, toPoint);
			}

			first = (first << 4) + GetByteColumn(fromPoint.X, roundLeft: false);
			last = (last << 4) + GetByteColumn(toPoint.X, roundLeft: true);

			return new ByteRange(first, last - first + 1);
		}

		ByteRange RangeFromDrag_Chars(Point fromPoint, Point toPoint)
		{
			var first = (long)Math.Floor(fromPoint.Y / CellHeight);
			var last = (long)Math.Floor(toPoint.Y / CellHeight);

			if (first == last && fromPoint.X > toPoint.X)
			{
				(toPoint, fromPoint) = (fromPoint, toPoint);
			}

			first = (first << 4) + GetCharColumn(fromPoint.X, roundLeft: false);
			last = (last << 4) + GetCharColumn(toPoint.X, roundLeft: true);

			return new ByteRange(first, last - first + 1);
		}

		int CategoriseDrag(double fromX, double toX)
		{
			var indexRightBound = _columnPositions[ByteSectionIndex - 1] + CellWidth;
			var charLeftBound = _columnPositions[CharSectionIndex];
			var charRightBound = _columnPositions[CharSectionIndex + 15] + CellWidth;
			var byteLeftBound = _columnPositions[ByteSectionIndex];
			var byteRightBound = _columnPositions[CharSectionIndex - 1] + CellWidth;

			if (fromX < indexRightBound)
			{
				return Section_Index;
			}
			else if (fromX >= charRightBound)
			{
				if (toX >= charRightBound)
				{
					return Section_Index;
				}
				else
				{
					return Section_Char;
				}
			}
			else if (fromX >= charLeftBound)
			{
				return Section_Char;
			}
			else if (fromX < byteLeftBound)
			{
				if (toX >= byteLeftBound)
				{
					return Section_Byte;
				}
				else
				{
					return Section_Index;
				}
			}
			else if (fromX >= byteRightBound)
			{
				if (toX < byteRightBound)
				{
					return Section_Byte;
				}
				else if (toX >= charLeftBound)
				{
					return Section_Char;
				}
				else
				{
					return Section_Index;
				}
			}
			else
			{
				return Section_Byte;
			}
		}

		int GetByteColumn(double x, bool roundLeft)
			=> (GetColumn(x, ByteSectionIndex, CharSectionIndex - 1, roundLeft) - ByteSectionIndex) >> 1;

		int GetCharColumn(double x, bool roundLeft)
			=> GetColumn(x, CharSectionIndex, CharSectionIndex + 15, roundLeft) - CharSectionIndex;

		int GetColumn(double x, int lowerBound, int upperBound, bool roundLeft)
		{
			if (roundLeft)
			{
				for (var i = upperBound; i >= lowerBound; i--)
				{
					if (_columnPositions[i] < x)
					{
						return i;
					}
				}

				return lowerBound - 1;
			}
			else
			{
				x -= CellWidth;

				for (var i = lowerBound; i <= upperBound; i++)
				{
					if (_columnPositions[i] >= x)
					{
						return i;
					}
				}

				return upperBound + 1;
			}
		}

		Rect CreateSelectionRectangleFromCells(long firstRow, long firstColumn, long lastRow, long lastColumn, HexBoxDisplaySection section)
		{
			double left, right;

			switch (section)
			{
				case HexBoxDisplaySection.Byte:
					left = _columnPositions[ByteSectionIndex + (firstColumn << 1)];
					right = _columnPositions[ByteSectionIndex + 1 + (lastColumn << 1)] + CellWidth;
					break;

				case HexBoxDisplaySection.Char:
					left = _columnPositions[CharSectionIndex + firstColumn];
					right = _columnPositions[CharSectionIndex + lastColumn] + CellWidth;
					break;

				default: throw new ArgumentOutOfRangeException(nameof(section));
			}

			return new Rect(
				left,
				firstRow * CellHeight,
				right - left,
				(lastRow - firstRow + 1) * CellHeight);
		}

		readonly double[] _columnPositions;
		readonly CellSpacing _full;

		// |         | Byte Section                                      | Char Section
		// xxxxxxxx  xx xx xx xx  xx xx xx xx  xx xx xx xx  xx xx xx xx  xxxx xxxx xxxx xxxx
		static readonly int[] _baseOffsets =
		[
			1, 1, 1, 1, 1, 1, 1, 3,
			1, 2, 1, 2, 1, 2, 1, 3,
			1, 2, 1, 2, 1, 2, 1, 3,
			1, 2, 1, 2, 1, 2, 1, 3,
			1, 2, 1, 2, 1, 2, 1, 3,
			1, 1, 1, 2,
			1, 1, 1, 2,
			1, 1, 1, 2,
			1, 1, 1, 1,
			0,
		];

		sealed class CellSpacing : IList<double>
		{
			public CellSpacing(int byteCount, double[] columnPositions)
			{
				_byteCount = byteCount;
				Count = ByteSectionIndex + (byteCount * 3);
				_columnPositions = columnPositions;
			}

			public int Count { get; }

			public double this[int index]
			{
				get
				{
					var virtualCharSectionIndex = ByteSectionIndex + (_byteCount << 1);

					int a, b;

					if (index >= virtualCharSectionIndex)
					{
						a = index - virtualCharSectionIndex + CharSectionIndex;
						b = a + 1;
					}
					else if (index == virtualCharSectionIndex - 1)
					{
						a = index;
						b = CharSectionIndex;
					}
					else
					{
						a = index;
						b = a + 1;
					}

					return _columnPositions[b] - _columnPositions[a];
				}
			}

			public IEnumerator<double> GetEnumerator()
			{
				var count = Count;
				var prev = _columnPositions[0];

				for (var i = 1; i <= count; i++)
				{
					var next = _columnPositions[i];
					yield return next - prev;
					prev = next;
				}
			}

			#region IList<double> Members

			int IList<double>.IndexOf(double item) => throw new NotSupportedException();
			void IList<double>.Insert(int index, double item) => throw new NotSupportedException();
			void IList<double>.RemoveAt(int index) => throw new NotSupportedException();

			double IList<double>.this[int index]
			{
				[DebuggerStepThrough]
				get => this[index];
				set => throw new NotSupportedException();
			}

			#endregion

			#region ICollection<double> Members

			void ICollection<double>.Add(double item) => throw new NotSupportedException();
			void ICollection<double>.Clear() => throw new NotSupportedException();
			bool ICollection<double>.Contains(double item) => throw new NotSupportedException();

			void ICollection<double>.CopyTo(double[] array, int arrayIndex)
			{
				var count = Count;

				for (var i = 0; i < count; i++)
				{
					array[arrayIndex + i] = this[i];
				}
			}

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			bool ICollection<double>.IsReadOnly => true;

			bool ICollection<double>.Remove(double item) => throw new NotSupportedException();

			#endregion

			#region IEnumerable Members

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			#endregion

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly int _byteCount;
			readonly double[] _columnPositions;
		}
	}
}
