// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using HexView.Framework;

namespace HexView;

sealed class HexBoxView : FrameworkElement, IScrollInfo
{
	public HexBoxView(HexBox parent)
	{
		_parent = parent;
	}

	public void InvalidateTypography()
	{
		_charSet = null;
		_layout = null;
		InvalidateMeasure();
		InvalidateVisual();
	}

	public void InvalidateData()
	{
		_lineSet = null;
		InvalidateMeasure();
		InvalidateVisual();
	}

	public void JumpToOffset(long offset, long length)
	{
		if (offset < 0 || _layout == null) return;
		var blob = _parent.Data;

		if (blob == null || offset >= blob.ByteCount) return;

		if (length <= 0) length = 1;

		var box = _layout.CreateBoundingBox(offset, offset + length - 1, HexBoxDisplaySection.Byte);
		SetVerticalOffset(box.Top, box.Height);
		SetHorizontalOffset(box.Left, box.Width);
	}

	public void ScrollToTop() => SetVerticalOffset(0);
	public void ScrollToEnd() => SetVerticalOffset(_extentHeight - _viewportHeight);

	public Point GetExtentPoint(MouseDevice device)
	{
		var point = device.GetPosition(this);

		var x = point.X + _horizontalOffset;
		var y = point.Y + _verticalOffset;

		if (x < 0)
		{
			x = 0;
		}
		else if (x > _extentWidth)
		{
			x = _extentWidth;
		}

		if (y < 0)
		{
			y = 0;
		}
		else if (y > _extentHeight)
		{
			y = _extentHeight;
		}

		return new Point(x, y);
	}

	public ByteRange? RangeFromDrag(Point fromExtentPoint, Point toExtentPoint)
	{
		return _layout?.RangeFromDrag(fromExtentPoint, toExtentPoint);
	}

	protected override Size MeasureOverride(Size availableSize)
	{
		var blob = _parent.Data;

		_charSet ??= new HexCharSet(_parent);
		_layout ??= new CharLayout(_charSet.CellWidth, _charSet.CellHeight);

		var lines = blob == null ? 1 : (blob.ByteCount + 15) >> 4;

		_dataWidth = _layout.RowWidth;
		_dataHeight = _layout.CellHeight * lines;

		var desiredWidth = double.IsInfinity(availableSize.Width) ? _dataWidth : Math.Min(_dataWidth, availableSize.Width);
		var desiredHeight = double.IsInfinity(availableSize.Height) ? _dataHeight : Math.Min(_dataHeight, availableSize.Height);
		return new Size(desiredWidth, desiredHeight);
	}

	protected override Size ArrangeOverride(Size finalSize)
	{
		if (double.IsInfinity(finalSize.Width))
		{
			_viewportWidth = _extentWidth = _dataWidth;
		}
		else
		{
			_viewportWidth = finalSize.Width;
			_extentWidth = Math.Max(_viewportWidth, _dataWidth);
		}

		if (double.IsInfinity(finalSize.Height))
		{
			_viewportHeight = _extentHeight = _dataHeight;
		}
		else
		{
			_viewportHeight = finalSize.Height;
			_extentHeight = Math.Max(_viewportHeight, _dataHeight);
		}

		SetVerticalOffset(_verticalOffset);
		SetHorizontalOffset(_horizontalOffset);

		InvalidateOwner();
		return new Size(_viewportWidth, _viewportHeight);
	}

