using System;
using System.IO;
namespace LibGGPK
{
	public sealed class GGPKRecord : BaseRecord
	{
		public const string Tag = "GGPK";
		public long[] RecordOffsets;
		public GGPKRecord(uint length, BinaryReader br)
		{
			this.RecordBegin = br.BaseStream.Position - 8L;
			this.Length = length;
			this.Read(br);
		}
		public override void Read(BinaryReader br)
		{
			int num = br.ReadInt32();
			this.RecordOffsets = new long[num];
			for (int i = 0; i < num; i++)
			{
				this.RecordOffsets[i] = br.ReadInt64();
			}
		}
		public override string ToString()
		{
			return "GGPK";
		}
	}
}
