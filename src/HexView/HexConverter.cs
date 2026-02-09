// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Windows.Data;

namespace HexView;

[ValueConversion(typeof(long?), typeof(string))]
sealed class HexConverter : IValueConverter
{
	public static HexConverter Default { get; } = new HexConverter();

	public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
	{
		var longValue = (long?)value;

		if (longValue.HasValue)
		{
			return longValue.Value.ToString("X", CultureInfo.InvariantCulture);
		}

		return null;
	}

	public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
	{
		if (value != null)
		{
			return HexHelper.Parse((string)value);
		}

		return null;
	}
}
