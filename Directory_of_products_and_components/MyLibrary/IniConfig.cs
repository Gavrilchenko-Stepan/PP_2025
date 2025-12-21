using IniParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class IniConfig
    {
        public static string ConnectionString =>
            "Server=localhost;Database=product_catalog;Uid=root;Pwd=vertrigo;Port=3306;";
    }
}
