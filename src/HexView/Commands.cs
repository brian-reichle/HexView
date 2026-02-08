// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Windows.Input;

namespace HexView
{
	static class Commands
	{
		public static readonly RoutedCommand CloseDocument = new(nameof(CloseDocument), typeof(Commands), [new KeyGesture(Key.F4, ModifierKeys.Control)]);
		public static readonly RoutedCommand Goto = new(nameof(Goto), typeof(Commands), [new KeyGesture(Key.G, ModifierKeys.Control)]);
		public static readonly RoutedCommand SelectNode = new(nameof(SelectNode), typeof(Commands));
		public static readonly RoutedCommand ShowSelection = new(nameof(ShowSelection), typeof(Commands));
	}
}
