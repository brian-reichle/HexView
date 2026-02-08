// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HexView
{
	sealed class OverviewControl : Control
	{
		public static readonly DependencyProperty DataLengthProperty = DependencyProperty.Register(
			nameof(DataLength),
			typeof(long),
			typeof(OverviewControl),
			new FrameworkPropertyMetadata(
				0L,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnDataLengthChanged));

		public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
			nameof(SelectionStart),
			typeof(long),
			typeof(OverviewControl),
			new FrameworkPropertyMetadata(
				0L,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnSelectionRangeChanged));

		public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register(
			nameof(SelectionLength),
			typeof(long),
			typeof(OverviewControl),
			new FrameworkPropertyMetadata(
				0L,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnSelectionRangeChanged));

		public static readonly DependencyProperty DataBrushProperty = DependencyProperty.Register(
			nameof(DataBrush),
			typeof(Brush),
			typeof(OverviewControl),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
			nameof(SelectionBrush),
			typeof(Brush),
			typeof(OverviewControl),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public static readonly DependencyProperty SelectionOpacityProperty = DependencyProperty.Register(
			nameof(SelectionOpacity),
			typeof(double),
			typeof(OverviewControl),
			new FrameworkPropertyMetadata(0.25d, FrameworkPropertyMetadataOptions.AffectsRender));

		public long DataLength
		{
			get => (long)GetValue(DataLengthProperty);
			set => SetValue(DataLengthProperty, value);
		}

		public long SelectionStart
		{
			get => (long)GetValue(SelectionStartProperty);
			set => SetValue(SelectionStartProperty, value);
		}

		public long SelectionLength
		{
			get => (long)GetValue(SelectionLengthProperty);
			set => SetValue(SelectionLengthProperty, value);
		}

		public Brush? DataBrush
		{
			get => (Brush?)GetValue(DataBrushProperty);
			set => SetValue(DataBrushProperty, value);
		}

		public Brush? SelectionBrush
		{
			get => (Brush?)GetValue(SelectionBrushProperty);
			set => SetValue(SelectionBrushProperty, value);
		}

		public double SelectionOpacity
		{
			get => (double)GetValue(SelectionOpacityProperty);
			set => SetValue(SelectionOpacityProperty, value);
		}

		protected override Size MeasureOverride(Size availableSize) => new Size(20, 0);

		protected override Size ArrangeOverride(Size finalSize)
		{
			var height = finalSize.Height;

			if (double.IsInfinity(height))
			{
				height = 0;
			}

			var geometry = DataGeometry;

			if (geometry == null)
			{
				_renderTransform = Transform.Identity;
			}
			else
			{
				var boundingBox = geometry.Bounds;
				_renderTransform = new ScaleTransform(1, height / boundingBox.Height);
			}

			return new Size(20, height);
		}

		protected override void OnRender(DrawingContext drawingContext)
		{
			drawingContext.PushGuidelineSet(new GuidelineSet(new[] { 0d, ActualWidth }, new[] { 0d, ActualHeight }));
			drawingContext.DrawRectangle(Background, null, new Rect(0, 0, ActualWidth, ActualHeight));
			drawingContext.PushTransform(new TranslateTransform(2, 0));
			drawingContext.PushTransform(_renderTransform);

			var dataGeometry = DataGeometry;
			var selectionGeometry = SelectionGeometry;

			if (dataGeometry != null)
			{
				drawingContext.DrawGeometry(DataBrush, null, dataGeometry);
			}

			if (selectionGeometry != null)
			{
				drawingContext.PushOpacity(SelectionOpacity);
				drawingContext.DrawGeometry(SelectionBrush, null, selectionGeometry);
				drawingContext.Pop();
			}

			drawingContext.Pop();
			drawingContext.Pop();
			drawingContext.Pop();
		}

		static Geometry? CreateSelectionGeometryFromByteRange(long start, long end)
		{
			var firstLine = start >> 4;
			var lastLine = end >> 4;

			if (firstLine == lastLine)
			{
				return new RectangleGeometry(CreateSelectionRectangleFromCells(firstLine, start & 0xF, lastLine, end & 0xF));
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
				lead = new RectangleGeometry(CreateSelectionRectangleFromCells(firstLine, start & 0xF, firstLine, 0xF));
				firstLine++;
			}

			if ((end & 0xF) == 0xF)
			{
				trail = null;
			}
			else
			{
				trail = new RectangleGeometry(CreateSelectionRectangleFromCells(lastLine, 0, lastLine, end & 0xF));
				lastLine--;
			}

			if (lastLine < firstLine)
			{
				if (lead == null) return trail;
				if (trail == null) return lead;
				return new CombinedGeometry(GeometryCombineMode.Union, lead, trail);
			}

			body = new RectangleGeometry(CreateSelectionRectangleFromCells(firstLine, 0, lastLine, 0xF));

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

		static Rect CreateSelectionRectangleFromCells(long firstRow, long firstColumn, long lastRow, long lastColumn)
		{
			return new Rect(
				firstColumn,
				firstRow,
				lastColumn - firstColumn + 1,
				lastRow - firstRow + 1);
		}

		static void OnDataLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (OverviewControl)d;
			control._dirtyDataGeometry = true;
			control._dirtySelectionGeomertry = true;
		}

		static void OnSelectionRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var control = (OverviewControl)d;
			control._dirtySelectionGeomertry = true;
		}

		Geometry? DataGeometry
		{
			get
			{
				if (_dirtyDataGeometry)
				{
					var dataLength = DataLength;
					_dirtyDataGeometry = false;
					_dataGeometry = dataLength == 0 ? null : CreateSelectionGeometryFromByteRange(0, dataLength);
				}

				return _dataGeometry;
			}
		}

		Geometry? SelectionGeometry
		{
			get
			{
				if (_dirtySelectionGeomertry)
				{
					var dataLength = DataLength;
					var selectionStart = SelectionStart;
					var selectionLength = SelectionLength;
					_dirtySelectionGeomertry = false;

					if (selectionLength == 0 || selectionStart >= dataLength)
					{
						_selectionGeometry = null;
					}
					else
					{
						var selectionEnd = Math.Min(selectionStart + selectionLength, dataLength) - 1;
						_selectionGeometry = CreateSelectionGeometryFromByteRange(selectionStart, selectionEnd);
					}
				}

				return _selectionGeometry;
			}
		}

		Transform? _renderTransform;
		Geometry? _dataGeometry;
		Geometry? _selectionGeometry;
		bool _dirtyDataGeometry;
		bool _dirtySelectionGeomertry;
	}
}
