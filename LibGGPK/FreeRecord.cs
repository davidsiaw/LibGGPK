using System;
using System.IO;
namespace LibGGPK
{
	public sealed class FreeRecord : BaseRecord
	{
		public const string Tag = "FREE";
		public long DataBegin;
		public long DataLength;
		public long NextFreeOffset;
		public FreeRecord(uint length, BinaryReader br)
		{
			this.RecordBegin = br.BaseStream.Position - 8L;
			this.Length = length;
			this.Read(br);
		}
		public override void Read(BinaryReader br)
		{
			base.Read(br);
			this.NextFreeOffset = br.ReadInt64();
			this.DataBegin = br.BaseStream.Position;
			this.DataLength = (long)((ulong)(this.Length - 16u));
			br.BaseStream.Seek((long)((ulong)(this.Length - 16u)), SeekOrigin.Current);
		}
		public byte[] ReadData(string ggpkPath)
		{
			byte[] array = new byte[this.DataLength];
			using (FileStream fileStream = File.Open(ggpkPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				fileStream.Seek(this.DataBegin, SeekOrigin.Begin);
				fileStream.Read(array, 0, array.Length);
			}
			return array;
		}
		public override string ToString()
		{
			return "FREE";
		}
	}
}
