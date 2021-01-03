using System.Collections.Generic;

namespace Model.Models.Work
{
    /********************************************************************\
    КЛАСС.....: CWorkData
    ОПИСАНИЕ..: Информация о работе.
    ПОЛЯ......: EWorkType Type - тип работы
                string Name - имя пользователя
                int WorkID - ID работы
                int TeacherID - ID преподавателя
                List<int> StudentID - обучающиеся, которым выдано задание
                List<int> Require - работы, требуемые для допуска к данной
                string SettingFile - название файла с данными о работе
    \********************************************************************/
    public class CWorkData
    {
        public EWorkType Type        { get; set; } // Тип работы
        public string    Name        { get; set; } // Название работы
        public string    ModuleName  { get; set; } // Название модуля (если требуется)
        public int       WorkID      { get; set; } // ID работы
        public int       TeacherID   { get; set; } // ID преподавателя
        public List<int> StudentID   { get; set; } // Обучающиеся
        public List<int> Require     { get; set; } // Требования
        public string    SettingFile { get; set; } // Файл с данными о работе
    }
}
