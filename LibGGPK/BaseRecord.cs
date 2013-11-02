using System;
using System.IO;
namespace LibGGPK
{
	public class BaseRecord
	{
		public uint Length;
		public long RecordBegin;
		public virtual void Read(BinaryReader br)
		{
		}
	}
}
