using System;
using System.Collections.Generic;
namespace LibGGPK
{
	public class DirectoryTreeNode : IComparable
	{
		public DirectoryTreeNode Parent;
		public List<DirectoryTreeNode> Children;
		public List<FileRecord> Files;
		public string Name;
		public DirectoryRecord Record;
		public static void TraverseTreePreorder(DirectoryTreeNode root, Action<DirectoryTreeNode> directoryAction, Action<FileRecord> fileAction)
		{
			foreach (DirectoryTreeNode current in root.Children)
			{
				if (directoryAction != null)
				{
					directoryAction(current);
				}
				DirectoryTreeNode.TraverseTreePreorder(current, directoryAction, fileAction);
			}
			if (fileAction != null)
			{
				foreach (FileRecord current2 in root.Files)
				{
					fileAction(current2);
				}
			}
		}
		public static void TraverseTreePostorder(DirectoryTreeNode root, Action<DirectoryTreeNode> directoryAction, Action<FileRecord> fileAction)
		{
			foreach (DirectoryTreeNode current in root.Children)
			{
				DirectoryTreeNode.TraverseTreePostorder(current, directoryAction, fileAction);
				if (directoryAction != null)
				{
					directoryAction(current);
				}
			}
			if (fileAction != null)
			{
				foreach (FileRecord current2 in root.Files)
				{
					fileAction(current2);
				}
			}
		}
		public override string ToString()
		{
			return this.Name;
		}
		public int CompareTo(object obj)
		{
			if (!(obj is DirectoryTreeNode))
			{
				throw new NotImplementedException("Can only compare DirectoryTreeNodes");
			}
			return this.Name.CompareTo((obj as DirectoryTreeNode).Name);
		}
	}
}
