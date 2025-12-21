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
        public static string ConnectionString
        {
            get
            {
                var parser = new FileIniDataParser();
                var data = parser.ReadFile("config.ini");

                return $"Server={data["Database"]["Server"]};" +
                       $"Database={data["Database"]["Database"]};" +
                       $"Uid={data["Database"]["Uid"]};" +
                       $"Pwd={data["Database"]["Pwd"]};" +
                       $"Port={data["Database"]["Port"]};";
            }
        }
    }
}
