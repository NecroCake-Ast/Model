using Model.Models.User;
using Model.Models.Work;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Models.Statistics.Testing
{
    public class CFileTestResultManager : ITestResultManager
    {
        private readonly string m_storagePath;

        public CFileTestResultManager(string storagepath)
        {
            m_storagePath = storagepath;
        }

        public void Save(CTestResult result)
        {
            string fileName = result.StudentID + "-" + result.TestID + ".json";
            StreamWriter writer = new StreamWriter(m_storagePath + "/" + fileName);
            if (writer != null)
                writer.Write(JsonConvert.SerializeObject(result, Program.JsonSetting));
            writer.Close();
        }

        public CTestResult GetResult(int TestID, int UserID)
        {
            CTestResult result = null;
            string fileName = UserID + "-" + TestID + ".json";
            try
            {
                StreamReader reader = new StreamReader(m_storagePath + "/" + fileName);
                string tmp = "";
                while (!reader.EndOfStream)
                    tmp += " " + reader.ReadLine();
                reader.Close();
                result = JsonConvert.DeserializeObject<CTestResult>(tmp, Program.JsonSetting);
            }
            catch { }
            return result;
        }

        public List<CUserStatistic> UserStatistic(int UserID, IWorkFinder workFinder)
        {
            List<CUserStatistic> result = new List<CUserStatistic>();
            try
            {
                List<string> Files = Directory.GetFiles(m_storagePath, UserID + "-*.json").ToList<string>();
                foreach(string curFile in Files)
                {
                    StreamReader reader = new StreamReader(curFile);
                    string tmp = "";
                    while (!reader.EndOfStream)
                        tmp += " " + reader.ReadLine();
                    reader.Close();
                    result.Add(new CUserStatistic(JsonConvert.DeserializeObject<CTestResult>(tmp, Program.JsonSetting), workFinder));
                }
            } catch { }
            return result;
        }

        public CTestStatistic TestStatistic(int TestID, IUserFinder userFinder)
        {
            CTestStatistic statistic = new CTestStatistic();
            List<CTestStatisticRecord> TestResults = new List<CTestStatisticRecord>();
            try
            {
                List<string> Files = Directory.GetFiles(m_storagePath, "*-" + TestID + ".json").ToList<string>();
                foreach (string curFile in Files)
                {
                    StreamReader reader = new StreamReader(curFile);
                    string tmp = "";
                    while (!reader.EndOfStream)
                        tmp += " " + reader.ReadLine();
                    reader.Close();
                    CTestResult result = JsonConvert.DeserializeObject<CTestResult>(tmp, Program.JsonSetting);
                    statistic.TestMaxScore = result.Scores.MaxScore;
                    statistic.TestMinScore = result.Scores.MinScore;
                    statistic.ID = result.TestID;
                    TestResults.Add(new CTestStatisticRecord(result, userFinder));
                }
            }
            catch { }
            statistic.Results = TestResults;
            return statistic;
        }
    }
}
