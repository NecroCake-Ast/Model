using Newtonsoft.Json;
using System;

namespace Model.Models.Statistics.Testing
{
    public class CTaskResult
    {
        public int      TaskID        { get; set; } // ID задания
        public double   Score         { get; set; } // Полученный балл
        public string   Answer        { get; set; } // Ответ пользователя
        public string   CorrectAnswer { get; set; } // Правильный ответ
        public TimeSpan Time          { get; set; } // Время выполнения задания

        public CTaskResult()
        {
            TaskID = 0;
            Score = 0;
            Answer = "";
            CorrectAnswer = "";
        }

        [JsonConstructor]
        public CTaskResult(int taskID, double score, string answer, string correctAnswer, TimeSpan time)
        {
            TaskID = taskID;
            Score = score;
            Answer = answer;
            CorrectAnswer = correctAnswer;
            Time = time;
        }
    }
}
