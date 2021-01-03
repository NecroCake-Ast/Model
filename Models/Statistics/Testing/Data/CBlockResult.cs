using Model.Models.Testing;
using Model.Models.Testing.Task;
using Model.Models.Work;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Model.Models.Statistics.Testing
{
    public class CBlockResult
    {
        public int               BlockID     { get; set; } // ID блока заданий
        public double            Score       { get; set; } // Полученный балл
        public int               MaxScore    { get; set; } // Максимальный балл за блок
        public List<CTaskResult> TaskResults { get; set; } // Результаты заданий блока
        public TimeSpan          Time        { get; set; } // Время выполнения блока

        private string Normalize(string str)
        {
            try
            {
                List<string> tmp = new List<string>(str.Trim().Split(' '));
                str = "";
                for (int i = 0; i < tmp.Count; i++)
                    try { str += (int.Parse(tmp[i]) - 1) + " "; } catch { }
                if (str.Length > 0)
                    return str.Substring(0, str.Length - 1);
            }
            catch { }
            return str;
        }

        public CBlockResult(CBlock block, List<string> answers, List<TimeSpan> time)
        {
            BlockID = block.ID;
            Time = new TimeSpan(0);
            foreach (TimeSpan curTime in time)
                Time += curTime;
            MaxScore = block.Tasks.Count;
            TaskResults = new List<CTaskResult>();
            for (int TaskIdx = 0; TaskIdx < block.Tasks.Count; TaskIdx++)
            {
                CTaskResult result = new CTaskResult();
                result.Time = time[TaskIdx];
                result.TaskID = block.Tasks[TaskIdx].ID;
                switch (block.Tasks[TaskIdx].Type)
                {
                    case ETaskTypeID.TID_SINGLE:
                        if (answers[TaskIdx] != null)
                        {
                            try
                            {
                                result.Answer = ((CSingleAnswer)block.Tasks[TaskIdx]).Variants[int.Parse(answers[TaskIdx]) - 1];
                            } catch { }
                        }
                        else
                            result.Answer = "";
                        break;
                    case ETaskTypeID.TID_SEQUENCE:
                        if (answers[TaskIdx] != null)
                            result.Answer = answers[TaskIdx];
                        else
                            result.Answer = "";
                        break;
                    case ETaskTypeID.TID_OPEN:
                        if (answers[TaskIdx] != null)
                            result.Answer = answers[TaskIdx];
                        else
                            result.Answer = "";
                        break;
                    case ETaskTypeID.TID_MULTI:
                        if (answers[TaskIdx] != null)
                        {
                            List<string> Answers = new List<string>(answers[TaskIdx].Split(' '));
                            result.Answer = "";
                            foreach (string curAnswer in Answers)
                                try
                                {
                                    result.Answer += ((CMultiAnswer)block.Tasks[TaskIdx]).Variants[int.Parse(curAnswer) - 1] + "\n";
                                }
                                catch { }
                        }
                        else
                            result.Answer = "";
                        break;
                    case ETaskTypeID.TID_COMPLIANCE:
                        if (answers[TaskIdx] != null)
                            result.Answer = answers[TaskIdx];
                        else
                            result.Answer = "";
                        break;
                }
                result.CorrectAnswer = block.Tasks[TaskIdx].Correct;
                if (block.Tasks[TaskIdx].Type != ETaskTypeID.TID_OPEN)
                    answers[TaskIdx] = Normalize(answers[TaskIdx]);
                result.Score = block.Tasks[TaskIdx].CorrectScore(answers[TaskIdx], (long)time[TaskIdx].TotalSeconds);
                TaskResults.Add(result);
            }
            Score = block.Score(answers, time);
        }

        [JsonConstructor]
        public CBlockResult(int blockID, double score, int maxScore, List<CTaskResult> taskResults, TimeSpan time)
        {
            BlockID = blockID;
            Score = score;
            MaxScore = maxScore;
            TaskResults = taskResults;
            Time = time;
        }
    }
}
