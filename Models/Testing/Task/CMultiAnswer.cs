using System.Collections.Generic;

namespace Model.Models.Testing.Task
{
    /********************************************************************\
    КЛАСС.....: CMultiAnswer
    НАСЛЕДУЕТ.: ITask
    ОПИСАНИЕ..: Задание с выбором нескольких правильных  вариантов ответа
                Схема оценивания: 1 балл, если выбраны
                все правильные варианты ответа. За каждую ошибку вычитается
                0.5 балла
    ПОЛЯ......: ETaskTypeID Type - ID типа задания
                int ID - ID задания
                string Text - текст вопроса
                HashSet<uint> Answer - правильный ответ
                long Time - ожидаемое время ответа (в секундах)
                List<string> m_variants - варианты ответа
    МЕТОДЫ....: double CalcMultiplier(long time) - возвращает значение 
                коэффициента поправки
                HashSet<uint> Parse(string str) - разбивает входную строку
                на множество целых беззнаковых чисел.
                double CorrectScore(string userAnswer, long time)
                - возвращает балл за задание
    \********************************************************************/
    public class CMultiAnswer : ITask
    {
        public  string        Text     { get; set; }  // Текст вопроса
        public  HashSet<int>  Answer   { get; set; }  // Правильный ответ
        public  long          Time     { get; set; }  // Ожидаемое время ответа (в секундах)
        public  List<string>  Variants { get; set; }  // Варианты ответа
        public  int           ID       { get; set; }  // ID задания
        public string         Correct  { get; set; }  // Правильный ответ
        public ETaskTypeID Type    { get => ETaskTypeID.TID_MULTI; } // Тип задания

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
        ОПИСАНИЕ..: Разбивает входную строку на множество целых беззнаковых 
                    чисел.
        ПАРАМЕТРЫ.: string str - разбиваемая строка
        ВОЗВРАТ...: HashSet<uint> - полученное множество
        \********************************************************************/
        private HashSet<int> Parse(string str)
        {
            HashSet<int> res = new HashSet<int>();
            List<string> values;
            try { values = new List<string>(str.Split(' ')); }
            catch { values = new List<string>(); }
            foreach (var i in values)
                try { res.Add(int.Parse(i)); } catch { }
            return res;
        }

        /******************************\PUBLIC/******************************/

        /********************************************************************\
        КОНСТРУКТОР.: CMultiAnswer
        \********************************************************************/
        public CMultiAnswer()
        {
            Text = "";
            Answer = new HashSet<int>();
            Time = 0;
            Variants = new List<string>();
            ID = 0;
        }

        /********************************************************************\
        КОНСТРУКТОР.: CMultiAnswer
        ПАРАМЕТРЫ...: int id - ID задания
                      string text - текст задания
                      List<string> variants - варианты ответа
                      HashSet<uint> answer - номера правильных вариантов 
                      ответа
                      long time - ожидаемое время ответа  (в секундах)
        \********************************************************************/
        public CMultiAnswer(int id, string text, List<string> variants, HashSet<int> answer, long time)
        {
            ID = id;
            Text = text;
            Answer = new HashSet<int>(answer);
            Time = time;
            Variants = new List<string>(variants);
        }

        /********************************************************************\
        КОНСТРУКТОР.: CMultiAnswer
        ПАРАМЕТРЫ...: CMultiAnswer other - копируемое задание
        \********************************************************************/
        public CMultiAnswer(CMultiAnswer other)
        {
            ID = other.ID;
            Text = other.Text;
            Answer = new HashSet<int>(other.Answer);
            Time = other.Time;
            Variants = new List<string>(other.Variants);
        }

        /********************************************************************\
        МЕТОД.....: CorrectScore
        ОПИСАНИЕ..: Возвращает балл за задание
        ПАРАМЕТРЫ.: string userAnswer - строковое представление
                    пользовательского ответа
                    long time - время, затраченное на вопрос (в секундах)
        ВОЗВРАТ...: double - балл за задание
        \********************************************************************/
        public double CorrectScore(string userAnswer, long time)
        {
            HashSet<int> parsedAnswer = Parse(userAnswer);
            parsedAnswer.SymmetricExceptWith(Answer);
            double score = 1.0 - 0.5 * parsedAnswer.Count;
            return (score < 0.0) ? 0.0 : (score * CalcMultiplier(time));
        }
    }
}
