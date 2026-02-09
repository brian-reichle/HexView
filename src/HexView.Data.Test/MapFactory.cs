// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.IO.MemoryMappedFiles;

namespace HexView.Data.Test;

static class MapFactory
{
	public static MemoryMappedFile CreatePatterenedMap(int size)
	{
		MemoryMappedFile? file = null;

		try
		{
			file = MemoryMappedFile.CreateNew(null, size);
			WritePattern(file, size);
			return file;
		}
		catch
		{
			file?.Dispose();
			throw;
		}
	}

	static void WritePattern(MemoryMappedFile file, int size)
	{
		using var accessor = file.CreateViewAccessor();
		for (var index = 0; index < size; index++)
		{
			accessor.Write(index, (byte)(index & 0xFF));
		}
	}
}
