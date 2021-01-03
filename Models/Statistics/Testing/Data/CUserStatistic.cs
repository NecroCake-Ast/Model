using Model.Models.Work;

namespace Model.Models.Statistics.Testing
{
    public class CUserStatistic
    {
        public string     TestName { get; set; }
        public CScoreData Scores   { get; set; }

        public CUserStatistic(CTestResult testResult, IWorkFinder workFinder)
        {
            TestName = workFinder.FindWorkByID(testResult.TestID).Name;
            Scores = testResult.Scores;
        }
    }
}