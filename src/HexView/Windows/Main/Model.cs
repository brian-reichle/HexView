// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.ComponentModel;
using System.Diagnostics;
using HexView.Data;
using HexView.Framework;

namespace HexView
{
	sealed class Model : INotifyPropertyChanged
	{
		public DataSource Buffer
		{
			[DebuggerStepThrough]
			get => _buffer;
			set
			{
				_buffer = value;
				_reader = NullFormatReader.Instance;
				Provider = null;
				OnPropertyChanged(nameof(Buffer));
				OnPropertyChanged(nameof(Reader));
				OnPropertyChanged(nameof(Provider));
			}
		}

		public IFormatReader Reader
		{
			[DebuggerStepThrough]
			get => _reader;
			set
			{
				if (_reader != value)
				{
					_reader = value;
					Provider = _reader.Read(_buffer);
					OnPropertyChanged(nameof(Reader));
					OnPropertyChanged(nameof(Provider));
				}
			}
		}

		public IStructuralNodeProvider Provider { get; private set; }

		public event PropertyChangedEventHandler PropertyChanged;

		void OnPropertyChanged(string name)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		DataSource _buffer = DataSource.Empty;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IFormatReader _reader = NullFormatReader.Instance;
	}
}
