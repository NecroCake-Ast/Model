using Model.Models.Testing.Score;
using Model.Models.Testing.Task;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Model.Models.Testing
{
    /********************************************************************\
    КЛАСС.....: CBlock
    НАСЛЕДУЕТ.: List<ITask>
    ОПИСАНИЕ..: Список вопросов, принадлежащих одному блоку. Дополнительно
                содержит информацию о минимальном балле, требуемом для
                успешного завершения блока и теста, а также о максимальном
                времени выполнения всех заданий в блоке.
    ПОЛЯ......: string Name - название блока.
                int ID - ID блока заданий.
                HashSet<int> Requires - ID блоков, которые должны быть
                выполнены для допуска.
                double MinScore - минимальный балл, требуемый для успешного
                завершения блока.
                long Time - максимальное время выполнения блока.
                IScoreSummator Summator - подводит итог блока после
                его завершения.
    МЕТОДЫ....: double Score(List<string> userAnswer, List<long> userTime)
                - возвращает итоговый балл за блок заданий.
                bool Access(HashSet<int> blocks) - проверка допуска к блоку
    ПРИМЕЧАНИЕ: - Максимальное время выполнения блока не совпадает с суммой
                ожидаемого времени всех вопросов блока.
    \********************************************************************/
    public class CBlock
    {
        public string         Name     { get; set; } // Название блока
        public int            ID       { get; set; } // ID блока
        public HashSet<int>   Requires { get; set; } // Требуемые блоки
        public double         MinScore { get; set; } // Минимальный балл
        public long           Time     { get; set; } // Ожидаемое время выполнения
        public IScoreSummator Summator { get; set; } // Способ подведения итога
        public List<ITask>    Tasks    { get; set; } // Задания блока

        /********************************************************************\
        КОНСТРУКТОР.: CBlock
        ПАРАМЕТРЫ...: string name - название блока.
                      int id - ID блока.
                      HashSet<int> requires - требования для допуска.
                      double minScore - минимальный балл для успешного
                      завершения блока.
                      long time - ожидаемое время выполнения блока.
                      IScoreSummator summator - способ подведения итогов.
        \********************************************************************/

        [JsonConstructor]
        public CBlock(
            string name,
            int id,
            HashSet<int> requires,
            double minScore,
            long time,
            IScoreSummator summator,
            List<ITask> tasks)
        {
            Name = name;
            ID = id;
            Requires = requires;
            MinScore = minScore;
            Time = time;
            Summator = summator;
            Tasks = tasks;
        }

        /********************************************************************\
        КОНСТРУКТОР.: CBlock
        НАЗНАЧЕНИЕ..: Копирование
        ПАРАМЕТРЫ...: CBlock other - копируемый блок
        \********************************************************************/
        public CBlock(CBlock other)
        {
            Name = other.Name;
            ID = other.ID;
            Requires = other.Requires == null ? null : new HashSet<int>(other.Requires);
            MinScore = other.MinScore;
            Time = other.Time;
            Summator = other.Summator.Copy();
            Tasks = new List<ITask>(other.Tasks);
        }

        /********************************************************************\
        КОНСТРУКТОР.: CBlock
        НАЗНАЧЕНИЕ..: Конструктор по умолчанию
        \********************************************************************/
        public CBlock()
        {
            Name = "";
            ID = 0;
            Requires = new HashSet<int>();
            MinScore = 0;
            Time = 0;
            Summator = new CClearSummator();
            Tasks = new List<ITask>();
        }

        /********************************************************************\
        МЕТОД.....: Score
        ОПИСАНИЕ..: Вычисляет итоговый балл за блок заданий
        ПАРАМЕТРЫ.: List<string> userAnswer - ответы пользователя на вопросы
                    блока.
                    List<long> userTime - время, затраченное пользователем на
                    вопросы блока.
        ВОЗВРАТ...: double - итоговый балл
        \********************************************************************/
        public double Score(List<string> userAnswer, List<TimeSpan> userTime)
        {
            List<double> score = new List<double>();
            for (int i = 0; i < Tasks.Count && i < userAnswer.Count && i < userTime.Count; i++)
                if (userAnswer[i] != null && userTime[i] != null)
                    score.Add(Tasks[i].CorrectScore(userAnswer[i], (long)userTime[i].TotalSeconds));
            return Summator.Calculate(score, userTime);
        }

        /********************************************************************\
        МЕТОД.....: Access
        ОПИСАНИЕ..: Проверка допуска к блоку.
        ПАРАМЕТРЫ.: HashSet<int> blocks - завершённые блоки теста.
        ВОЗВРАТ...: bool - есть ли допуск.
        \********************************************************************/
        public bool Access(HashSet<int> blocks)
        {
            HashSet<int> tmp = new HashSet<int>(Requires);
            tmp.ExceptWith(blocks);
            return tmp.Count == 0;
        }
    }
}
