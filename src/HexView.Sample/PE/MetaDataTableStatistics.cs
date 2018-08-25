// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
namespace HexView.Plugins.Sample.PE
{
	sealed class MetaDataTableStatistics
	{
		public MetaDataTableStatistics(int[] rowCounts, HeapSizeFlags heapSizes)
		{
			_rowCounts = rowCounts;
			_heapSizes = heapSizes;
		}

		public int GetRowCount(MetaDataTableCodes code)
		{
			var id = (int)code;
			return id < _rowCounts.Length ? _rowCounts[id] : 0;
		}

		public bool UseLargeBlobIndex => (_heapSizes & HeapSizeFlags.BLOB_HEAP_USES_LARGE_INDEXES) != 0;
		public bool UseLargeStringIndex => (_heapSizes & HeapSizeFlags.STRING_HEAP_USES_LARGE_INDEXES) != 0;
		public bool UseLargeGuidIndex => (_heapSizes & HeapSizeFlags.GUID_HEAP_USES_LARGE_INDEXES) != 0;

		readonly int[] _rowCounts;
		readonly HeapSizeFlags _heapSizes;
	}
}
