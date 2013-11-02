using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace LibGGPK
{
	public sealed class FileRecord : BaseRecord, IComparable
	{
		public enum DataFormat
		{
			Unknown,
			Image,
			Ascii,
			Unicode,
			RichText,
			Sound,
			Dat,
			TextureDDS
		}
		public const string Tag = "FILE";
		public byte[] Hash;
		public string Name;
		public long DataBegin;
		public long DataLength;
		public DirectoryTreeNode ContainingDirectory;
		private string directoryPath;
		private static readonly Dictionary<string, FileRecord.DataFormat> KnownFileFormats = new Dictionary<string, FileRecord.DataFormat>
		{

			{
				".act",
				FileRecord.DataFormat.Unicode
			},

			{
				".amd",
				FileRecord.DataFormat.Unicode
			},

			{
				".ao",
				FileRecord.DataFormat.Unicode
			},

			{
				".aoc",
				FileRecord.DataFormat.Unicode
			},

			{
				".arl",
				FileRecord.DataFormat.Unicode
			},

			{
				".arm",
				FileRecord.DataFormat.Unicode
			},

			{
				".ast",
				FileRecord.DataFormat.Unknown
			},

			{
				".atlas",
				FileRecord.DataFormat.Unicode
			},

			{
				".cfg",
				FileRecord.DataFormat.Ascii
			},

			{
				".cht",
				FileRecord.DataFormat.Unicode
			},

			{
				".clt",
				FileRecord.DataFormat.Unicode
			},

			{
				".csv",
				FileRecord.DataFormat.Ascii
			},

			{
				".dat",
				FileRecord.DataFormat.Dat
			},

			{
				".dct",
				FileRecord.DataFormat.Unicode
			},

			{
				".dds",
				FileRecord.DataFormat.TextureDDS
			},

			{
				".ddt",
				FileRecord.DataFormat.Unicode
			},

			{
				".dgr",
				FileRecord.DataFormat.Unicode
			},

			{
				".dlp",
				FileRecord.DataFormat.Unicode
			},

			{
				".ecf",
				FileRecord.DataFormat.Unicode
			},

			{
				".env",
				FileRecord.DataFormat.Unicode
			},

			{
				".epk",
				FileRecord.DataFormat.Unicode
			},

			{
				".et",
				FileRecord.DataFormat.Unicode
			},

			{
				".ffx",
				FileRecord.DataFormat.Unicode
			},

			{
				".fmt",
				FileRecord.DataFormat.Unknown
			},

			{
				".fx",
				FileRecord.DataFormat.Ascii
			},

			{
				".gft",
				FileRecord.DataFormat.Unicode
			},

			{
				".gt",
				FileRecord.DataFormat.Unicode
			},

			{
				".idl",
				FileRecord.DataFormat.Unicode
			},

			{
				".idt",
				FileRecord.DataFormat.Unicode
			},

			{
				".jpg",
				FileRecord.DataFormat.Image
			},

			{
				".mat",
				FileRecord.DataFormat.Unicode
			},

			{
				".mel",
				FileRecord.DataFormat.Ascii
			},

			{
				".mtd",
				FileRecord.DataFormat.Unicode
			},

			{
				".mtp",
				FileRecord.DataFormat.Unknown
			},

			{
				".ogg",
				FileRecord.DataFormat.Sound
			},

			{
				".ot",
				FileRecord.DataFormat.Unicode
			},

			{
				".otc",
				FileRecord.DataFormat.Unicode
			},

			{
				".pet",
				FileRecord.DataFormat.Unicode
			},

			{
				".png",
				FileRecord.DataFormat.Image
			},

			{
				".properties",
				FileRecord.DataFormat.Ascii
			},

			{
				".psg",
				FileRecord.DataFormat.Unknown
			},

			{
				".red",
				FileRecord.DataFormat.Unicode
			},

			{
				".rs",
				FileRecord.DataFormat.Unicode
			},

			{
				".rtf",
				FileRecord.DataFormat.RichText
			},

			{
				".slt",
				FileRecord.DataFormat.Ascii
			},

			{
				".sm",
				FileRecord.DataFormat.Unicode
			},

			{
				".smd",
				FileRecord.DataFormat.Unknown
			},

			{
				".tdt",
				FileRecord.DataFormat.Unknown
			},

			{
				".tgr",
				FileRecord.DataFormat.Unicode
			},

			{
				".tgt",
				FileRecord.DataFormat.Unknown
			},

			{
				".tmd",
				FileRecord.DataFormat.Unknown
			},

			{
				".tsi",
				FileRecord.DataFormat.Unicode
			},

			{
				".tst",
				FileRecord.DataFormat.Unicode
			},

			{
				".ttf",
				FileRecord.DataFormat.Unknown
			},

			{
				".txt",
				FileRecord.DataFormat.Unicode
			},

			{
				".ui",
				FileRecord.DataFormat.Unicode
			},

			{
				".xls",
				FileRecord.DataFormat.Unknown
			},

			{
				".xlsx",
				FileRecord.DataFormat.Unknown
			},

			{
				".xml",
				FileRecord.DataFormat.Unicode
			}
		};
		public FileRecord.DataFormat FileFormat
		{
			get
			{
				return FileRecord.KnownFileFormats[Path.GetExtension(this.Name).ToLower()];
			}
		}
		public FileRecord(uint length, BinaryReader br)
		{
			this.RecordBegin = br.BaseStream.Position - 8L;
			this.Length = length;
			this.Read(br);
		}
		public override void Read(BinaryReader br)
		{
			int num = br.ReadInt32();
			this.Hash = br.ReadBytes(32);
			this.Name = Encoding.Unicode.GetString(br.ReadBytes(2 * (num - 1)));
			br.ReadBytes(2);
			this.DataBegin = br.BaseStream.Position;
			this.DataLength = (long)((ulong)this.Length - (ulong)((long)(8 + num * 2 + 32 + 4)));
			br.BaseStream.Seek(this.DataLength, SeekOrigin.Current);
		}
		public string ExtractTempFile(string ggpkPath)
		{
			string tempFileName = Path.GetTempFileName();
			string text = tempFileName + Path.GetExtension(this.Name);
			File.Move(tempFileName, text);
			this.ExtractFile(ggpkPath, text);
			return text;
		}
		public void ExtractFile(string ggpkPath, string outputPath)
		{
			byte[] bytes = this.ReadData(ggpkPath);
			File.WriteAllBytes(outputPath, bytes);
		}
		public void ExtractFileWithDirectoryStructure(string ggpkPath, string outputDirectory)
		{
			byte[] bytes = this.ReadData(ggpkPath);
			string text = outputDirectory + Path.DirectorySeparatorChar + this.GetDirectoryPath();
			Directory.CreateDirectory(text);
			File.WriteAllBytes(text + Path.DirectorySeparatorChar + this.Name, bytes);
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
		public string GetDirectoryPath()
		{
			if (this.directoryPath != null)
			{
				return this.directoryPath;
			}
			Stack<string> stack = new Stack<string>();
			StringBuilder stringBuilder = new StringBuilder();
			DirectoryTreeNode directoryTreeNode = this.ContainingDirectory;
			while (directoryTreeNode != null && directoryTreeNode.Name.Length > 0)
			{
				stack.Push(directoryTreeNode.Name);
				directoryTreeNode = directoryTreeNode.Parent;
			}
			foreach (string current in stack)
			{
				stringBuilder.Append(current + Path.DirectorySeparatorChar);
			}
			this.directoryPath = stringBuilder.ToString();
			return this.directoryPath;
		}
		private void MarkAsFree(FileStream ggpkFileStream, LinkedList<FreeRecord> freeRecordRoot)
		{
			byte[] bytes = BitConverter.GetBytes(0L);
			byte[] bytes2 = Encoding.ASCII.GetBytes("FREE");
			ggpkFileStream.Seek(this.RecordBegin + 4L, SeekOrigin.Begin);
			ggpkFileStream.Write(bytes2, 0, 4);
			ggpkFileStream.Write(bytes, 0, bytes.Length);
			ggpkFileStream.Seek(freeRecordRoot.Last.Value.RecordBegin + 8L, SeekOrigin.Begin);
			ggpkFileStream.Write(BitConverter.GetBytes(this.RecordBegin), 0, 8);
		}
		public void ReplaceContents(string ggpkPath, byte[] replacmentData, LinkedList<FreeRecord> freeRecordRoot)
		{
			long recordBegin = this.RecordBegin;
			using (FileStream fileStream = File.Open(ggpkPath, FileMode.Open))
			{
				this.MarkAsFree(fileStream, freeRecordRoot);
				fileStream.Seek(0L, SeekOrigin.End);
				this.RecordBegin = fileStream.Position;
				this.Length = (uint)((ulong)this.Length - (ulong)this.DataLength + (ulong)((long)replacmentData.Length));
				BinaryWriter binaryWriter = new BinaryWriter(fileStream);
				binaryWriter.Write(this.Length);
				binaryWriter.Write("FILE".ToCharArray(0, 4));
				binaryWriter.Write(this.Name.Length + 1);
				binaryWriter.Write(this.Hash);
				binaryWriter.Write(Encoding.Unicode.GetBytes(this.Name + "\0"));
				this.DataBegin = binaryWriter.BaseStream.Position;
				this.DataLength = (long)replacmentData.Length;
				binaryWriter.Write(replacmentData);
			}
			this.ContainingDirectory.Record.UpdateOffset(ggpkPath, recordBegin, this.RecordBegin);
		}
		public void ReplaceContents(string ggpkPath, string replacmentPath, LinkedList<FreeRecord> freeRecordRoot)
		{
			this.ReplaceContents(ggpkPath, File.ReadAllBytes(replacmentPath), freeRecordRoot);
		}
		public override string ToString()
		{
			return this.Name;
		}
		public int CompareTo(object obj)
		{
			if (!(obj is FileRecord))
			{
				throw new NotImplementedException("Can only compare FileRecords");
			}
			return this.Name.CompareTo((obj as FileRecord).Name);
		}
	}
}
