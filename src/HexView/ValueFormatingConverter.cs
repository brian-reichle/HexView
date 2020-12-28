// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace HexView
{
	[ValueConversion(typeof(object), typeof(string))]
	sealed class ValueFormatingConverter : IValueConverter
	{
		public static ValueFormatingConverter Default { get; } = new ValueFormatingConverter();

		public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) return null;

			var builder = new StringBuilder();
			Format(builder, value);
			return builder.ToString();
		}

		static void Format(StringBuilder builder, object value)
		{
			var valueType = value.GetType();

			if (valueType.IsEnum)
			{
				AppendEnumBreakdown(builder, (IConvertible)value);
			}
			else if (value is IConvertible convertable)
			{
				AppendValue(builder, convertable);
			}
			else
			{
				builder.Append(value.ToString());
			}
		}

		static void AppendEnumBreakdown(StringBuilder builder, IConvertible convertable)
		{
			switch (convertable.GetTypeCode())
			{
				case TypeCode.SByte:
					AppendEnumBreakdown(builder, convertable.GetType(), unchecked((ulong)convertable.ToSByte(null)), "X2", x => unchecked((sbyte)x));
					break;

				case TypeCode.Int16:
					AppendEnumBreakdown(builder, convertable.GetType(), unchecked((ulong)convertable.ToInt16(null)), "X4", x => unchecked((short)x));
					break;

				case TypeCode.Int32:
					AppendEnumBreakdown(builder, convertable.GetType(), unchecked((ulong)convertable.ToInt32(null)), "X8", x => unchecked((long)x));
					break;

				case TypeCode.Int64:
					AppendEnumBreakdown(builder, convertable.GetType(), unchecked((ulong)convertable.ToInt64(null)), "X16", x => unchecked((long)x));
					break;

				case TypeCode.Byte:
					AppendEnumBreakdown(builder, convertable.GetType(), convertable.ToByte(null), "X2", x => (byte)x);
					break;

				case TypeCode.UInt16:
					AppendEnumBreakdown(builder, convertable.GetType(), convertable.ToUInt16(null), "X4", x => (ushort)x);
					break;

				case TypeCode.UInt32:
					AppendEnumBreakdown(builder, convertable.GetType(), convertable.ToUInt32(null), "X8", x => (uint)x);
					break;

				case TypeCode.UInt64:
					AppendEnumBreakdown(builder, convertable.GetType(), convertable.ToUInt64(null), "X16", x => (ulong)x);
					break;
			}
		}

		static void AppendValue(StringBuilder builder, IConvertible convertable)
		{
			switch (convertable.GetTypeCode())
			{
				case TypeCode.SByte:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X2}: {0}\r\n", convertable.ToSByte(null));
					break;

				case TypeCode.Int16:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X4}: {0}\r\n", convertable.ToInt16(null));
					break;

				case TypeCode.Int32:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X8}: {0}\r\n", convertable.ToInt32(null));
					break;

				case TypeCode.Int64:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X16}: {0}\r\n", convertable.ToInt64(null));
					break;

				case TypeCode.Byte:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X2}: {0}\r\n", convertable.ToByte(null));
					break;

				case TypeCode.UInt16:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X4}: {0}\r\n", convertable.ToUInt16(null));
					break;

				case TypeCode.UInt32:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X8}: {0}\r\n", convertable.ToUInt32(null));
					break;

				case TypeCode.UInt64:
					builder.AppendFormat(CultureInfo.InvariantCulture, "0x{0:X16}: {0}\r\n", convertable.ToUInt64(null));
					break;

				case TypeCode.Char:
					AppendCharValue(builder, convertable.ToChar(null));
					break;

				case TypeCode.String:
					AppendStringValue(builder, convertable.ToString(null));
					break;

				case TypeCode.DateTime:
					AppendDateTime(builder, convertable.ToDateTime(null));
					break;

				default:
					builder.AppendFormat(CultureInfo.InvariantCulture, "{0}\r\n", convertable);
					break;
			}
		}

		static void AppendCharValue(StringBuilder builder, char value)
		{
			builder.Append("0x");
			builder.Append(((int)value).ToString("X4", CultureInfo.InvariantCulture));
			builder.Append(": '");
			AppendChar(builder, value, stringChar: false);
			builder.Append('\'');
		}

		static void AppendStringValue(StringBuilder builder, string value)
		{
			builder.Append('"');

			for (var i = 0; i < value.Length; i++)
			{
				AppendChar(builder, value[i], stringChar: true);
			}

			builder.Append('"');
		}

		static void AppendChar(StringBuilder builder, char value, bool stringChar)
		{
			switch (value)
			{
				case '\0': builder.Append("\\0"); break;
				case '\b': builder.Append("\\b"); break;
				case '\a': builder.Append("\\a"); break;
				case '\r': builder.Append("\\r"); break;
				case '\n': builder.Append("\\n"); break;
				case '\v': builder.Append("\\v"); break;
				case '\f': builder.Append("\\f"); break;
				case '\t': builder.Append("\\t"); break;
				case '\\': builder.Append("\\\\"); break;

				case '\'':
					if (stringChar) goto default;
					builder.Append("\\'");
					break;

				case '"':
					if (!stringChar) goto default;
					builder.Append("\\\"");
					break;

				default:
					if (value < 0x20 || value >= 0x7F)
					{
						builder.Append("\\u");
						builder.Append(((int)value).ToString("X4", CultureInfo.InvariantCulture));
					}
					else
					{
						builder.Append(value);
					}
					break;
			}
		}

		static void AppendDateTime(StringBuilder builder, DateTime dateTime)
		{
			builder.AppendFormat(CultureInfo.InvariantCulture, "{0:O}\r\n{1:F}\r\n", dateTime, dateTime.ToLocalTime());
		}

		static void AppendEnumBreakdown(StringBuilder builder, Type enumType, ulong value, string formatString, Func<ulong, object> box)
		{
			if (value == 0 || !IsFlags(enumType))
			{
				var label = value.ToString(formatString, CultureInfo.InvariantCulture);

				if (Enum.IsDefined(enumType, box(value)))
				{
					label = "0x" + label + ": " + Enum.GetName(enumType, box(value));
				}
				else
				{
					label = "0x" + label + ": <unknown value>";
				}

				builder.AppendLine(label);
			}
			else
			{
				var fail = 0UL;

				do
				{
					var tmp = value & ~(value - 1);
					value ^= tmp;

					if (Enum.IsDefined(enumType, box(tmp)))
					{
						var label = "0x" + tmp.ToString(formatString, CultureInfo.InvariantCulture) + ": " + Enum.GetName(enumType, box(tmp));
						builder.AppendLine(label);
					}
					else
					{
						fail |= tmp;
					}
				}
				while (value != 0);

				if (fail != 0)
				{
					var label = "0x" + fail.ToString(formatString, CultureInfo.InvariantCulture) + ": <unknown flags>";
					builder.AppendLine(label);
				}
			}
		}

		static bool IsFlags(Type enumType) => enumType.IsDefined(typeof(FlagsAttribute), inherit: false);

		#region IValueConverter Members

		object IValueConverter.ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();

		#endregion
	}
}
