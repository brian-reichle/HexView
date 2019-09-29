// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Globalization;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	sealed class TableStreamNode : LazyStructuralNode
	{
		public TableStreamNode(PEStructuralNodeProvider provider, IStructuralNode parent, string name, long offset, long length)
			: base(parent)
		{
			_provider = provider;
			Name = name;
			ByteRange = new Range(offset, length);
		}

		public override string Name { get; }
		public override Range ByteRange { get; }

		protected override IList<IStructuralNode> CreateChildNodes()
		{
			var valid = _provider.Data.Read<MetaDataTableFlags>(ByteRange.Offset + 8);
			var heapSizes = _provider.Data.Read<HeapSizeFlags>(ByteRange.Offset + 6);
			var tableCodes = GetCodes(valid);

			var rowCountsTemplate = CreateRowHeader(tableCodes);
			var tableLestTemplate = CreateTableListNode(tableCodes, heapSizes);

			return new IStructuralNode[]
			{
				new TemplatedStructuralNode(_provider.Data, this, "Table Header", PETemplates.TableStreamHeader, ByteRange.Offset),
				new TemplatedStructuralNode(_provider.Data, this, "Row Counts", rowCountsTemplate, ByteRange.Offset + PETemplates.TableStreamHeader.Width),
				new TemplatedStructuralNode(_provider.Data, this, "Tables", tableLestTemplate, ByteRange.Offset + PETemplates.TableStreamHeader.Width + rowCountsTemplate.Width),
			};
		}

		IStructuralNodeTemplate CreateTableListNode(MetaDataTableCodes[] codes, HeapSizeFlags heapSizes)
		{
			var tables = new CompoundNodeTemplate();
			var statistics = new MetaDataTableStatistics(GetRowCounts(codes), heapSizes);

			for (var i = 0; i < codes.Length; i++)
			{
				var code = codes[i];
				var rowCount = statistics.GetRowCount(code);

				if (rowCount <= 0) continue;

				var rowTemplate = CreateRowTemplate(statistics, code);

				if (rowTemplate == null)
				{
					tables.Add(code + " Not Yet Implemented", StandardTemplates.UInt8);
					return tables;
				}

				tables.Add(code.ToString(), new RepeatingNodeTemplate(rowTemplate, rowCount, GetMetaDataTokenFormatter(code)));
			}

			return tables;
		}

		static IStructuralNodeTemplate CreateRowTemplate(MetaDataTableStatistics statistics, MetaDataTableCodes code)
		{
			return code switch
			{
				MetaDataTableCodes.Module => CreateRowNode_Module(statistics),
				MetaDataTableCodes.TypeRef => CreateRowNode_TypeRef(statistics),
				MetaDataTableCodes.TypeDef => CreateRowNode_TypeDef(statistics),
				MetaDataTableCodes.Field => CreateRowNode_FieldDef(statistics),
				MetaDataTableCodes.MethodDef => CreateRowNode_MethodDef(statistics),
				MetaDataTableCodes.Param => CreateRowNode_Param(statistics),
				MetaDataTableCodes.InterfaceImpl => CreateRowNode_InterfaceImpl(statistics),
				MetaDataTableCodes.MemberRef => CreateRowNode_MemberRef(statistics),
				MetaDataTableCodes.Constant => CreateRowNode_Constant(statistics),
				MetaDataTableCodes.CustomAttribute => CreateRowNode_CustomAttribute(statistics),
				MetaDataTableCodes.FieldMarshal => CreateRowNode_FieldMarshal(statistics),
				MetaDataTableCodes.DeclSecurity => CreateRowNode_DeclSecurity(statistics),
				MetaDataTableCodes.ClassLayout => CreateRowNode_ClassLayout(statistics),
				MetaDataTableCodes.FieldLayout => CreateRowNode_FieldLayout(statistics),
				MetaDataTableCodes.StandAloneSig => CreateRowNode_StandAloneSig(statistics),
				MetaDataTableCodes.EventMap => CreateRowNode_EventMap(statistics),
				MetaDataTableCodes.Event => CreateRowNode_Event(statistics),
				MetaDataTableCodes.PropertyMap => CreateRowNode_PropertyMap(statistics),
				MetaDataTableCodes.Property => CreateRowNode_Property(statistics),
				MetaDataTableCodes.MethodSemantics => CreateRowNode_MethodSemantics(statistics),
				MetaDataTableCodes.MethodImpl => CreateRowNode_MethodImpl(statistics),
				MetaDataTableCodes.ModuleRef => CreateRowNode_ModuleRef(statistics),
				MetaDataTableCodes.TypeSpec => CreateRowNode_TypeSpec(statistics),
				MetaDataTableCodes.ImplMap => CreateRowNode_ImplMap(statistics),
				MetaDataTableCodes.FieldRVA => CreateRowNode_FieldRVA(statistics),
				MetaDataTableCodes.Assembly => CreateRowNode_Assembly(statistics),
				MetaDataTableCodes.AssemblyProcessor => RowNode_AssemblyProcessor,
				MetaDataTableCodes.AssemblyOS => RowNode_AssemblyOS,
				MetaDataTableCodes.AssemblyRef => CreateRowNode_AssemblyRef(statistics),
				MetaDataTableCodes.AssemblyRefProcessor => CreateRowNode_AssemblyRefProcessor(statistics),
				MetaDataTableCodes.AssemblyRefOS => CreateRowNode_AssemblyRefOS(statistics),
				MetaDataTableCodes.File => CreateRowNode_File(statistics),
				MetaDataTableCodes.ExportedType => CreateRowNode_ExportedType(statistics),
				MetaDataTableCodes.ManifestResource => CreateRowNode_ManifestResource(statistics),
				MetaDataTableCodes.NestedClass => CreateRowNode_NestedClass(statistics),
				MetaDataTableCodes.GenericParam => CreateRowNode_GenericParam(statistics),
				MetaDataTableCodes.MethodSpec => CreateRowNode_MethodSpec(statistics),
				MetaDataTableCodes.GenericParamConstraint => CreateRowNode_GenericParamConstraint(statistics),
				_ => null,
			};
		}

		static IStructuralNodeTemplate CreateRowNode_Module(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Generation", StandardTemplates.UInt16 },
				{ "Name", GetStringToken(statistics) },
				{ "Mvid", GetGuidToken(statistics) },
				{ "EncId", GetGuidToken(statistics) },
				{ "EncBaseId", GetGuidToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_TypeRef(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "ResolutionScope", CompoundMetaDataTokenColumnDef.ResolutionScope.SelectTemplate(statistics) },
				{ "Name", GetStringToken(statistics) },
				{ "Namespace", GetStringToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_TypeDef(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Flags", StandardTemplates.UInt32 },
				{ "Name", GetStringToken(statistics) },
				{ "Namespace", GetStringToken(statistics) },
				{ "Extends", CompoundMetaDataTokenColumnDef.TypeDefOrRef.SelectTemplate(statistics) },
				{ "FieldList", SimpleMetaDataTokenColumnDef.Field.SelectTemplate(statistics) },
				{ "MethodList", SimpleMetaDataTokenColumnDef.MethodDef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_FieldDef(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Flags", StandardTemplates.UInt16 },
				{ "Name", GetStringToken(statistics) },
				{ "Signature", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_MethodDef(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "RVA", StandardTemplates.UInt32 },
				{ "ImplFlags", StandardTemplates.UInt16 },
				{ "Flags", StandardTemplates.UInt16 },
				{ "Name", GetStringToken(statistics) },
				{ "Signature", GetBlobToken(statistics) },
				{ "ParamList", SimpleMetaDataTokenColumnDef.Param.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_Param(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Flags", StandardTemplates.UInt16 },
				{ "Sequence", StandardTemplates.UInt16 },
				{ "Name", GetStringToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_InterfaceImpl(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Class", SimpleMetaDataTokenColumnDef.TypeDef.SelectTemplate(statistics) },
				{ "Interface", CompoundMetaDataTokenColumnDef.TypeDefOrRef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_MemberRef(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Class", CompoundMetaDataTokenColumnDef.MemberRefParent.SelectTemplate(statistics) },
				{ "Name", GetStringToken(statistics) },
				{ "Signature", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_Constant(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Type", StandardTemplates.UInt8 },
				{ "Padding", StandardTemplates.UInt8 },
				{ "Parent", CompoundMetaDataTokenColumnDef.HasConstant.SelectTemplate(statistics) },
				{ "Value", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_CustomAttribute(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Parent", CompoundMetaDataTokenColumnDef.HasCustomAttribute.SelectTemplate(statistics) },
				{ "Type", CompoundMetaDataTokenColumnDef.CustomAttributeType.SelectTemplate(statistics) },
				{ "Value", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_FieldMarshal(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Parent", CompoundMetaDataTokenColumnDef.HasFieldMarshall.SelectTemplate(statistics) },
				{ "NativeType", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_DeclSecurity(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Action", StandardTemplates.UInt16 },
				{ "Parent", CompoundMetaDataTokenColumnDef.HasDeclSecurity.SelectTemplate(statistics) },
				{ "PermissionSet", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_ClassLayout(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Packing Size", StandardTemplates.UInt16 },
				{ "Class Size", StandardTemplates.UInt32 },
				{ "Parent", SimpleMetaDataTokenColumnDef.TypeDef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_FieldLayout(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Offset", StandardTemplates.UInt32 },
				{ "Field", SimpleMetaDataTokenColumnDef.FieldLayout.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_StandAloneSig(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Signature", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_EventMap(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Parent", SimpleMetaDataTokenColumnDef.TypeDef.SelectTemplate(statistics) },
				{ "EventList", SimpleMetaDataTokenColumnDef.Event.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_Event(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "EventFlags", StandardTemplates.UInt16 },
				{ "Name", GetStringToken(statistics) },
				{ "EventType", CompoundMetaDataTokenColumnDef.TypeDefOrRef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_PropertyMap(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Parent", SimpleMetaDataTokenColumnDef.TypeDef.SelectTemplate(statistics) },
				{ "Property List", SimpleMetaDataTokenColumnDef.MethodDef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_Property(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Flags", StandardTemplates.UInt16 },
				{ "Name", GetStringToken(statistics) },
				{ "Type", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_MethodSemantics(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Semantics", StandardTemplates.UInt16 },
				{ "Method", SimpleMetaDataTokenColumnDef.MethodDef.SelectTemplate(statistics) },
				{ "Association", CompoundMetaDataTokenColumnDef.HasSemantics.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_MethodImpl(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Class", SimpleMetaDataTokenColumnDef.TypeDef.SelectTemplate(statistics) },
				{ "MethodBody", CompoundMetaDataTokenColumnDef.MethodDefOrRef.SelectTemplate(statistics) },
				{ "MethodDeclaration", CompoundMetaDataTokenColumnDef.MethodDefOrRef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_ModuleRef(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Name", GetStringToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_TypeSpec(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Signature", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_ImplMap(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "MappingFlags", StandardTemplates.UInt16 },
				{ "MemberForwarded", CompoundMetaDataTokenColumnDef.MemberForwarded.SelectTemplate(statistics) },
				{ "ImportName", GetStringToken(statistics) },
				{ "ImportScope", SimpleMetaDataTokenColumnDef.MemberRef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_FieldRVA(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "RVA", StandardTemplates.UInt32 },
				{ "Field", SimpleMetaDataTokenColumnDef.Field.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_Assembly(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "HashAlgId", StandardTemplates.UInt32 },
				{ "Major Version", StandardTemplates.UInt16 },
				{ "Minor Version", StandardTemplates.UInt16 },
				{ "Build Number", StandardTemplates.UInt16 },
				{ "Revision Number", StandardTemplates.UInt16 },
				{ "Flags", StandardTemplates.UInt32 },
				{ "Public Key", GetBlobToken(statistics) },
				{ "Name", GetStringToken(statistics) },
				{ "Culture", GetStringToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_AssemblyRef(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Major Version", StandardTemplates.UInt16 },
				{ "Minor Version", StandardTemplates.UInt16 },
				{ "Build Number", StandardTemplates.UInt16 },
				{ "Revision Number", StandardTemplates.UInt16 },
				{ "Flags", StandardTemplates.UInt32 },
				{ "Public Key Token", GetBlobToken(statistics) },
				{ "Name", GetStringToken(statistics) },
				{ "Culture", GetStringToken(statistics) },
				{ "Hash Value", GetStringToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_AssemblyRefProcessor(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Processor", StandardTemplates.UInt32 },
				{ "AssemblyRef", SimpleMetaDataTokenColumnDef.AssemblyRef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_AssemblyRefOS(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "OS Platform ID", StandardTemplates.UInt32 },
				{ "OS Major Version", StandardTemplates.UInt32 },
				{ "OS Minor Version", StandardTemplates.UInt32 },
				{ "AssemblyRef", SimpleMetaDataTokenColumnDef.AssemblyRefOS.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_File(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Flags", StandardTemplates.UInt32 },
				{ "Name", GetStringToken(statistics) },
				{ "HashValue", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_ExportedType(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Flags", StandardTemplates.UInt32 },
				{ "TypeDefId", SimpleMetaDataTokenColumnDef.TypeDef.LongForm },
				{ "Name", GetStringToken(statistics) },
				{ "Namespace", GetStringToken(statistics) },
				{ "Implementation", CompoundMetaDataTokenColumnDef.Implementation.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_ManifestResource(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Offset", StandardTemplates.UInt32 },
				{ "Flags", StandardTemplates.UInt32 },
				{ "Name", GetStringToken(statistics) },
				{ "Implementation", CompoundMetaDataTokenColumnDef.Implementation.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_NestedClass(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "NestedClass", SimpleMetaDataTokenColumnDef.TypeDef.SelectTemplate(statistics) },
				{ "EnclosingClass", SimpleMetaDataTokenColumnDef.TypeDef.SelectTemplate(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_GenericParam(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Number", StandardTemplates.UInt16 },
				{ "Flags", StandardTemplates.UInt16 },
				{ "Owner", CompoundMetaDataTokenColumnDef.TypeOrMethodDef.SelectTemplate(statistics) },
				{ "Name", GetStringToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_MethodSpec(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Method", CompoundMetaDataTokenColumnDef.MethodDefOrRef.SelectTemplate(statistics) },
				{ "Instantiation", GetBlobToken(statistics) },
			};
		}

		static IStructuralNodeTemplate CreateRowNode_GenericParamConstraint(MetaDataTableStatistics statistics)
		{
			return new CompoundNodeTemplate()
			{
				{ "Owner", SimpleMetaDataTokenColumnDef.GenericParam.SelectTemplate(statistics) },
				{ "Constraint", CompoundMetaDataTokenColumnDef.TypeDefOrRef.SelectTemplate(statistics) },
			};
		}

		static readonly IStructuralNodeTemplate RowNode_AssemblyProcessor = new CompoundNodeTemplate()
		{
			{ "Processor", StandardTemplates.UInt32 },
		};

		static readonly IStructuralNodeTemplate RowNode_AssemblyOS = new CompoundNodeTemplate()
		{
			{ "OS Platform ID", StandardTemplates.UInt32 },
			{ "OS Major Version", StandardTemplates.UInt32 },
			{ "OS Minor Version", StandardTemplates.UInt32 },
		};

		static IStructuralNodeTemplate CreateRowHeader(MetaDataTableCodes[] codes)
		{
			var result = new CompoundNodeTemplate();

			for (var i = 0; i < codes.Length; i++)
			{
				result.Add(codes[i].ToString(), StandardTemplates.UInt32);
			}

			return result;
		}

		static IStructuralNodeTemplate GetStringToken(MetaDataTableStatistics statistics)
			=> statistics.UseLargeStringIndex ? StandardTemplates.UInt32 : StandardTemplates.UInt16;

		static IStructuralNodeTemplate GetGuidToken(MetaDataTableStatistics statistics)
			=> statistics.UseLargeGuidIndex ? StandardTemplates.UInt32 : StandardTemplates.UInt16;

		static IStructuralNodeTemplate GetBlobToken(MetaDataTableStatistics statistics)
			=> statistics.UseLargeBlobIndex ? StandardTemplates.UInt32 : StandardTemplates.UInt16;

		int[] GetRowCounts(MetaDataTableCodes[] codes)
		{
			var offset = ByteRange.Offset + 24;
			var result = new int[0x40];

			for (var i = 0; i < codes.Length; i++)
			{
				var tableNum = unchecked((int)codes[i]);
				result[tableNum] = _provider.Data.Read<int>(offset);
				offset += 4;
			}

			return result;
		}

		static Func<int, long, string> GetMetaDataTokenFormatter(MetaDataTableCodes code)
		{
			var tmp = ((int)code) << 24;
			return (i, l) => ((i + 1) | tmp).ToString("X08", CultureInfo.InvariantCulture);
		}

		static MetaDataTableCodes[] GetCodes(MetaDataTableFlags flags)
		{
			var tmp = unchecked((ulong)flags);
			var result = new MetaDataTableCodes[CountBits(tmp)];
			var i = 0;

			while (tmp != 0)
			{
				var next = tmp & ~(tmp - 1);
				tmp ^= next;
				result[i++] = unchecked((MetaDataTableCodes)CountBits(next - 1));
			}

			return result;
		}

		static int CountBits(ulong value)
		{
			value -= value >> 1 & 0x5555555555555555;
			value = (value >> 2 & 0x3333333333333333) + (value & 0x3333333333333333);
			value = (value >> 4 & 0x0F0F0F0F0F0F0F0F) + (value & 0x0F0F0F0F0F0F0F0F);
			value = (value * 0x0101010101010101) >> 56;
			return unchecked((int)value);
		}

		readonly PEStructuralNodeProvider _provider;
	}
}
