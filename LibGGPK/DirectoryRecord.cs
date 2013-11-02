using System;
using System.IO;
using System.Text;
namespace LibGGPK
{
	public sealed class DirectoryRecord : BaseRecord
	{
		public struct DirectoryEntry
		{
			public int EntryNameHash;
			public long Offset;
		}
		public const string Tag = "PDIR";
		public byte[] Hash;
		public string Name;
		public DirectoryRecord.DirectoryEntry[] Entries;
		public long EntriesBegin;
		public DirectoryRecord(uint length, BinaryReader br)
		{
			this.RecordBegin = br.BaseStream.Position - 8L;
			this.Length = length;
			this.Read(br);
		}
		public override void Read(BinaryReader br)
		{
			int num = br.ReadInt32();
			int num2 = br.ReadInt32();
			this.Hash = br.ReadBytes(32);
			this.Name = Encoding.Unicode.GetString(br.ReadBytes(2 * (num - 1)));
			br.ReadBytes(2);
			this.EntriesBegin = br.BaseStream.Position;
			this.Entries = new DirectoryRecord.DirectoryEntry[num2];
			for (int i = 0; i < num2; i++)
			{
				this.Entries[i] = new DirectoryRecord.DirectoryEntry
				{
					EntryNameHash = br.ReadInt32(),
					Offset = br.ReadInt64()
				};
			}
		}
		public void UpdateOffset(string ggpkPath, long previousEntryOffset, long newEntryOffset)
		{
			int num = -1;
			for (int i = 0; i < this.Entries.Length; i++)
			{
				if (this.Entries[i].Offset == previousEntryOffset)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				throw new ApplicationException("Entry not found!");
			}
			using (FileStream fileStream = File.Open(ggpkPath, FileMode.Open))
			{
				fileStream.Seek(this.EntriesBegin + (long)(12 * num) + 4L, SeekOrigin.Begin);
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write(newEntryOffset);
				this.Entries[num].Offset = newEntryOffset;
			}
		}
		public override string ToString()
		{
			return this.Name;
		}
	}
}
