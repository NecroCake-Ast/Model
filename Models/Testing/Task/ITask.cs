using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Testing.Task
{
    /*******************************************************\
    ИНТЕРФЕЙС.: ITask
    ОПИСАНИЕ..: Интерфейс заданий различного типа
    ПОЛЯ......: ETaskTypeID Type - ID типа задания
                int ID - ID задания
    МЕТОДЫ....: double CorrectScore(string userAnswer,
                long time) - возвращает балл за задание
    \*******************************************************/
    public interface ITask
    {
        ETaskTypeID Type    { get; }      // Тип задания
        int         ID      { get; set; } // ID задания
        long        Time    { get; set; } // Ожидаемое время ответа (в секундах)
        string      Correct { get; }      // Правильный ответ

        /*******************************************************\
        МЕТОД.....: CorrectScore
        ПАРАМЕТРЫ.: string userAnswer - строковое представление
                    пользовательского ответа
                    long time - время, затраченное на вопрос
                    (в секундах)
        ОПИСАНИЕ..: Возвращает балл за задание
        ВОЗВРАТ...: double - балл за задание
        \*******************************************************/
        double CorrectScore(string userAnswer, long time);
    }
}
