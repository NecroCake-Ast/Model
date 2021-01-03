using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.User
{
    /********************************************************************\
    КЛАСС.....: CAuthorizationData
    ОПИСАНИЕ..: Логин и пароль пользователя
    ПОЛЯ......: string Login - логин пользователя
                string Password - пароль пользователя
    \********************************************************************/
    public class CAuthorizationData
    {
        public string Login    { get; set; } // Логин пользователя
        public string Password { get; set; } // Пароль пользователя
    }
}
