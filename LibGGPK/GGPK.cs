using System;
using System.Collections.Generic;
using System.IO;
namespace LibGGPK
{
	public class GGPK
	{
		private const int EstimatedFileCount = 175000;
		public Dictionary<long, BaseRecord> RecordOffsets;
		public DirectoryTreeNode DirectoryRoot;
		public LinkedList<FreeRecord> FreeRoot;
		private bool isReadOnly;
		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
		}
		public GGPK()
		{
			this.RecordOffsets = new Dictionary<long, BaseRecord>(175000);
		}
		private void ReadRecordOfsets(string pathToGgpk, Action<string> output)
		{
			float num = 0f;
			using (FileStream fileStream = Utils.OpenFile(pathToGgpk, out this.isReadOnly))
			{
				BinaryReader binaryReader = new BinaryReader(fileStream);
				while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
				{
					long position = binaryReader.BaseStream.Position;
					BaseRecord value = RecordFactory.ReadRecord(binaryReader);
					this.RecordOffsets.Add(position, value);
					float num2 = (float)binaryReader.BaseStream.Position / (float)binaryReader.BaseStream.Length;
					if (num2 - num >= 0.1f)
					{
						if (output != null)
						{
							output(string.Format("{0:00.00}%{1}", 100.0 * (double)num2, Environment.NewLine));
						}
						num = num2;
					}
				}
				if (output != null)
				{
					output(string.Format("{0:00.00}%{1}", 100f * (float)binaryReader.BaseStream.Position / (float)binaryReader.BaseStream.Length, Environment.NewLine));
				}
			}
		}
		private void CreateDirectoryTree(Action<string> output)
		{
			this.DirectoryRoot = DirectoryTreeMaker.BuildDirectoryTree(this.RecordOffsets);
			this.FreeRoot = FreeListMaker.BuildFreeList(this.RecordOffsets);
		}
		public void Read(string pathToGgpk, Action<string> output)
		{
			if (output != null)
			{
				output("Parsing GGPK..." + Environment.NewLine);
			}
			this.ReadRecordOfsets(pathToGgpk, output);
			if (output != null)
			{
				output(Environment.NewLine);
				output("Building directory tree..." + Environment.NewLine);
			}
			this.CreateDirectoryTree(output);
		}
	}
}
