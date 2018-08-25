// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows.Input;

namespace HexView
{
	static class Commands
	{
		public static readonly RoutedCommand CloseDocument = new RoutedCommand(nameof(CloseDocument), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Control) });
		public static readonly RoutedCommand Goto = new RoutedCommand(nameof(Goto), typeof(Commands), new InputGestureCollection() { new KeyGesture(Key.G, ModifierKeys.Control) });
		public static readonly RoutedCommand SelectNode = new RoutedCommand(nameof(SelectNode), typeof(Commands));
		public static readonly RoutedCommand ShowSelection = new RoutedCommand(nameof(ShowSelection), typeof(Commands));
	}
}