	protected override void OnRender(DrawingContext drawingContext)
	{
		drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0, 0, ActualWidth, ActualHeight));

		if (_layout == null) return;

		var blob = _parent.Data;
		if (blob == null) return;

		var firstLine = (int)Math.Floor(_verticalOffset / _layout.CellHeight);
		var lastLine = Math.Min((int)Math.Ceiling((_verticalOffset + _viewportHeight) / _layout.CellHeight), (blob.ByteCount - 1) >> 4);
		var lines = GetLines(firstLine, (int)(lastLine - firstLine + 1));

		drawingContext.PushTransform(new TranslateTransform(-_horizontalOffset, -(_verticalOffset % _layout.CellHeight)));
		PaintLineSet(drawingContext, TextBlock.GetForeground(_parent), lines);

		var selectionStart = _parent.SelectionStart;
		var selectionEnd = selectionStart + _parent.SelectionLength - 1;
		var firstByte = firstLine << 4;
		var lastByte = Math.Min(blob.ByteCount - 1, lastLine << 4 + 15);

		if (selectionStart < firstByte) selectionStart = firstByte;
		if (selectionEnd > lastByte) selectionEnd = lastByte;

		if (selectionStart <= selectionEnd)
		{
			var from = selectionStart - firstByte;
			var to = selectionEnd - firstByte;

			var selectionGeometry = new CombinedGeometry(
				GeometryCombineMode.Union,
				_layout.CreateSelectionGeometryFromByteRange(from, to, HexBoxDisplaySection.Byte),
				_layout.CreateSelectionGeometryFromByteRange(from, to, HexBoxDisplaySection.Char));

			drawingContext.PushOpacity(_parent.SelectionOpacity);
			drawingContext.DrawGeometry(_parent.SelectionBrush, null, selectionGeometry);
			drawingContext.Pop();
		}

		drawingContext.Pop();
	}

	#region IScrollInfo Methods

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool IScrollInfo.CanHorizontallyScroll
	{
		[DebuggerStepThrough]
		get => _canHorizontallyScroll;
		[DebuggerStepThrough]
		set => _canHorizontallyScroll = value;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	bool IScrollInfo.CanVerticallyScroll
	{
		[DebuggerStepThrough]
		get => _canVerticallyScroll;
		[DebuggerStepThrough]
		set => _canVerticallyScroll = value;
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	double IScrollInfo.ExtentHeight => _extentHeight;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	double IScrollInfo.ExtentWidth => _extentWidth;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	double IScrollInfo.HorizontalOffset => _horizontalOffset;

	public void LineDown()
	{
		if (_layout != null)
		{
			SetVerticalOffset(_verticalOffset + _layout.CellHeight);
		}
	}

	public void LineLeft()
	{
		if (_layout != null)
		{
			SetHorizontalOffset(_horizontalOffset - _layout.CellWidth * 2);
		}
	}

	public void LineRight()
	{
		if (_layout != null)
		{
			SetHorizontalOffset(_horizontalOffset + _layout.CellWidth * 2);
		}
	}

	public void LineUp()
	{
		if (_layout != null)
		{
			SetVerticalOffset(_verticalOffset - _layout.CellHeight);
		}
	}

	Rect IScrollInfo.MakeVisible(Visual visual, Rect rectangle)
	{
		var newRect = visual.TransformToAncestor(this).TransformBounds(rectangle);

		SetVerticalOffset(ChooseOffset(newRect.Top, newRect.Height, _verticalOffset, _viewportHeight));
		SetHorizontalOffset(ChooseOffset(newRect.Left, newRect.Width, _horizontalOffset, _viewportWidth));

		return newRect;
	}

	public void MouseWheelDown()
	{
		if (_layout != null)
		{
			SetVerticalOffset(_verticalOffset + _layout.CellHeight * 3);
		}
	}

	public void MouseWheelLeft()
	{
		if (_layout != null)
		{
			SetHorizontalOffset(_horizontalOffset - _layout.CellWidth * 2);
		}
	}

	public void MouseWheelRight()
	{
		if (_layout != null)
		{
			SetHorizontalOffset(_horizontalOffset + _layout.CellWidth * 2);
		}
	}

	public void MouseWheelUp()
	{
		if (_layout != null)
		{
			SetVerticalOffset(_verticalOffset - _layout.CellHeight * 3);
		}
	}

	public void PageDown() => SetVerticalOffset(_verticalOffset + _viewportHeight * 0.9);
	public void PageLeft() => SetHorizontalOffset(_horizontalOffset - _viewportWidth * 0.9);
	public void PageRight() => SetHorizontalOffset(_horizontalOffset + _viewportWidth * 0.9);
	public void PageUp() => SetVerticalOffset(_verticalOffset - _viewportHeight * 0.9);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	ScrollViewer? IScrollInfo.ScrollOwner
	{
		[DebuggerStepThrough]
		get => _owner;
		[DebuggerStepThrough]
		set => _owner = value;
	}

	[DebuggerStepThrough]
	void IScrollInfo.SetHorizontalOffset(double offset) => SetHorizontalOffset(offset);

	[DebuggerStepThrough]
	void IScrollInfo.SetVerticalOffset(double offset) => SetVerticalOffset(offset);

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	double IScrollInfo.VerticalOffset => _verticalOffset;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	double IScrollInfo.ViewportHeight => _viewportHeight;

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	double IScrollInfo.ViewportWidth => _viewportWidth;

	#endregion

	static void PaintLineSet(DrawingContext context, Brush brush, LineSet lineSet)
	{
		var runs = lineSet.Runs;

		for (var i = 0; i < runs.Length; i++)
		{
			var run = runs[i];

			var rect = run.ComputeAlignmentBox();
			var blo = run.BaselineOrigin;

			var guideLines = new GuidelineSet(
				[rect.Left + blo.X, rect.Right + blo.X],
				[rect.Top + blo.Y, rect.Bottom + blo.Y]);

			context.PushGuidelineSet(guideLines);
			context.DrawGlyphRun(brush, run);
			context.Pop();
		}
	}

	LineSet GetLines(long startingLine, int count)
	{
		Debug.Assert(_layout != null, "We should not have reached this point if _layout is null.");
		Debug.Assert(_charSet != null, "We should not have reached this point if _charSet is null.");

		var lines = _lineSet;

		if (lines == null || lines.StartingLine != startingLine || lines.Runs.Length < count)
		{
			lines = CalculateLines(startingLine, count);
		}

		return _lineSet = lines;
	}

	LineSet CalculateLines(long startingLine, int count)
	{
		Debug.Assert(_layout != null, "We should not have reached this point if _layout is null.");
		Debug.Assert(_charSet != null, "We should not have reached this point if _charSet is null.");

		var blob = _parent.Data;

		if (blob == null || count == 0)
		{
			return new LineSet(startingLine, []);
		}

		var y = 0d;
		var runs = new GlyphRun[count];

		for (var i = 0; i < count; i++)
		{
			runs[i] = CreateRun(new Point(0, y), blob, (startingLine + i) << 4);
			y += _layout.CellHeight;
		}

		return new LineSet(startingLine, runs);
	}

	GlyphRun CreateRun(Point point, IDataSource block, long start)
	{
		Debug.Assert(_layout != null, "We should not have reached this point if _layout is null.");
		Debug.Assert(_charSet != null, "We should not have reached this point if _charSet is null.");

		var length = (int)(Math.Min(block.ByteCount, start + 16) - start);
		var chars = new char[CharLayout.ByteSectionIndex + (length * 3)];
		var tmp = start;

		for (var i = CharLayout.ByteSectionIndex - 1; i >= 0; i--)
		{
			chars[i] = HexHelper.HexChars[(int)(tmp & 0x0F)];
			tmp >>= 4;
		}

		var charIndex1 = CharLayout.ByteSectionIndex;
		var charIndex2 = charIndex1 + (length << 1);

		Span<byte> buffer = stackalloc byte[length];
		block.CopyTo(start, buffer);

		for (var i = 0; i < length; i++)
		{
			var b = buffer[i];
			chars[charIndex1++] = HexHelper.HexChars[b >> 4];
			chars[charIndex1++] = HexHelper.HexChars[b & 0x0F];
			chars[charIndex2++] = b < 32 || b >= 127 ? '.' : (char)b;
		}

		var scale = VisualTreeHelper.GetDpi(this);

		return _charSet.NewGlyphRun(point, chars, _layout.GetSpacing(length), (float)scale.PixelsPerDip);
	}

	void SetHorizontalOffset(double offset)
	{
		offset = ConstrainOffset(offset, _viewportWidth, _extentWidth);

		if (_horizontalOffset != offset)
		{
			_horizontalOffset = offset;
			InvalidateVisual();
			InvalidateOwner();
		}
	}

	void SetHorizontalOffset(double offset, double length)
	{
		SetHorizontalOffset(IdealViewportOffset(offset, length, _verticalOffset, _viewportWidth));
	}

	void SetVerticalOffset(double offset)
	{
		offset = ConstrainOffset(offset, _viewportHeight, _extentHeight);

		if (_verticalOffset != offset)
		{
			_verticalOffset = offset;
			InvalidateVisual();
			InvalidateOwner();
		}
	}

	void SetVerticalOffset(double offset, double length)
	{
		SetVerticalOffset(IdealViewportOffset(offset, length, _verticalOffset, _viewportHeight));
	}

	static double ChooseOffset(double position, double length, double offset, double viewport)
	{
		if (position <= offset || length >= viewport)
		{
			return position;
		}

		var endAlignOffset = position + length - viewport;

		if (endAlignOffset > offset)
		{
			return endAlignOffset;
		}
		else
		{
			return offset;
		}
	}

	static double ConstrainOffset(double offset, double viewport, double extent)
	{
		if (offset < 0 || viewport > extent)
		{
			return 0;
		}
		else if (offset + viewport > extent)
		{
			return extent - viewport;
		}
		else
		{
			return offset;
		}
	}

	static double IdealViewportOffset(double selectionOffset, double selectionLength, double currentViewportOffset, double currentViewportLength)
	{
		// Moving the view port too much is dissorientating so:
		// * Avoid it if we can
		// * If must move, try to keep as much common context as possible (only really relevant for small movements)
		// * Try to make the move worth while.
		//
		// Specifically:
		// * Don't move the viewport if atleast part of the selection is in the middle 80%.
		// * If we have to move the view port, fit as much of the selection as possible in the middle 80%.
		// * Move the viewport as little as possible.

		var idealViewportLength = currentViewportLength * 0.8;
		var idealViewportAdjustment = (currentViewportLength - idealViewportLength) / 2;
		var idealViewportOffset = currentViewportOffset + idealViewportAdjustment;

		if (selectionOffset > idealViewportOffset + idealViewportLength)
		{
			if (selectionLength > idealViewportLength)
			{
				// align ideal viewport start with selection start.
				return selectionOffset - idealViewportAdjustment;
			}
			else
			{
				// align ideal viewport end with selection end.
				return selectionOffset + selectionLength - idealViewportLength - idealViewportAdjustment;
			}
		}
		else if (selectionOffset + selectionLength < idealViewportOffset)
		{
			if (selectionLength > idealViewportLength)
			{
				// align ideal viewport end with selection end.
				return selectionOffset + selectionLength - idealViewportLength - idealViewportAdjustment;
			}
			else
			{
				// align ideal viewport start with selection start.
				return selectionOffset - idealViewportAdjustment;
			}
		}

		// current offset should be sufficient.
		return currentViewportOffset;
	}

	void InvalidateOwner() => _owner?.InvalidateScrollInfo();

	readonly HexBox _parent;
	double _extentWidth;
	double _extentHeight;
	double _dataHeight;
	double _dataWidth;
	double _horizontalOffset;
	double _verticalOffset;
	double _viewportHeight;
	double _viewportWidth;
	bool _canVerticallyScroll;
	bool _canHorizontallyScroll;
	HexCharSet? _charSet;
	CharLayout? _layout;
	ScrollViewer? _owner;
	LineSet? _lineSet;

	sealed class LineSet
	{
		public LineSet(long startingLine, GlyphRun[] runs)
		{
			StartingLine = startingLine;
			Runs = runs;
		}

		public long StartingLine { get; }
		public GlyphRun[] Runs { get; }
	}
}
