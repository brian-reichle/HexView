// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace HexView
{
	static class PendingChangeFlush
	{
		public static readonly RoutedEvent FlushRequestEvent = EventManager.RegisterRoutedEvent(
			"FlushRequest",
			RoutingStrategy.Bubble,
			typeof(RoutedEventHandler),
			typeof(PendingChangeFlush));

		static PendingChangeFlush()
		{
			EventManager.RegisterClassHandler(typeof(TextBox), FlushRequestEvent, (RoutedEventHandler)FlushTextBox);
		}

		public static void Flush()
		{
			if (InputManager.Current.PrimaryKeyboardDevice.FocusedElement is FrameworkElement element)
			{
				Flush(element);
			}
		}

		public static void Flush(FrameworkElement element)
		{
			ArgumentNullException.ThrowIfNull(element);
			element.RaiseEvent(new RoutedEventArgs(FlushRequestEvent));
		}

		static void FlushTextBox(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			var binding = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);

			if (binding != null)
			{
				binding.UpdateSource();
			}
		}
	}
}
