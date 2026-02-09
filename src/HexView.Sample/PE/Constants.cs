// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
namespace HexView.Plugins.Sample.PE;

static class Constants
{
	public const short MSDos_Signature = 0x5a4d;
	public const int MSDos_DosHeaderSize = 0x08;
	public const int MSDos_NewHeaderOffset = 0x3C;

	public const int Section_Name_Offset = 0;
	public const int Section_Name_Length = 8;
	public const int Section_VirtualSize_Offset = 8;
	public const int Section_VirtualAddress_Offset = 12;
	public const int Section_SizeOfRawData_Offset = 16;
	public const int Section_PointerToRawData_Offset = 20;
	public const int Section_Length = 40;

	public const int Directory_RVAOffset = 0;
	public const int Directory_LengthOffset = 4;
	public const int Directory_Length = 8;

	public const int Directory_Index_Resources = 2;
	public const int Directory_Index_CLI = 14;

	public const int Header_Signature = 0x4550;
	public const int Header_SectionCountOffset = 6;
	public const int Header_OptionalHeaderSizeOffset = 20;
	public const int Header_Length = 24;

	public const int OptHeader_DirectoryCountOffset = 92;
	public const int OptHeader_DirectoryOffset = 96;

	public const int OptHeaderPlus_DirectoryCountOffset = 108;
	public const int OptHeaderPlus_DirectoryOffset = 112;

	public const int CLIHeader_MetaDataRVA_Offset = 8;
	public const int CLIHeader_MetaDataLength_Offset = 12;

	public static string GetDirectoryName(int index)
	{
		if (index >= DirectoryNames.Length)
		{
			return "Directory" + index;
		}
		else
		{
			return DirectoryNames[index];
		}
	}

	static readonly string[] DirectoryNames =
	[
		"Export Table",
		"Import Table",
		"Resource Table",
		"Exception Table",
		"Certificate Table",
		"Base Relocation Table",
		"Debug",
		"Copyright",
		"Global Ptr",
		"TLS Table",
		"Load Config Table",
		"Bound Import",
		"IAT",
		"Delay Import Descriptor",
		"CLI Header",
		"Reserved",
	];
}
