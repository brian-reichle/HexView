// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	// Date time format specifically used by the PE header. (seconds since 1970-01-01 00:00 or 0).
	sealed class TimestampNodeTemplate : IStructuralNodeTemplate
	{
		public static readonly TimestampNodeTemplate DateTime = new TimestampNodeTemplate();

		TimestampNodeTemplate()
		{
		}

		public long Width => 4;
		public IReadOnlyList<Component> Components => Array.Empty<Component>();

		public object? GetValue(IDataSource data, long offset)
		{
			var value = data.Read<uint>(offset);
			return value == 0 ? null : (object)Epoch.AddSeconds(value);
		}

		static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
