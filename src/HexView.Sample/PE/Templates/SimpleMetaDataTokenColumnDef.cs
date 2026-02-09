// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using HexView.Framework;

namespace HexView.Plugins.Sample.PE;

sealed class SimpleMetaDataTokenColumnDef : MetaDataTokenColumnDef
{
	public static readonly SimpleMetaDataTokenColumnDef AssemblyRef = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.AssemblyRef);
	public static readonly SimpleMetaDataTokenColumnDef AssemblyRefOS = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.AssemblyRefOS);
	public static readonly SimpleMetaDataTokenColumnDef Event = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.Event);
	public static readonly SimpleMetaDataTokenColumnDef Field = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.Field);
	public static readonly SimpleMetaDataTokenColumnDef FieldLayout = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.FieldLayout);
	public static readonly SimpleMetaDataTokenColumnDef GenericParam = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.GenericParam);
	public static readonly SimpleMetaDataTokenColumnDef MemberRef = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.MemberRef);
	public static readonly SimpleMetaDataTokenColumnDef MethodDef = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.MethodDef);
	public static readonly SimpleMetaDataTokenColumnDef Param = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.Param);
	public static readonly SimpleMetaDataTokenColumnDef TypeDef = new SimpleMetaDataTokenColumnDef(MetaDataTableCodes.TypeDef);

	public SimpleMetaDataTokenColumnDef(MetaDataTableCodes code) => _code = code;

	protected override int Decode(int rawValue)
	{
		if (rawValue == 0) return 0;
		return ((int)_code << 24) | rawValue;
	}

	public override IStructuralNodeTemplate SelectTemplate(MetaDataTableStatistics statistics)
		=> RequiredBits(statistics.GetRowCount(_code)) > 16 ? LongForm : ShortForm;

	readonly MetaDataTableCodes _code;
}
