// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	abstract class MetaDataTokenColumnDef
	{
		public IStructuralNodeTemplate ShortForm => _shortForm ??= new ShortFormTemplate(this);
		public IStructuralNodeTemplate LongForm => _longForm ??= new LongFormTemplate(this);

		public abstract IStructuralNodeTemplate SelectTemplate(MetaDataTableStatistics statistics);
		protected abstract int Decode(int rawValue);

		protected static int RequiredBits(int possibleValues)
		{
			if (possibleValues == 0) return 0;

			var count = 0;
			possibleValues--;

			while (possibleValues > 0)
			{
				count++;
				possibleValues >>= 1;
			}

			return count;
		}

		IStructuralNodeTemplate? _shortForm;
		IStructuralNodeTemplate? _longForm;

		sealed class LongFormTemplate : IStructuralNodeTemplate
		{
			public LongFormTemplate(MetaDataTokenColumnDef def) => _def = def;

			long IStructuralNodeTemplate.Width => 4;
			object IStructuralNodeTemplate.GetValue(IDataSource data, long offset) => _def.Decode(data.Read<int>(offset));
			IReadOnlyList<Component> IStructuralNodeTemplate.Components => Array.Empty<Component>();

			readonly MetaDataTokenColumnDef _def;
		}

		sealed class ShortFormTemplate : IStructuralNodeTemplate
		{
			public ShortFormTemplate(MetaDataTokenColumnDef def) => _def = def;

			long IStructuralNodeTemplate.Width => 2;
			object IStructuralNodeTemplate.GetValue(IDataSource data, long offset) => _def.Decode(data.Read<ushort>(offset));
			IReadOnlyList<Component> IStructuralNodeTemplate.Components => Array.Empty<Component>();

			readonly MetaDataTokenColumnDef _def;
		}
	}
}
