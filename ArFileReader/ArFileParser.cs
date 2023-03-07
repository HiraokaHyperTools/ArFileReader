using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArFileReader
{
    /// <summary>
    /// An ar file parser
    /// </summary>
    /// <see cref="https://ja.wikipedia.org/wiki/Ar_(UNIX)"/>
    public class ArFileParser
    {
        private static readonly Encoding _raw = Encoding.GetEncoding("latin1");

        public static ReadOnlyCollection<ArFileEntry> Parse(Span<byte> file)
        {
            if (file.Length < 8)
            {
                throw new EndOfStreamException();
            }
            if (false
                || file[0] != 0x21
                || file[1] != 0x3C
                || file[2] != 0x61
                || file[3] != 0x72
                || file[4] != 0x63
                || file[5] != 0x68
                || file[6] != 0x3E
                || file[7] != 0x0A
            )
            {
                throw new InvalidDataException();
            }

            var list = new List<ArFileEntry>();

            var position = 8;

            while (position < file.Length)
            {
                if (file.Length <= position + 60)
                {
                    throw new EndOfStreamException();
                }

                var head2 = file.Slice(position, 60).ToArray();

                var entry = new ArFileEntry();
                entry.FileName = _raw.GetString(head2, 0x8 - 8, 16).TrimEnd();
                entry.Modification = _raw.GetString(head2, 0x18 - 8, 12).TrimEnd();
                entry.OwnerID = _raw.GetString(head2, 0x24 - 8, 6).TrimEnd();
                entry.GroupID = _raw.GetString(head2, 0x2a - 8, 6).TrimEnd();
                entry.FileMode = _raw.GetString(head2, 0x30 - 8, 8).TrimEnd();
                {
                    long.TryParse(_raw.GetString(head2, 0x38 - 8, 8).TrimEnd(), out long value);
                    entry.FileSize = value;
                }
                position += 60;
                entry.Position = position;
                list.Add(entry);

                position += Convert.ToInt32((entry.FileSize + 1) & ~1);
            }

            return list.AsReadOnly();
        }

        public static ReadOnlyCollection<ArFileEntry> ReparseAsGnu(Span<byte> file, IEnumerable<ArFileEntry> entries)
        {
            var list = new List<ArFileEntry>();

            var state = 0;

            string fileNameTable = "";

            foreach (var source in entries)
            {
                if (state == 0)
                {
                    if (source.FileName == "//")
                    {
                        state = 1;

                        fileNameTable = _raw.GetString(ReadData(file, source).ToArray());
                    }
                }
                else if (state == 1)
                {
                    string name;

                    if (source.FileName.StartsWith("/") && int.TryParse(source.FileName.Substring(1), out int offset))
                    {
                        name = fileNameTable.Substring(offset).Split('\0').First();
                    }
                    else
                    {
                        name = source.FileName.TrimEnd();
                    }

                    list.Add(
                        new ArFileEntry
                        {
                            FileName = name,
                            Modification = source.Modification,
                            OwnerID = source.OwnerID,
                            GroupID = source.GroupID,
                            FileMode = source.FileMode,
                            FileSize = source.FileSize,
                            Position = source.Position,
                        }
                    );
                }
            }

            if (state == 0)
            {
                // not gnu
                list.AddRange(entries);
            }

            return list.AsReadOnly();
        }

        public static Span<byte> ReadData(Span<byte> file, ArFileEntry entry) =>
            file.Slice(Convert.ToInt32(entry.Position), Convert.ToInt32(entry.FileSize));
    }
}
