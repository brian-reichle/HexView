// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView
{
	static class HexHelper
	{
		public static readonly string HexChars = "0123456789ABCDEF";

		public static long Parse(string value)
		{
			if (!TryParse(value, out var result))
			{
				throw new ArgumentException("Invalid value.", nameof(value));
			}

			return result;
		}

		public static bool TryParse(string value, out long result)
		{
			if (value == null || value.Length > 16)
			{
				result = 0L;
				return false;
			}

			var tmp = 0L;

			for (var i = 0; i < value.Length; i++)
			{
				var c = value[i];

				if (c >= '0' && c <= '9')
				{
					tmp = (tmp << 4) | (uint)(c - '0');
				}
				else if (c >= 'A' && c <= 'F')
				{
					tmp = (tmp << 4) | (uint)(c - 'A' + 10);
				}
				else if (c >= 'a' && c <= 'f')
				{
					tmp = (tmp << 4) | (uint)(c - 'a' + 10);
				}
				else
				{
					result = 0;
					return false;
				}
			}

			result = tmp;
			return true;
		}
	}
}
