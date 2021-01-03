using Model.Models.Testing.Score;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Testing
{
    /********************************************************************\
    КЛАСС.....: CTest
    НАСЛЕДУЕТ.: List<CBlock>
    ОПИСАНИЕ..: Тест, состоящий из нескольких блоков
    ПОЛЯ......: int ID - ID теста
                double MinScore - минимальный балл, требуемый для успешного
                завершения блока.
                long Time - максимальное время выполнения блока.
                IScoreSummator Summator - подводит итог блока после
                его завершения.
    МЕТОДЫ....: double Score(List<string> userAnswer, List<long> userTime)
                - возвращает итоговый балл за блок заданий.
    ПРИМЕЧАНИЕ: - Максимальное время выполнения блока не совпадает с суммой
                ожидаемого времени всех вопросов блока.
    \********************************************************************/
    public class CTest
    {
        public  int            ID       { get; set; }
        public  double         MinScore { get; set; }
        public  IScoreSummator Summator { get; set; }
        public  List<CBlock>   Blocks   { get; set; }

        public CTest() { ID = 0; MinScore = 0; Summator = new CClearSummator(); Blocks = new List<CBlock>(); }

        [JsonConstructor]
        public CTest(int id, double minScore, long time, IScoreSummator summator, List<CBlock> blocks)
        {
            ID = id;
            MinScore = minScore;
            Summator = summator;
            Blocks = blocks;
        }

        public CTest(CTest other)
        {
            ID = other.ID;
            MinScore = other.MinScore;
            Summator = other.Summator;
            Blocks = new List<CBlock>(other.Blocks);
        }

        public double Score(List<List<string>> userAnswers, List<List<TimeSpan>> userTime)
        {
            List<double> score = new List<double>();
            List<TimeSpan> blockTimes = new List<TimeSpan>();
            for (int i = 0; i < Blocks.Count && i < userAnswers.Count && i < userTime.Count; i++)
            {
                score.Add(Blocks[i].Score(userAnswers[i], userTime[i]));
                TimeSpan SummaryTime = new TimeSpan();
                foreach (TimeSpan time in userTime[i])
                    SummaryTime += time;
                blockTimes.Add(SummaryTime);
            }
            return Summator.Calculate(score, blockTimes);
        }
    }
}
