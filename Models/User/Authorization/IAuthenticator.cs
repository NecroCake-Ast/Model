using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.User
{
    public interface IAuthenticator
    {
        CUser Find(CAuthorizationData data);
        bool Handshake();
    }
}
