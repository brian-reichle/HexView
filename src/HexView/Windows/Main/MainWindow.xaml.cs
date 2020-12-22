// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using HexView.Data;
using HexView.Framework;
using Microsoft.Win32;
using Range = HexView.Framework.Range;

namespace HexView
{
	public partial class MainWindow : Window
	{
		static MainWindow()
		{
			CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(ApplicationCommands.Close, CloseWindow));
			CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(ApplicationCommands.Open, OpenFile));
			CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Commands.CloseDocument, CloseFile, CanCloseFile));
			CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Commands.Goto, GotoOffset));
			CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Commands.SelectNode, SelectNode));
			CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(Commands.ShowSelection, ShowSelection));
		}

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			HexControl.Focus();
			base.OnInitialized(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);

			if (!e.Handled && e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				if (e.Data.GetData(DataFormats.FileDrop) is string[] filenames &&
					filenames.Length == 1 &&
					!string.IsNullOrEmpty(filenames[0]))
				{
					e.Effects = e.AllowedEffects & DragDropEffects.Copy;
				}
				else
				{
					e.Effects = DragDropEffects.None;
				}

				e.Handled = true;
			}
		}

		protected override void OnDrop(DragEventArgs e)
		{
			base.OnDrop(e);

			if (!e.Handled && e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Handled = true;

				if (e.Data.GetData(DataFormats.FileDrop) is string[] filenames &&
					filenames.Length == 1 &&
					!string.IsNullOrEmpty(filenames[0]) &&
					File.Exists(filenames[0]))
				{
					OpenFile(filenames[0]);
				}
			}
		}

		static void CloseWindow(object sender, ExecutedRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				var window = (MainWindow)sender;
				window.Close();
			}
		}

		static void OpenFile(object sender, ExecutedRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var window = (MainWindow)sender;

				var dialog = new OpenFileDialog();
				dialog.CheckFileExists = true;
				dialog.CheckPathExists = true;
				dialog.Multiselect = false;
				dialog.AddExtension = false;

				if (dialog.ShowDialog(window).GetValueOrDefault())
				{
					window.OpenFile(dialog.FileName);
				}
			}
		}

		static void CloseFile(object sender, ExecutedRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var window = (MainWindow)sender;
				window.CloseFile();
			}
		}

		static void GotoOffset(object sender, ExecutedRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				var window = (MainWindow)sender;
				var buffer = window.Model.Buffer;

				var dialog = new GotoDialog();
				dialog.MaxOffset = buffer == null ? 0L : buffer.ByteCount;

				if (dialog.ShowDialog(window).GetValueOrDefault())
				{
					var control = window.HexControl;
					control.Select(dialog.Offset, 1);
					control.JumpToOffset(dialog.Offset, 0);
				}
			}
		}

		static void SelectNode(object sender, ExecutedRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var node = (IStructuralNode?)e.Parameter;
				var window = (MainWindow)sender;
				window.SelectRange(node?.ByteRange);
			}
		}

		static void ShowSelection(object sender, ExecutedRoutedEventArgs e)
		{
			var window = (MainWindow)sender;
			var control = window.HexControl;

			var dialog = new ShowDialog(control.Data, control.SelectionStart, control.SelectionLength);
			dialog.Owner = window;
			dialog.ShowDialog();
		}

		static void CanCloseFile(object sender, CanExecuteRoutedEventArgs e)
		{
			if (!e.Handled)
			{
				e.Handled = true;

				var window = (MainWindow)sender;
				var buffer = window.Model.Buffer;
				e.CanExecute = buffer != null;
			}
		}

		void OpenFile(string filename)
		{
			DataSource newSource;

			try
			{
				newSource = DataSource.Load(filename);
			}
			catch (IOException ex)
			{
				MessageBox.Show(ex.Message, "Error Opening File");
				return;
			}

			using var oldSource = Model.Buffer;
			Model.Buffer = newSource;
		}

		void CloseFile()
		{
			var old = Model.Buffer;

			if (old != null)
			{
				try
				{
					Model.Buffer = DataSource.Empty;
				}
				finally
				{
					old.Dispose();
				}
			}
		}

		void SelectRange(Range? range)
		{
			if (range == null)
			{
				HexControl.Select(0, 0);
			}
			else
			{
				HexControl.Select(range.Offset, range.Length);
				HexControl.JumpToOffset(range.Offset, range.Length);
			}
		}

		Model Model => (Model)DataContext;
	}
}
