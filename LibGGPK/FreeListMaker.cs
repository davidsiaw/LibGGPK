using System;
using System.Collections.Generic;
namespace LibGGPK
{
	internal class FreeListMaker
	{
		internal static LinkedList<FreeRecord> BuildFreeList(Dictionary<long, BaseRecord> recordOffsets)
		{
			LinkedList<FreeRecord> linkedList = new LinkedList<FreeRecord>();
			GGPKRecord gGPKRecord = recordOffsets[0L] as GGPKRecord;
			FreeRecord freeRecord = null;
			long[] recordOffsets2 = gGPKRecord.RecordOffsets;
			for (int i = 0; i < recordOffsets2.Length; i++)
			{
				long key = recordOffsets2[i];
				if (recordOffsets[key] is FreeRecord)
				{
					freeRecord = (recordOffsets[key] as FreeRecord);
					break;
				}
			}
			if (freeRecord == null)
			{
				throw new Exception("Failed to find FREE record root in GGPK header");
			}
			while (true)
			{
				linkedList.AddLast(freeRecord);
				long nextFreeOffset = freeRecord.NextFreeOffset;
				if (nextFreeOffset == 0L)
				{
					return linkedList;
				}
				if (!recordOffsets.ContainsKey(nextFreeOffset))
				{
					break;
				}
				freeRecord = (recordOffsets[freeRecord.NextFreeOffset] as FreeRecord);
				if (freeRecord == null)
				{
					goto Block_5;
				}
			}
			throw new Exception("Failed to find next FREE record in map of record offsets");
			Block_5:
			throw new Exception("Found a record that wasn't a FREE record while looking for next FREE record");
		}
	}
}
