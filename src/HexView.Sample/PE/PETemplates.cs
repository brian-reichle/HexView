// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Text;
using HexView.Framework;

namespace HexView.Plugins.Sample.PE
{
	static class PETemplates
	{
		public static readonly IStructuralNodeTemplate MSDosHeader = new CompoundNodeTemplate()
		{
			{ "Signature", StandardTemplates.UInt16 },
			{ "Last Page Size", StandardTemplates.UInt16 },
			{ "File Pages", StandardTemplates.UInt16 },
			{ "Relocation Items", StandardTemplates.UInt16 },
			{ "Header Paragraphs", StandardTemplates.UInt16 },
			{ "MINALLOC", StandardTemplates.UInt16 },
			{ "MAXALLOC", StandardTemplates.UInt16 },
			{ "Initial SS Value", StandardTemplates.UInt16 },
			{ "Initial SP Value", StandardTemplates.UInt16 },
			{ "Checksum", StandardTemplates.UInt16 },
			{ "Initial IP Value", StandardTemplates.UInt16 },
			{ "Initial CS Value (pre-relocated)", StandardTemplates.UInt16 },
			{ "Relocation Table Offset", StandardTemplates.UInt16 },
			{ "Overlay number", StandardTemplates.UInt16 },
			{ PositionMode.RelativeToParent, 0x3C, "Offset of 'new' header", StandardTemplates.UInt32 },
		};

		public static readonly IStructuralNodeTemplate PEHeaderTemplate = new CompoundNodeTemplate()
		{
			{ "Signature", StandardTemplates.UInt32 },
			{ "Machine", StandardTemplates.Enum<IMAGE_FILE_MACHINE>() },
			{ "NumberOfSections", StandardTemplates.UInt16 },
			{ "Time/Date Stamp", TimestampNodeTemplate.DateTime },
			{ "Pointer to Symbol Table", StandardTemplates.UInt32 },
			{ "Number of Symbols", StandardTemplates.UInt32 },
			{ "Optional Header Size", StandardTemplates.UInt16 },
			{ "Characteristics", StandardTemplates.Enum<IMAGE_FILE_CHARACTERISTICS>() },
		};

		public static readonly IStructuralNodeTemplate StandardOptionalHeaderTemplate = new CompoundNodeTemplate()
		{
			{ "Magic", StandardTemplates.Enum<IMAGE_FILE_MAGIC>() },
			{ "LMajor", StandardTemplates.UInt8 },
			{ "LMinor", StandardTemplates.UInt8 },
			{ "Code Size", StandardTemplates.UInt32 },
			{ "Initialized Data Size", StandardTemplates.UInt32 },
			{ "Uninitialized Data Size", StandardTemplates.UInt32 },
			{ "Entry Point RVA", StandardTemplates.UInt32 },
			{ "Base Of Code", StandardTemplates.UInt32 },
			{ "Base Of Data", StandardTemplates.UInt32 },
		};

		public static readonly IStructuralNodeTemplate StandardOptionalHeaderTemplate_Plus = new CompoundNodeTemplate()
		{
			{ "Magic", StandardTemplates.Enum<IMAGE_FILE_MAGIC>() },
			{ "LMajor", StandardTemplates.UInt8 },
			{ "LMinor", StandardTemplates.UInt8 },
			{ "Code Size", StandardTemplates.UInt32 },
			{ "Initialized Data Size", StandardTemplates.UInt32 },
			{ "Uninitialized Data Size", StandardTemplates.UInt32 },
			{ "Entry Point RVA", StandardTemplates.UInt32 },
			{ "Base Of Code", StandardTemplates.UInt32 },
		};

		public static readonly IStructuralNodeTemplate NTSpecificOptionalHeaderTemplate = new CompoundNodeTemplate()
		{
			{ "Image Base", StandardTemplates.UInt32 },
			{ "Section Alignment", StandardTemplates.UInt32 },
			{ "File Alignment", StandardTemplates.UInt32 },
			{ "OS Major", StandardTemplates.UInt16 },
			{ "OS Minor", StandardTemplates.UInt16 },
			{ "User Major", StandardTemplates.UInt16 },
			{ "User Minor", StandardTemplates.UInt16 },
			{ "SubSys Major", StandardTemplates.UInt16 },
			{ "SubSys Minor", StandardTemplates.UInt16 },
			{ "Reserved", StandardTemplates.UInt32 },
			{ "Image Size", StandardTemplates.UInt32 },
			{ "Header Size", StandardTemplates.UInt32 },
			{ "File Checksum", StandardTemplates.UInt32 },
			{ "SubSystem", StandardTemplates.Enum<IMAGE_SUBSYSTEM>() },
			{ "DLL Characteristics", StandardTemplates.Enum<IMAGE_DLL_CHARATERISTICS>() },
			{ "Stack Reserve Size", StandardTemplates.UInt32 },
			{ "Stack Commit Size", StandardTemplates.UInt32 },
			{ "Heap Reserve Size", StandardTemplates.UInt32 },
			{ "Heap Commit Size", StandardTemplates.UInt32 },
			{ "Loader Flags", StandardTemplates.UInt32 },
			{ "Number of Data Directories", StandardTemplates.UInt32 },
		};

