using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Models.Testing
{
    public class CActiveTestData
    {   
        public  List<List<string>>   Answers         { get; set; }         // Ответы, данные пользователем
        public  List<List<double>>   Scores          { get; set; }         // Баллы, набранные за задания
        public  List<double>         BlockScores     { get; set; }         // Баллы, набранные за блоки заданий
        public  HashSet<int>         EndedBlocks     { get; set; }         // Завершённые блоки.

        public CActiveTestData(int BlockCnt)
        {
            Answers = new List<List<string>>(new List<string>[BlockCnt]);
            Scores = new List<List<double>>(new List<double>[BlockCnt]);
            BlockScores = new List<double>(new double[BlockCnt]);
            EndedBlocks = new HashSet<int>();
        }

        [JsonConstructor]
        public CActiveTestData(List<List<string>> answers, List<List<double>> scores,
            List<double> blockscores, HashSet<int> endedblocks)
        {
            Answers = answers;
            Scores = scores;
            BlockScores = blockscores;
            EndedBlocks = endedblocks;
        }
    }
}
