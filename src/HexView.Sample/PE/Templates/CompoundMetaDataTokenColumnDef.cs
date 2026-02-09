// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using HexView.Framework;

namespace HexView.Plugins.Sample.PE;

sealed class CompoundMetaDataTokenColumnDef : MetaDataTokenColumnDef
{
	public static readonly CompoundMetaDataTokenColumnDef TypeDefOrRef = new(
	[
		MetaDataTableCodes.TypeDef,
		MetaDataTableCodes.TypeRef,
		MetaDataTableCodes.TypeSpec,
	]);

	public static readonly CompoundMetaDataTokenColumnDef HasConstant = new(
	[
		MetaDataTableCodes.Field,
		MetaDataTableCodes.Param,
		MetaDataTableCodes.Property,
	]);

	public static readonly CompoundMetaDataTokenColumnDef HasCustomAttribute = new(
	[
		MetaDataTableCodes.MethodDef,
		MetaDataTableCodes.Field,
		MetaDataTableCodes.TypeRef,
		MetaDataTableCodes.TypeDef,
		MetaDataTableCodes.Param,
		MetaDataTableCodes.InterfaceImpl,
		MetaDataTableCodes.MemberRef,
		MetaDataTableCodes.Module,
		(MetaDataTableCodes)0xFF,
		MetaDataTableCodes.Property,
		MetaDataTableCodes.Event,
		MetaDataTableCodes.StandAloneSig,
		MetaDataTableCodes.ModuleRef,
		MetaDataTableCodes.TypeSpec,
		MetaDataTableCodes.Assembly,
		MetaDataTableCodes.AssemblyRef,
		MetaDataTableCodes.File,
		MetaDataTableCodes.ExportedType,
		MetaDataTableCodes.ManifestResource,
	]);

	public static readonly CompoundMetaDataTokenColumnDef HasFieldMarshall = new(
	[
		MetaDataTableCodes.Field,
		MetaDataTableCodes.Param,
	]);

	public static readonly CompoundMetaDataTokenColumnDef HasDeclSecurity = new(
	[
		MetaDataTableCodes.TypeDef,
		MetaDataTableCodes.MethodDef,
		MetaDataTableCodes.Assembly,
	]);

	public static readonly CompoundMetaDataTokenColumnDef MemberRefParent = new(
	[
		MetaDataTableCodes.TypeDef,
		MetaDataTableCodes.TypeRef,
		MetaDataTableCodes.ModuleRef,
		MetaDataTableCodes.MethodDef,
		MetaDataTableCodes.TypeSpec,
	]);

	public static readonly CompoundMetaDataTokenColumnDef HasSemantics = new(
	[
		MetaDataTableCodes.Event,
		MetaDataTableCodes.Property,
	]);

	public static readonly CompoundMetaDataTokenColumnDef MethodDefOrRef = new(
	[
		MetaDataTableCodes.MethodDef,
		MetaDataTableCodes.MemberRef,
	]);

	public static readonly CompoundMetaDataTokenColumnDef MemberForwarded = new(
	[
		MetaDataTableCodes.Field,
		MetaDataTableCodes.MethodDef,
	]);

	public static readonly CompoundMetaDataTokenColumnDef Implementation = new(
	[
		MetaDataTableCodes.File,
		MetaDataTableCodes.AssemblyRef,
		MetaDataTableCodes.ExportedType,
	]);

	public static readonly CompoundMetaDataTokenColumnDef CustomAttributeType = new(
	[
		(MetaDataTableCodes)0xFF,
		(MetaDataTableCodes)0xFF,
		MetaDataTableCodes.MethodDef,
		MetaDataTableCodes.MemberRef,
		(MetaDataTableCodes)0xFF,
	]);

	public static readonly CompoundMetaDataTokenColumnDef ResolutionScope = new(
	[
		MetaDataTableCodes.Module,
		MetaDataTableCodes.ModuleRef,
		MetaDataTableCodes.AssemblyRef,
		MetaDataTableCodes.TypeRef,
	]);

	public static readonly CompoundMetaDataTokenColumnDef TypeOrMethodDef = new(
	[
		MetaDataTableCodes.TypeDef,
		MetaDataTableCodes.MethodDef,
	]);

	public CompoundMetaDataTokenColumnDef(MetaDataTableCodes[] codes)
	{
		_codes = codes;
		_codeBits = RequiredBits(codes.Length);
		_codeMask = (1 << _codeBits) - 1;
	}

	protected override int Decode(int rawValue)
	{
		if (rawValue == 0) return 0;
		return (rawValue >> _codeBits) | ((int)_codes[rawValue & _codeMask] << 24);
	}

	public override IStructuralNodeTemplate SelectTemplate(MetaDataTableStatistics statistics)
	{
		var count = 0;

		for (var i = 0; i < _codes.Length; i++)
		{
			var tmp = statistics.GetRowCount(_codes[i]);

			if (tmp > count)
			{
				count = tmp;
			}
		}

		return (RequiredBits(count) + _codeBits) > 16 ? LongForm : ShortForm;
	}

	readonly int _codeBits;
	readonly int _codeMask;
	readonly MetaDataTableCodes[] _codes;
}
