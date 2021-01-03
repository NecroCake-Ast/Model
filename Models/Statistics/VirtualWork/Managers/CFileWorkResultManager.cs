using Model.Models.User;
using Model.Models.Work;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Models.Statistics.Work
{
    public class CFileWorkResultManager : IWorkResultManager
    {
        private readonly string m_storagePath;

        public CFileWorkResultManager(string storagepath)
        {
            m_storagePath = storagepath;
        }

        public string GetResult(int WorkID, int UserID)
        {
            string fileName = UserID + "-" + WorkID + ".txt";
            try
            {
                StreamReader reader = new StreamReader(m_storagePath + "/" + fileName);
                string tmp = reader.ReadLine();
                reader.Close();
                return tmp;
            }
            catch { }
            return null;
        }

        public void Save(int WorkID, int UserID, string Result)
        {
            string fileName = UserID + "-" + WorkID + ".txt";
            StreamWriter writer = new StreamWriter(m_storagePath + "/" + fileName);
            if (writer != null)
            {
                writer.WriteLine(Result);
                writer.Close();
            }
        }
    }
}
