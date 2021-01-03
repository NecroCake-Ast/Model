using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Works
{
    public class CFileSettingReader : ISettingReader
    {

        private readonly string m_Path;
        public CFileSettingReader(string Path)
        {
            m_Path = Path;
        }

        public string LoadSetting(string WorkName)
        {
            StreamReader reader = new StreamReader(m_Path + "/" + WorkName);
            if (reader != null)
            {
                string tmp = "";
                while (!reader.EndOfStream)
                    tmp += reader.ReadLine() + '\n';
                reader.Close();
                return tmp;
            }
            return null;
        }
    }
}
