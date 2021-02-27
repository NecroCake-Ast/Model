using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using Model.Models.User;
using Newtonsoft.Json;

namespace Model.Controllers
{
    public class HomeController : Controller
    {
        private IAuthenticator m_authenticator;

        public static string AuthorizationFilesPath = Directory.GetCurrentDirectory() + "/wwwroot/Users";

        public HomeController(IAuthenticator authenticator)
        {
            m_authenticator = authenticator;
        }

        public RedirectResult Exit()
        {
            HttpContext.Session.Clear();
            return Redirect("Login");
        }

        public ViewResult NoAccess()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (HttpContext.Session.GetString("login") != null)
            {
                CUser tmp = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
                switch(tmp.Type)
                {
                    case EUserType.UT_ADMIN:   return RedirectToRoute(new{ controller = "Admin",   action = "UserList" });
                    case EUserType.UT_STUDENT: return RedirectToRoute(new{ controller = "Student", action = "WorkList" });
                    case EUserType.UT_TEACHER: return RedirectToRoute(new{ controller = "Teacher", action = "Journal" });
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(CAuthorizationData ReadedData)
        {
            if (m_authenticator.Handshake())
            {
                CUser UserData = m_authenticator.Find(ReadedData);
                if (UserData != null)
                {
                    HttpContext.Session.SetString("UserData", JsonConvert.SerializeObject(UserData));
                    HttpContext.Session.SetString("Login", ReadedData.Login);
                    switch (UserData.Type)
                    {
                        case EUserType.UT_ADMIN:   return RedirectToRoute(new{ controller = "Admin",   action = "UserList" });
                        case EUserType.UT_STUDENT: return RedirectToRoute(new{ controller = "Student", action = "WorkList" });
                        case EUserType.UT_TEACHER: return RedirectToRoute(new{ controller = "Teacher", action = "Journal" });
                    }
                }
            }
            return View();
        }
    }
}
