using Model.Models.User;
using System;

namespace Model.Models.Statistics.Testing
{
    public class CTestStatisticRecord
    {
        public int      ID    { get; set; }
        public string   Name  { get; set; }
        public double   Score { get; set; }
        public long     Time  { get; set; }
        public DateTime Date  { get; set; }

        public CTestStatisticRecord(CTestResult result, IUserFinder userFinder)
        {
            ID = result.StudentID;
            Time = (long)result.Time.TotalSeconds;
            Date = result.Date;
            CFindRecord userRecord = userFinder.findByID(ID);
            if (userRecord != null)
                Name = userRecord.user.FirstName + " " + userRecord.user.SecondName;
            else
                Name = "ID-" + ID;
            Score = result.Scores.Score;
        }
    }
}
