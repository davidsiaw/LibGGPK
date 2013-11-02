using System;
using System.IO;
using System.Text;
namespace LibGGPK
{
	internal static class RecordFactory
	{
		public static BaseRecord ReadRecord(BinaryReader br)
		{
			uint length = br.ReadUInt32();
			string @string = Encoding.ASCII.GetString(br.ReadBytes(4));
			string a;
			if ((a = @string) != null)
			{
				if (a == "FILE")
				{
					return new FileRecord(length, br);
				}
				if (a == "GGPK")
				{
					return new GGPKRecord(length, br);
				}
				if (a == "FREE")
				{
					return new FreeRecord(length, br);
				}
				if (a == "PDIR")
				{
					return new DirectoryRecord(length, br);
				}
			}
			throw new Exception("Invalid tag");
		}
	}
}
