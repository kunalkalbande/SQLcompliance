using System.Text;
internal class FileParameter
        {
            public FileParameter(byte[] file) : this(file, null)
            {
            }

            public FileParameter(byte[] file, string filename) : this(file, filename, null)
            {
            }

            public FileParameter(byte[] file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }

            public byte[] File { get; set; }

            public string FileName { get; set; }

            public string ContentType { get; set; }

            public override string ToString()
            {
                var toStringBuilder = new StringBuilder();
                toStringBuilder.AppendFormat("FileName: {0}\r\n", FileName);
                toStringBuilder.AppendFormat("ContentType: {0}\r\n", ContentType);
                toStringBuilder.AppendFormat("FileLength: {0}", File.Length);
                return toStringBuilder.ToString();
            }
        }