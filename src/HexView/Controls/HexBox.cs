// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using HexView.Framework;

namespace HexView
{
	[TemplatePart(Name = ScrollViewerPART, Type = typeof(ScrollViewer))]
	class HexBox : Control
	{
		public const string ScrollViewerPART = "PART_ScrollViewer";

		public static readonly DependencyProperty DataProperty = DependencyProperty.Register(
			nameof(Data),
			typeof(IDataSource),
			typeof(HexBox),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
				OnDataChanged));

		public static readonly DependencyProperty DataLengthProperty = DependencyProperty.Register(
			nameof(DataLength),
			typeof(long),
			typeof(HexBox),
			new PropertyMetadata(0L));

		public static readonly DependencyProperty SelectionBrushProperty = DependencyProperty.Register(
			nameof(SelectionBrush),
			typeof(Brush),
			typeof(HexBox),
			new FrameworkPropertyMetadata(
				Brushes.Blue,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnSelectionChanged));

		public static readonly DependencyProperty SelectionOpacityProperty = DependencyProperty.Register(
			nameof(SelectionOpacity),
			typeof(double),
			typeof(HexBox),
			new FrameworkPropertyMetadata(
				0.25d,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnSelectionChanged));

		public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register(
			nameof(SelectionStart),
			typeof(long),
			typeof(HexBox),
			new FrameworkPropertyMetadata(
				0L,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnSelectionChanged));

		public static readonly DependencyProperty SelectionLengthProperty = DependencyProperty.Register(
			nameof(SelectionLength),
			typeof(long),
			typeof(HexBox),
			new FrameworkPropertyMetadata(
				0L,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnSelectionChanged));

