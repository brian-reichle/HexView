// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using HexView.Framework;

namespace HexView.Plugins.Sample
{
	static class Helper
	{
		public static string ReadASCIIStringNode(IDataSource data, long offset, int length)
		{
			var buffer = new char[length];
			var i = 0;

			for (; i < buffer.Length; i++)
			{
				var c = (char)data.Read<byte>(offset + i);

				if (c == '\0') break;

				buffer[i] = c;
			}

			return new string(buffer, 0, i);
		}
	}
}
