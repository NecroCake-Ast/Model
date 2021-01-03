using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Testing.Task
{
    /********************************************************************\
    КЛАСС.....: CComplianceAnswer
    НАСЛЕДУЕТ.: ITask
    ОПИСАНИЕ..: Задание на установление соответствия
    ПОЛЯ......: ETaskTypeID Type - ID типа задания
                int ID - ID задания
                string Text - текст вопроса
                uint Answer - правильный ответ
                long Time - ожидаемое время ответа (в секундах)
                List<string> m_firstSet - первое множество
                List<string> m_secondSet - второе множество
    МЕТОДЫ....: double CalcMultiplier(long time) - возвращает значение 
                коэффициента поправки
                List<uint> Parse(string str) - разбивает входную строку на
                список целых беззнаковых чисел.
                double CorrectScore(string userAnswer, long time)
                - возвращает балл за задание
    \********************************************************************/
    public class CComplianceAnswer : ITask
    {
        public string        Text      { get; set; }  // Текст вопроса
        public List<uint>    Answer    { get; set; }  // Правильный ответ
        public long          Time      { get; set; }  // Ожидаемое время ответа (в секундах)
        public List<string>  FirstSet  { get; set; }  // Первое множество
        public List<string>  SecondSet { get; set; }  // Второе множество
        public int           ID        { get; set; }  // ID задания
        public string        Correct   { get; set; }  // Правильный ответ
        public ETaskTypeID   Type      { get => ETaskTypeID.TID_COMPLIANCE; } // Тип задания

        /******************************\PRIVATE/******************************/

        /********************************************************************\
        МЕТОД.....: CalcMultiplier
        ОПИСАНИЕ..: Вычисляет значение коэффициента поправки балла исходя из
                    затраченного времени
        ПАРАМЕТРЫ.: long time - затраченное время
        ВОЗВРАТ...: double - коэффициент поправки
        \********************************************************************/
        private double CalcMultiplier(long time)
        {
            double multiplier = 1.0;
            while (time > Time)
            {
                time -= (long)(0.2 * Time);
                multiplier -= 0.1;
            }
            if (multiplier < 0.5)
                multiplier = 0.5;
            return multiplier;
        }

        /********************************************************************\
        МЕТОД.....: Parse
        ОПИСАНИЕ..: Разбивает входную строку на список целых беззнаковых чисел
        ПАРАМЕТРЫ.: string str - разбиваемая строка
        ВОЗВРАТ...: HashSet<uint> - полученное множество
        \********************************************************************/
        private List<uint> Parse(string str)
        {
            List<uint> res = new List<uint>();
            List<string> values = new List<string>(str.Split(' '));
            foreach (var i in values)
                try { res.Add(uint.Parse(i)); } catch { }
            return res;
        }

        /******************************\PUBLIC/******************************/

        /********************************************************************\
        КОНСТРУКТОР.: CComplianceAnswer
        ПАРАМЕТРЫ...: int id - ID задания
                      string text - текст задания
                      List<string> firstSet - первое множество
                      List<string> secondSet - второе множество
                      List<uint> answer - правильное соответствие
                      long time - ожидаемое время ответа (в секундах)
        \********************************************************************/
        public CComplianceAnswer(int id, string text, List<string> firstSet, List<string> secondSet,
            List<uint> answer, long time)
        {
            ID = id;
            Text = text;
            FirstSet = new List<string>(firstSet);
            SecondSet = new List<string>(secondSet);
            Answer = new List<uint>(answer);
            Time = time;
        }

        /********************************************************************\
        КОНСТРУКТОР.: CComplianceAnswer
        ПАРАМЕТРЫ...: CComplianceAnswer other - копируемое задание
        \********************************************************************/
        public CComplianceAnswer(CComplianceAnswer other)
        {
            ID = other.ID;
            Text = other.Text;
            FirstSet = new List<string>(other.FirstSet);
            SecondSet = new List<string>(other.SecondSet);
            Answer = new List<uint>(other.Answer);
            Time = other.Time;
        }

        /********************************************************************\
        МЕТОД.....: CorrectScore
        ПАРАМЕТРЫ.: string userAnswer - строковое представление
                    пользовательского ответа
                    long time - время, затраченное на вопрос (в секундах)
        ОПИСАНИЕ..: Возвращает балл за задание
        ВОЗВРАТ...: double - балл за задание
        \********************************************************************/
        public double CorrectScore(string userAnswer, long time)
        {
            double score = 1.0;
            List<uint> ParsedAnswer = Parse(userAnswer);
            int errCnt = Math.Abs(Answer.Count - ParsedAnswer.Count);
            int minCnt = Math.Min(Answer.Count, ParsedAnswer.Count);
            for (int i = 0; i < minCnt; i++)
                if (ParsedAnswer[i] != Answer[i])
                    errCnt++;
            score -= errCnt * 0.5;
            return (score < 0.0) ? 0.0 : (score * CalcMultiplier(time));
        }
    }
}