		public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty = ScrollViewer.HorizontalScrollBarVisibilityProperty.AddOwner(typeof(HexBox), new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));
		public static readonly DependencyProperty VerticalScrollBarVisibilityProperty = ScrollViewer.VerticalScrollBarVisibilityProperty.AddOwner(typeof(HexBox), new FrameworkPropertyMetadata(ScrollBarVisibility.Hidden));

		static HexBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(typeof(HexBox)));

			FontFamilyProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(OnTypographyChanged));
			FontStyleProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(OnTypographyChanged));
			FontWeightProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(OnTypographyChanged));
			FontStretchProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(OnTypographyChanged));
			FontSizeProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(OnTypographyChanged));
			ForegroundProperty.OverrideMetadata(typeof(HexBox), new FrameworkPropertyMetadata(OnTypographyChanged));
		}

		public HexBox()
		{
			_autoScrollTick = new DispatcherTimer(TimeSpan.FromMilliseconds(50), DispatcherPriority.Background, AutoScrollTick, Dispatcher);
		}

		public IDataSource Data
		{
			get => (IDataSource)GetValue(DataProperty);
			set => SetValue(DataProperty, value);
		}

		public long DataLength
		{
			get => (long)GetValue(DataLengthProperty);
			set => SetValue(DataLengthProperty, value);
		}

		public Brush SelectionBrush
		{
			get => (Brush)GetValue(SelectionBrushProperty);
			set => SetValue(SelectionBrushProperty, value);
		}

		public double SelectionOpacity
		{
			get => (double)GetValue(SelectionOpacityProperty);
			set => SetValue(SelectionOpacityProperty, value);
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

		public ScrollViewer HorizontalScrollBarVisibility
		{
			get => (ScrollViewer)GetValue(HorizontalScrollBarVisibilityProperty);
			set => SetValue(HorizontalScrollBarVisibilityProperty, value);
		}

		public ScrollViewer VerticalScrollBarVisibility
		{
			get => (ScrollViewer)GetValue(VerticalScrollBarVisibilityProperty);
			set => SetValue(VerticalScrollBarVisibilityProperty, value);
		}

		public void Select(long selectionStart, long selectionLength)
		{
			SelectionStart = selectionStart;
			SelectionLength = selectionLength;
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				Focus();

				if (_view != null && !IsMouseCaptured)
				{
					_dragFrom = _view.GetExtentPoint(e.MouseDevice);

					if (!CaptureMouse())
					{
						_dragFrom = new Point();
					}
					else
					{
						_autoScrollTick.Start();
					}
				}
			}

			base.OnMouseLeftButtonDown(e);
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			if (!e.Handled && IsMouseCaptured)
			{
				e.Handled = true;

				if (_view != null)
				{
					var dragFrom = _dragFrom;
					var dragTo = _view.GetExtentPoint(e.MouseDevice);

					_dragFrom = new Point();
					ReleaseMouseCapture();

					SelectByDrag(dragFrom, dragTo);
				}

				_autoScrollTick.Stop();
			}

			base.OnMouseLeftButtonUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (!e.Handled && IsMouseCaptured)
			{
				e.Handled = true;

				var dragFrom = _dragFrom;
				var dragTo = _view.GetExtentPoint(e.MouseDevice);

				SelectByDrag(dragFrom, dragTo);
			}

			base.OnMouseMove(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (!e.Handled && _view != null && Data != null)
			{
				switch (e.Key)
				{
					case Key.Home:
						if (GetModifierKeys(e) == ModifierKeys.Control)
						{
							_view.ScrollToTop();
							e.Handled = true;
						}
						break;

					case Key.End:
						if (GetModifierKeys(e) == ModifierKeys.Control)
						{
							_view.ScrollToEnd();
							e.Handled = true;
						}
						break;

					case Key.PageUp:
						if (GetModifierKeys(e) == ModifierKeys.None)
						{
							_view.PageUp();
							e.Handled = true;
						}
						break;

					case Key.PageDown:
						if (GetModifierKeys(e) == ModifierKeys.None)
						{
							_view.PageDown();
							e.Handled = true;
						}
						break;

					case Key.Up:
						if (GetModifierKeys(e) == ModifierKeys.Control)
						{
							_view.LineUp();
							e.Handled = true;
						}
						break;

					case Key.Down:
						if (GetModifierKeys(e) == ModifierKeys.Control)
						{
							_view.LineDown();
							e.Handled = true;
						}
						break;

					case Key.Escape:
						if (GetModifierKeys(e) == ModifierKeys.None && IsMouseCaptured)
						{
							_dragFrom = new Point();
							ReleaseMouseCapture();
							Select(0, 0);
						}
						break;
				}
			}

			base.OnKeyDown(e);
		}

		public void JumpToOffset(long offset, long length) => _view?.JumpToOffset(offset, length);

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (GetTemplateChild(ScrollViewerPART) is ScrollViewer viewer)
			{
				viewer.CanContentScroll = true;
				viewer.Content = _view = new HexBoxView(this);
			}
		}

		void SelectByDrag(Point fromPoint, Point toPoint)
		{
			var blob = Data;

			if (blob == null || blob.ByteCount == 0)
			{
				return; // selection will already have the only value it can.
			}

			var range = _view.RangeFromDrag(fromPoint, toPoint);

			if (range == null || range.Length <= 0 || range.Offset >= blob.ByteCount)
			{
				Select(0, 0);
				return;
			}

			var selectFrom = range.Offset;
			var selectTo = range.Offset + range.Length - 1;

			if (selectTo >= blob.ByteCount)
			{
				selectTo = blob.ByteCount - 1;
			}

			Select(selectFrom, selectTo - selectFrom + 1);
		}

		void AutoScrollTick(object sender, EventArgs e)
		{
			if (!IsMouseCaptured || _view == null)
			{
				_autoScrollTick.Stop();
				return;
			}

			var device = InputManager.Current.PrimaryMouseDevice;
			var point = device.GetPosition(this);

			if (point.Y < 0)
			{
				_view.MouseWheelUp();
			}
			else if (point.Y > ActualHeight)
			{
				_view.MouseWheelDown();
			}

			if (point.X < 0)
			{
				_view.MouseWheelLeft();
			}
			else if (point.X > ActualWidth)
			{
				_view.MouseWheelRight();
			}

			var dragTo = _view.GetExtentPoint(device);
			var dragFrom = _dragFrom;
			SelectByDrag(dragFrom, dragTo);
		}

		static void OnTypographyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HexBox)d)._view?.InvalidateTypography();
		}

		static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var box = (HexBox)d;
			var view = box._view;
			var data = (IDataSource)e.NewValue;

			box.DataLength = data == null ? 0 : data.ByteCount;
			box.Select(0, 0);

			view?.InvalidateData();
		}

		static void OnSelectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((HexBox)d)._view?.InvalidateVisual();
		}

		static ModifierKeys GetModifierKeys(KeyEventArgs e)
		{
			var keyboard = e.KeyboardDevice;
			return keyboard == null ? ModifierKeys.None : keyboard.Modifiers;
		}

		HexBoxView _view;
		Point _dragFrom;
		DispatcherTimer _autoScrollTick;
	}
}
