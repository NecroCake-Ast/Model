using Model.Models.Testing;
using Model.Models.Testing.Task;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Model.Models.Statistics.Testing
{
    public class CTestResult
    {
        public int                TestID    { get; set; } // ID теста
        public int                StudentID { get; set; } // ID студента, выполнившего тест
        public CScoreData         Scores    { get; set; } // Балл, набранный за тест
        public List<CBlockResult> Results   { get; set; } // Ответы на вопросы блоков
        public DateTime           Date      { get; set; } // Дата выполнения теста
        public TimeSpan           Time      { get; set; } // Время выполнения теста

        public CTestResult(int studentID, CActiveTest test)
        {
            Scores = new CScoreData();
            Date = DateTime.Now;
            Time = new TimeSpan();
            foreach (var curBlock in test.Times)
                foreach (var curTaskTime in curBlock)
                    Time += curTaskTime;
            Scores.MinScore = test.Test.MinScore;
            Scores.MaxScore = 0;
            StudentID = studentID;
            TestID = test.Test.ID;
            Results = new List<CBlockResult>();
            for (int BlockIdx = 0; BlockIdx < test.Test.Blocks.Count; BlockIdx++)
            {
                Scores.MaxScore += test.Test.Blocks[BlockIdx].Tasks.Count;
                Results.Add(new CBlockResult(test.Test.Blocks[BlockIdx], test.TestData.Answers[BlockIdx],
                    test.Times[BlockIdx]));
            }
            Scores.Score = test.Test.Score(test.TestData.Answers, test.Times);
        }

        [JsonConstructor]
        public CTestResult(int studentID, int testID, CScoreData score, List<CBlockResult> results, DateTime date, TimeSpan time)
        {
            TestID = testID;
            StudentID = studentID;
            Scores = score;
            Results = results;
            Date = date;
            Time = time;
        }
    }
}
