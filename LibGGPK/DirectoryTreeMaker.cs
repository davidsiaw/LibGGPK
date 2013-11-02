using System;
using System.Collections.Generic;
namespace LibGGPK
{
	internal static class DirectoryTreeMaker
	{
		internal static DirectoryTreeNode BuildDirectoryTree(Dictionary<long, BaseRecord> recordOffsets)
		{
			GGPKRecord gGPKRecord = recordOffsets[0L] as GGPKRecord;
			DirectoryTreeNode directoryTreeNode = new DirectoryTreeNode
			{
				Children = new List<DirectoryTreeNode>(),
				Files = new List<FileRecord>(),
				Name = "ROOT",
				Parent = null,
				Record = null
			};
			long[] recordOffsets2 = gGPKRecord.RecordOffsets;
			for (int i = 0; i < recordOffsets2.Length; i++)
			{
				long fileOffset = recordOffsets2[i];
				DirectoryTreeMaker.BuildDirectoryTree(fileOffset, directoryTreeNode, recordOffsets);
			}
			return directoryTreeNode;
		}
		private static void BuildDirectoryTree(long fileOffset, DirectoryTreeNode root, Dictionary<long, BaseRecord> recordOffsets)
		{
			if (!recordOffsets.ContainsKey(fileOffset))
			{
				return;
			}
			if (recordOffsets[fileOffset] is DirectoryRecord)
			{
				DirectoryRecord directoryRecord = recordOffsets[fileOffset] as DirectoryRecord;
				DirectoryTreeNode directoryTreeNode = new DirectoryTreeNode
				{
					Name = directoryRecord.Name,
					Parent = root,
					Children = new List<DirectoryTreeNode>(),
					Files = new List<FileRecord>(),
					Record = directoryRecord
				};
				root.Children.Add(directoryTreeNode);
				DirectoryRecord.DirectoryEntry[] entries = directoryRecord.Entries;
				for (int i = 0; i < entries.Length; i++)
				{
					DirectoryRecord.DirectoryEntry directoryEntry = entries[i];
					DirectoryTreeMaker.BuildDirectoryTree(directoryEntry.Offset, directoryTreeNode, recordOffsets);
				}
				return;
			}
			if (recordOffsets[fileOffset] is FileRecord)
			{
				FileRecord fileRecord = recordOffsets[fileOffset] as FileRecord;
				fileRecord.ContainingDirectory = root;
				root.Files.Add(fileRecord);
			}
		}
	}
}
