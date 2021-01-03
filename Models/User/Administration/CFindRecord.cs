using System.Collections.Generic;

namespace Model.Models.User
{
    /********************************************************************\
    КЛАСС.....: CFindRecord
    ОПИСАНИЕ..: Логин и данные пользователя
    ПОЛЯ......: string login - логин пользователя
                CUser user - данные о пользователе
    \********************************************************************/
    public class CFindRecord
    {
        public string login { get; set; } // Логин пользователя
        public CUser  user  { get; set; } // Данные о пользователе
    }
}
