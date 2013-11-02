using System;
using System.IO;
namespace LibGGPK
{
	internal class Utils
	{
		public static FileStream OpenFile(string path, out bool isReadOnly)
		{
			isReadOnly = true;
			FileStream result;
			try
			{
				FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
				isReadOnly = false;
				result = fileStream;
			}
			catch (IOException)
			{
				result = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			}
			return result;
		}
	}
}
