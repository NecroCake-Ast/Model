using System.Collections.Generic;

namespace Model.Models.User
{
    /********************************************************************\
    КЛАСС.....: CUser
    ОПИСАНИЕ..: Информация о пользователе.
    ПОЛЯ......: EUserType Type - тип пользователя
                string FirstName - имя пользователя
                string SecondName - фамилия пользователя
                int ID - ID пользователя
    \********************************************************************/
    public class CUser
    {
        public EUserType Type       { get; set; } // Тип пользователя
        public string    FirstName  { get; set; } // Имя
        public string    SecondName { get; set; } // Фамилия
        public int       ID         { get; set; } // ID
    }
}
