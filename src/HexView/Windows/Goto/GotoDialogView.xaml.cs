// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace HexView;

sealed partial class GotoDialogView : Window
{
	public GotoDialogView()
	{
		InitializeComponent();
	}

	protected override void OnInitialized(EventArgs e)
	{
		base.OnInitialized(e);
		PositionTextBox.Focus();

		Dispatcher.BeginInvoke(PositionTextBox.SelectAll, DispatcherPriority.Loaded);
	}

	void OkClick(object sender, RoutedEventArgs e)
	{
		PendingChangeFlush.Flush();

		if (!Validation.GetHasError(PositionTextBox))
		{
			DialogResult = true;
			Close();
		}
	}

	void CancelClick(object sender, RoutedEventArgs e)
	{
		DialogResult = false;
		Close();
	}
}
