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
				_reader = null;
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

					if (_reader != null && _buffer != null)
					{
						Provider = _reader.Read(_buffer);
					}
					else
					{
						Provider = null;
					}

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
		DataSource _buffer;
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IFormatReader _reader = NullFormatReader.Instance;
	}
}
