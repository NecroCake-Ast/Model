using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Testing.Task
{
    /********************************************************************\
    КЛАСС.....: CSequenceAnswer
    НАСЛЕДУЕТ.: ITask
    ОПИСАНИЕ..: Интерфейс заданий различного типа
    ПОЛЯ......: ETaskTypeID Type - ID типа задания
                int ID - ID задания
                string Text - текст вопроса
                uint Answer - правильный ответ
                long Time - ожидаемое время ответа (в секундах)
                List<string> Set - неупорядоченная последовательность
    МЕТОДЫ....: double CalcMultiplier(long time) - возвращает
                значение коэффициента поправки
                List<uint> Parse(string str) - разбивает входную строку
                на список целых беззнаковых чисел
                bool isUnique(List<uint> list, uint val) - проверка
                значения на уникальность
                double CorrectScore(string userAnswer, long time)
                - возвращает балл за задание
    \********************************************************************/
    public class CSequenceAnswer : ITask
    {
        public string       Text    { get; set; }  //!< Текст вопроса
        public List<uint>   Answer  { get; set; }  //!< Правильный ответ
        public long         Time    { get; set; }  //!< Ожидаемое время ответа (в секундах)
        public List<string> Set     { get; set; }  //!< Неупорядоченное множество
        public int          ID      { get; set; }  //!< ID задания
        public string       Correct { get; set; }  //!< Правильный ответ

        public ETaskTypeID  Type    { get => ETaskTypeID.TID_SEQUENCE; } //!< Тип задания

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

        /********************************************************************\
        МЕТОД.....: isUnique
        ОПИСАНИЕ..: Проверка значения на уникальность - возвращает true, если
                    значение входит в список не более одного раза
        ПАРАМЕТРЫ.: List<uint> list - список значений
                    uint val - искомое значение
        ВОЗВРАТ...: bool - является ли уникальным
        ПРИМЕЧАНИЕ: Значения, отсутствующие в списке, считаются уникальными
        \********************************************************************/
        private bool isUnique(List<uint> list, uint val)
        {
            uint cnt = 0;
            for (int i = 0; i < list.Count; i++)
                if (list[i] == val)
                    cnt++;
            return (cnt < 2);
        }

        /******************************\PUBLIC/******************************/

        public CSequenceAnswer()
        {
            Text = "";
            Answer = new List<uint>();
            Time = 0;
            Set = new List<string>();
            ID = 0;
            Correct = "";
        }

        /********************************************************************\
        КОНСТРУКТОР.: CSequenceAnswer
        ПАРАМЕТРЫ...: int id - ID задания
                      string text - текст задания
                      List<string> set - неупорядоченная последовательность
                      List<uint> answer - правильная последовательность
                      long time - ожидаемое время ответа (в секундах)
        \********************************************************************/
        public CSequenceAnswer(int id, string text, List<string> set, List<uint> answer, long time)
        {
            ID = id;
            Text = text;
            Set = new List<string>(set);
            Answer = new List<uint>(answer);
            Time = time;
        }

        /********************************************************************\
        КОНСТРУКТОР.: CSequenceAnswer
        ПАРАМЕТРЫ...: CSequenceAnswer other - копируемое задание
        \********************************************************************/
        public CSequenceAnswer(CSequenceAnswer other)
        {
            ID = other.ID;
            Text = other.Text;
            Set = new List<string>(other.Set);
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
                if (ParsedAnswer[i] != Answer[i] || !isUnique(ParsedAnswer, ParsedAnswer[i]))
                    errCnt++;
            score -= errCnt * 0.5;
            return (score < 0.0) ? 0.0 : (score * CalcMultiplier(time));
        }
    }
}