		public static readonly IStructuralNodeTemplate NTSpecificOptionalHeaderTemplate_Plus = new CompoundNodeTemplate()
		{
			{ "Image Base", StandardTemplates.UInt64 },
			{ "Section Alignment", StandardTemplates.UInt32 },
			{ "File Alignment", StandardTemplates.UInt32 },
			{ "OS Major", StandardTemplates.UInt16 },
			{ "OS Minor", StandardTemplates.UInt16 },
			{ "User Major", StandardTemplates.UInt16 },
			{ "User Minor", StandardTemplates.UInt16 },
			{ "SubSys Major", StandardTemplates.UInt16 },
			{ "SubSys Minor", StandardTemplates.UInt16 },
			{ "Reserved", StandardTemplates.UInt32 },
			{ "Image Size", StandardTemplates.UInt32 },
			{ "Header Size", StandardTemplates.UInt32 },
			{ "File Checksum", StandardTemplates.UInt32 },
			{ "SubSystem", StandardTemplates.Enum<IMAGE_SUBSYSTEM>() },
			{ "DLL Characteristics", StandardTemplates.Enum<IMAGE_DLL_CHARATERISTICS>() },
			{ "Stack Reserve Size", StandardTemplates.UInt64 },
			{ "Stack Commit Size", StandardTemplates.UInt64 },
			{ "Heap Reserve Size", StandardTemplates.UInt64 },
			{ "Heap Commit Size", StandardTemplates.UInt64 },
			{ "Loader Flags", StandardTemplates.UInt32 },
			{ "Number of Data Directories", StandardTemplates.UInt32 },
		};

		public static readonly IStructuralNodeTemplate DictionaryTemplate = new CompoundNodeTemplate()
		{
			{ "Offset", StandardTemplates.Int32 },
			{ "Length", StandardTemplates.Int32 },
		};

		public static readonly IStructuralNodeTemplate SectionHeaderTemplate = new CompoundNodeTemplate()
		{
			{ "Name", StandardTemplates.Text(Constants.Section_Name_Length, Encoding.ASCII) },
			{ "Virtual Size", StandardTemplates.UInt32 },
			{ "Virtual Address", StandardTemplates.UInt32 },
			{ "Size of Raw Data", StandardTemplates.UInt32 },
			{ "Pointer to Raw Data", StandardTemplates.UInt32 },
			{ "Pointer to Relocations", StandardTemplates.UInt32 },
			{ "Pointer to Linenumbers", StandardTemplates.UInt32 },
			{ "Number of Relocations", StandardTemplates.UInt16 },
			{ "Number of Linenumbers", StandardTemplates.UInt16 },
			{ "Characteristics", StandardTemplates.Enum<IMAGE_SCN_CHARACTERISTICS>() },
		};

		public static readonly IStructuralNodeTemplate CLIHeaderTemplate = new CompoundNodeTemplate()
		{
			{ "Header Size in Bytes", StandardTemplates.UInt32 },
			{ "Major Runtime Version", StandardTemplates.UInt16 },
			{ "Minor Runtime Version", StandardTemplates.UInt16 },
			{ "MetaData", DictionaryTemplate },
			{ "Flags", StandardTemplates.Enum<COMIMAGE_FLAGS>() },
			{ "Entry Point Token", StandardTemplates.UInt32 },
			{ "Resources", DictionaryTemplate },
			{ "Strong Name Signature", StandardTemplates.UInt64 },
			{ "Code Manager Table", StandardTemplates.UInt64 },
			{ "VTable Fixups", StandardTemplates.UInt64 },
			{ "Export Address Table Jumps", StandardTemplates.UInt64 },
			{ "Managed Native Header", StandardTemplates.UInt64 },
		};

		public static readonly IStructuralNodeTemplate TableStreamHeader = new CompoundNodeTemplate()
		{
			{ "Reserved", StandardTemplates.UInt32 },
			{ "Major Version", StandardTemplates.UInt8 },
			{ "Minor Version", StandardTemplates.UInt8 },
			{ "Heap Sizes", StandardTemplates.Enum<HeapSizeFlags>() },
			{ "Reserved", StandardTemplates.UInt8 },
			{ "Valid", StandardTemplates.Enum<MetaDataTableFlags>() },
			{ "Sorted", StandardTemplates.Enum<MetaDataTableFlags>() },
		};

		public static readonly IStructuralNodeTemplate ResourceDirectoryHeader = new CompoundNodeTemplate()
		{
			{ "Characteristics", StandardTemplates.Int32 },
			{ "Time/Date Stamp", TimestampNodeTemplate.DateTime },
			{ "Major Version", StandardTemplates.Int16 },
			{ "Minor Version", StandardTemplates.Int16 },
			{ "Number of Name Entries", StandardTemplates.Int16 },
			{ "Number of ID Entries", StandardTemplates.Int16 },
		};

		public static readonly IStructuralNodeTemplate ResourceDataEntry = new CompoundNodeTemplate()
		{
			{ "Data RVA", StandardTemplates.Int32 },
			{ "Size", StandardTemplates.Int32 },
			{ "Code Page", StandardTemplates.Int32 },
			{ "Reserved", StandardTemplates.Blob(4) },
		};
	}
}
