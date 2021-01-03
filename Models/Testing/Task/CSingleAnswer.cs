using Newtonsoft.Json;
using System.Collections.Generic;

namespace Model.Models.Testing.Task
{
    /********************************************************************\
    КЛАСС.....: CSingleAnswer
    НАСЛЕДУЕТ.: ITask
    ОПИСАНИЕ..: Реализует задания закрытого типа с 1 ответом
                Схема оценивания: 1 балл, если выбран правильный вариант 
                ответа.
    ПОЛЯ......: ETaskTypeID TypeID - ID типа задания
                string Text - текст вопроса
                uint Answer - правильный ответ
                long Time - ожидаемое время ответа (в секундах)
                List<string> m_variants - варианты ответа
    МЕТОДЫ....: double CalcMultiplier(long time) - возвращает значение 
                коэффициента поправки
                double CorrectScore(string userAnswer, long time)
                - возвращает балл за задание
    \********************************************************************/
    public class CSingleAnswer : ITask
    {
        public string        Text     { get; set; }  // Текст вопроса
        public uint          Answer   { get; set; }  // Индекс правильного ответа
        public long          Time     { get; set; }  // Ожидаемое время ответа (в секундах)
        public List<string>  Variants { get; set; }  // Варианты ответа
        public int           ID       { get; set; }  // ID задания
        public string        Correct  { get; set; }  // Правильный ответ
        public ETaskTypeID   Type     { get => ETaskTypeID.TID_SINGLE; } // Тип задания

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

        /******************************\PUBLIC/******************************/

        public CSingleAnswer() { Text = ""; Answer = 0; Time = 0; Variants = null; ID = 0; }

        /********************************************************************\
        КОНСТРУКТОР.: CSingleAnswer
        ПАРАМЕТРЫ...: int id - ID задания
                      string text - текст задания
                      List<string> variants - варианты ответа
                      uint answer - номер правильного ответа
                      long time - ожидаемое время ответа (в секундах)
        \********************************************************************/
        public CSingleAnswer(int id, string text, List<string> variants, uint answer, long time, string correct)
        {
            ID = id;
            Text = text;
            Answer = answer;
            Time = time;
            Variants = variants;
            Correct = correct;
        }

        /********************************************************************\
        КОНСТРУКТОР.: CSingleAnswer
        ПАРАМЕТРЫ...: string other - копируемое задание
        \********************************************************************/
        public CSingleAnswer(CSingleAnswer other)
        {
            ID = other.ID;
            Text = other.Text;
            Answer = other.Answer;
            Time = other.Time;
            Variants = new List<string>(other.Variants);
        }

        /********************************************************************\
        МЕТОД.....: CorrectScore
        ПАРАМЕТРЫ.: string userAnswer - строковое представление
                    пользовательского ответа
                    long time - время, затраченное на вопрос (в секундах)
        ОПИСАНИЕ..: Возвращает балл за задание
        ВОЗВРАТ...: double - балл за задание
        ПРИМЕЧАНИЕ: Предполагается, что строка ответа состоит из 1 числа,
                    например "1" или "10", которое соответствует индексу 
                    правильного ответа.
        \********************************************************************/
        public double CorrectScore(string userAnswer, long time)
        {
            double score = 0.0;
            int tmp;
            if (int.TryParse(userAnswer, out tmp) && (tmp == Answer))
                score = 1.0;
            return score * CalcMultiplier(time);
        }
    }
}
