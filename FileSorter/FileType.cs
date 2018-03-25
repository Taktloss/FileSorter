using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSorter
{
    class FileType
    {
        public string Name { get; set; }
        public string[] Extensions { get; set; }

        public FileType(string name, string[] extensions)
        {
            Name = name;
            Extensions = extensions;
        }
    }
}
