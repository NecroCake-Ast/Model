using System;
using System.Collections.Generic;

namespace Model.Models.Statistics.Testing
{
    public class CTestStatistic
    {
        private List<CTestStatisticRecord> m_results;
        public  int     ID           { get; set; }
        public  string  Name         { get; set; }
        public  int     TestMaxScore { get; set; }
        public  double  TestMinScore { get; set; }
        public  double  MiddleScore  { get; private set; }
        public  double  MaxScore     { get; private set; }
        public  double  MinScore     { get; private set; }
        public  long    MiddleTime   { get; private set; }

        public List<CTestStatisticRecord> Results  {
            get => m_results;
            set {
                m_results = value;
                MinScore = double.MaxValue;
                MaxScore = MiddleScore = 0;
                MiddleTime = 0;
                foreach(CTestStatisticRecord curResult in m_results)
                {
                    if (MaxScore < curResult.Score)
                        MaxScore = curResult.Score;
                    if (MinScore > curResult.Score)
                        MinScore = curResult.Score;
                    MiddleScore += curResult.Score;
                    MiddleTime += curResult.Time;
                }
                if (m_results.Count != 0)
                {
                    MiddleScore /= m_results.Count;
                    MiddleTime /= m_results.Count;
                }
            }
        }
    }
}
