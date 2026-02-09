// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace HexView;

sealed class GotoDialog : INotifyPropertyChanged, IDataErrorInfo
{
	public long Offset
	{
		get => _offset;
		set
		{
			_offset = value;
			OnPropertyChanged();
		}
	}

	public long MaxOffset
	{
		get => _maxOffset;
		set
		{
			_maxOffset = value;
			OnPropertyChanged();
		}
	}

	public bool? ShowDialog(Window window)
	{
		var view = new GotoDialogView();
		view.DataContext = this;
		view.Owner = window;
		return view.ShowDialog();
	}

	#region INotifyPropertyChanged Members

	event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
	{
		add => PropertyChanged += value;
		remove => PropertyChanged -= value;
	}

	#endregion

	#region IDataErrorInfo Members

	string IDataErrorInfo.Error => string.Empty;

	string IDataErrorInfo.this[string columnName]
	{
		get
		{
			switch (columnName)
			{
				case nameof(Offset):
					if (Offset > MaxOffset)
					{
						return "Offset out of range.";
					}
					break;
			}

			return string.Empty;
		}
	}

	#endregion

	void OnPropertyChanged([CallerMemberName] string propertyName = null!)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	long _offset;
	long _maxOffset;
	event PropertyChangedEventHandler? PropertyChanged;
}
