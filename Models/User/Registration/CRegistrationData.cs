using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.User
{
    public class CRegistrationData
    {
        public string    Login      { get; set; }
        public string    Password   { get; set; }
        public string    FirstName  { get; set; }
        public string    SecondName { get; set; }
        public EUserType Type       { get; set; }
    }
}
