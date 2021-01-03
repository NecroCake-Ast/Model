using Newtonsoft.Json;

namespace Model.Models.Testing.Task
{
    /********************************************************************\
    КЛАСС.....: COpenAnswer
    НАСЛЕДУЕТ.: ITask
    ОПИСАНИЕ..: Задание открытого типа
    ПОЛЯ......: ETaskTypeID Type - ID типа задания
                int ID - ID задания
                string Text - текст вопроса
                string Answer - правильный ответ
                long Time - ожидаемое время ответа (в секундах)
    МЕТОДЫ....: string Normalize(string str) - приводит строку к
                нормальному виду (удаление пробелов в начале и конце
                строки, лишних пробелов между словами, приведение к
                нижнему регистру)
                double CalcMultiplier(long time) - возвращает
                значение коэффициента поправки
                double CorrectScore(string userAnswer, long time)
                - возвращает балл за задание
    \********************************************************************/
    public class COpenAnswer : ITask
    {
        private string     m_Answer;
        public  string     Text    { get; set; }   // Текст вопроса
        public  long       Time    { get; set; }   // Ожидаемое время ответа (в секундах)
        public  int        ID      { get; set; }   // ID задания
        public string      Correct { get => m_Answer; }            // Правильный ответ
        public ETaskTypeID Type    { get => ETaskTypeID.TID_OPEN; } // Тип задания
        public string Answer { get => m_Answer; set => m_Answer = Normalize(value); }  // Правильный ответ

        /******************************\PRIVATE/******************************/

        /********************************************************************\
        МЕТОД.....: Normalize
        ОПИСАНИЕ..: Нормализация строки
        ПАРАМЕТРЫ.: long time - затраченное время
        ВОЗВРАТ...: double - коэффициент поправки
        \********************************************************************/
        private string Normalize(string str)
        {
            try
            {
                str = str.Trim();
                for (int pos = 0; pos < str.Length; pos++)
                    if (str[pos] == ' ')
                    {
                        int lastSpace = pos + 1;
                        while (lastSpace < str.Length && str[lastSpace] == ' ')
                            lastSpace++;
                        str = str.Substring(0, pos + 1) + str.Substring(lastSpace);
                    }
                str = str.ToLower();
            } catch { }
            return str;
        }

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

        /********************************************************************\
        КОНСТРУКТОР.: COpenAnswer
        \********************************************************************/
        public COpenAnswer()
        {
            ID = 0;
            Text = "";
            Answer = "";
            Time = 0;
        }

        [JsonConstructor]
        public COpenAnswer(string text, string answer, long time, int id)
        {
            Text = text;
            Answer = answer;
            Time = time;
            ID = id;
        }

        /********************************************************************\
        КОНСТРУКТОР.: COpenAnswer
        ПАРАМЕТРЫ...: int id - ID задания
                      string text - текст задания
                      string answer - правильный ответ
                      long time - ожидаемое время ответа (в секундах)
        \********************************************************************/
        public COpenAnswer(int id, string text, string answer, long time)
        {
            ID = id;
            Text = text;
            Answer = Normalize(answer);
            Time = time;
        }

        /********************************************************************\
        КОНСТРУКТОР.: COpenAnswer
        ПАРАМЕТРЫ...: COpenAnswer other - копируемое задание
        \********************************************************************/
        public COpenAnswer(COpenAnswer other)
        {
            ID = other.ID;
            Text = other.Text;
            Answer = other.Answer;
            Time = other.Time;
        }

        /********************************************************************\
        МЕТОД.....: CorrectScore
        ОПИСАНИЕ..: Возвращает балл за задание
        ПАРАМЕТРЫ.: string userAnswer - строковое представление
                    пользовательского ответа
                    long time - время, затраченное на вопрос
                    (в секундах)
        ВОЗВРАТ...: double - балл за задание
        \********************************************************************/
        public double CorrectScore(string userAnswer, long time)
        {
            userAnswer = Normalize(userAnswer);
            double score = 0.0;
            if (userAnswer == Answer)
                score = 1.0;
            return score * CalcMultiplier(time);
        }
    }
}
