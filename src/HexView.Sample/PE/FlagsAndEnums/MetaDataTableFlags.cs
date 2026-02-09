// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace HexView.Plugins.Sample.PE;

[Flags]
enum MetaDataTableFlags : ulong
{
	Assembly = 1UL << MetaDataTableCodes.Assembly,
	AssemblyOS = 1UL << MetaDataTableCodes.AssemblyOS,
	AssemblyProcessor = 1UL << MetaDataTableCodes.AssemblyProcessor,
	AssemblyRef = 1UL << MetaDataTableCodes.AssemblyRef,
	AssemblyRefOS = 1UL << MetaDataTableCodes.AssemblyRefOS,
	AssemblyRefProcessor = 1UL << MetaDataTableCodes.AssemblyRefProcessor,
	ClassLayout = 1UL << MetaDataTableCodes.ClassLayout,
	Constant = 1UL << MetaDataTableCodes.Constant,
	CustomAttribute = 1UL << MetaDataTableCodes.CustomAttribute,
	DeclSecurity = 1UL << MetaDataTableCodes.DeclSecurity,
	EventMap = 1UL << MetaDataTableCodes.EventMap,
	Event = 1UL << MetaDataTableCodes.Event,
	ExportedType = 1UL << MetaDataTableCodes.ExportedType,
	Field = 1UL << MetaDataTableCodes.Field,
	FieldLayout = 1UL << MetaDataTableCodes.FieldLayout,
	FieldMarshal = 1UL << MetaDataTableCodes.FieldMarshal,
	FieldRVA = 1UL << MetaDataTableCodes.FieldRVA,
	File = 1UL << MetaDataTableCodes.File,
	GenericParam = 1UL << MetaDataTableCodes.GenericParam,
	GenericParamConstraint = 1UL << MetaDataTableCodes.GenericParamConstraint,
	ImplMap = 1UL << MetaDataTableCodes.ImplMap,
	InterfaceImpl = 1UL << MetaDataTableCodes.InterfaceImpl,
	ManifestResource = 1UL << MetaDataTableCodes.ManifestResource,
	MemberRef = 1UL << MetaDataTableCodes.MemberRef,
	MethodDef = 1UL << MetaDataTableCodes.MethodDef,
	MethodImpl = 1UL << MetaDataTableCodes.MethodImpl,
	MethodSemantics = 1UL << MetaDataTableCodes.MethodSemantics,
	MethodSpec = 1UL << MetaDataTableCodes.MethodSpec,
	Module = 1UL << MetaDataTableCodes.Module,
	ModuleRef = 1UL << MetaDataTableCodes.ModuleRef,
	NestedClass = 1UL << MetaDataTableCodes.NestedClass,
	Param = 1UL << MetaDataTableCodes.Param,
	Property = 1UL << MetaDataTableCodes.Property,
	PropertyMap = 1UL << MetaDataTableCodes.PropertyMap,
	StandAloneSig = 1UL << MetaDataTableCodes.StandAloneSig,
	TypeDef = 1UL << MetaDataTableCodes.TypeDef,
	TypeRef = 1UL << MetaDataTableCodes.TypeRef,
	TypeSpec = 1UL << MetaDataTableCodes.TypeSpec,
}
