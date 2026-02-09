// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HexView.Framework;

namespace HexView;

sealed partial class ShowDialog : Window
{
	static ShowDialog()
	{
		CommandManager.RegisterClassCommandBinding(typeof(ShowDialog), new(ApplicationCommands.Close, OnCloseExecuted));
	}

	public ShowDialog(IDataSource source, long selectionStart, long selectionLength)
	{
		InitializeComponent();

		var encodings = Encoding.GetEncodings();
		Array.Sort(encodings, (x, y) => string.CompareOrdinal(x.DisplayName, y.DisplayName));

		EncodingDropDown.ItemsSource = encodings;
		EncodingDropDown.SelectedItem = encodings.FirstOrDefault(x => x.Name == "utf-16");
		EncodingDropDown.SelectionChanged += EncodingChanged;

		_source = source;
		_selectionStart = selectionStart;
		_selectionLength = selectionLength;

		Generate();
	}

	void EncodingChanged(object sender, SelectionChangedEventArgs e)
	{
		Generate();
	}

	static void OnCloseExecuted(object sender, ExecutedRoutedEventArgs e)
	{
		((Window)sender).Close();
	}

	void Generate()
	{
		Encoding? encoding;

		if (_source == null)
		{
			SetErrorText("No Data.");
		}
		else if (_selectionLength > 0x1000000)
		{
			SetErrorText("Selected data too long.");
		}
		else if (_selectionLength == 0)
		{
			SetErrorText("Empty Selection.");
		}
		else if ((encoding = ((EncodingInfo?)EncodingDropDown.SelectedItem)?.GetEncoding()) == null)
		{
			SetErrorText("Invalid encoding.");
		}
		else
		{
			SetErrorText(null);
			ContentTextBox.Text = _source.ReadText(_selectionStart, (int)_selectionLength, encoding);
		}
	}

	void SetErrorText(string? text)
	{
		if (string.IsNullOrEmpty(text))
		{
			ErrorLabel.Visibility = Visibility.Collapsed;
			ErrorLabel.Content = null;
			ContentTextBox.IsEnabled = true;
		}
		else
		{
			ErrorLabel.Content = text;
			ErrorLabel.Visibility = Visibility.Visible;
			ContentTextBox.IsEnabled = false;
		}
	}

	readonly IDataSource _source;
	readonly long _selectionStart;
	readonly long _selectionLength;
}
