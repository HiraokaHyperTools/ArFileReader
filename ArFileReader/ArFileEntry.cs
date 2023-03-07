using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArFileReader
{
    public class ArFileEntry
    {
        public string FileName { get; set; }
        public string Modification { get; set; }
        public string OwnerID { get; set; }
        public string GroupID { get; set; }
        public string FileMode { get; set; }
        public long FileSize { get; set; }
        public long Position { get; set; }

        public override string ToString() => $"{FileName}";
    }
}
