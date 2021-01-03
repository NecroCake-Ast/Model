using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using Microsoft;
using Model.Models.User;
using Newtonsoft.Json;
using System;

namespace Model.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserFinder      m_finder;
        private readonly IUserRegistrator m_registrator;

        public AdminController(IUserFinder finder, IUserRegistrator registrator)
        {
            m_finder = finder;
            m_registrator = registrator;
        }

        private RedirectToRouteResult IsNoLog()
        {
            if (HttpContext.Session.GetString("UserData") != null)
            {
                CUser tmp = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
                // Пользователь авторизован, но не является администратором
                if (tmp.Type != EUserType.UT_ADMIN)
                    return RedirectToRoute(new { controller = "Home", action = "NoAccess" });
                // Пользователь авторизован и является администратором
                return null;
            }
            // Пользователь не авторизован
            return RedirectToRoute(new { controller = "Home", action = "Login" });
        }

        public ActionResult UserList()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            List<CFindRecord> users = m_finder.getAllUser();
            users.Sort(CRecordSorter.CompareByID);
            ViewBag.UserList = users;
            return View();
        }
        
        [HttpGet]
        public ActionResult Registration()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            return View();
        }

        [HttpPost]
        public ActionResult Registration(CRegistrationData data)
        {
            if(m_registrator.tryRegistrate(data))
                return RedirectToRoute(new { controller = "Admin", action = "UserList" });
            return RedirectToRoute(new { controller = "Admin", action = "Registration" });
        }

        public RedirectToRouteResult RemoveUser(string login)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            CFindRecord user = m_finder.findByLogin(login);
            if (user != null && user.user.Type != EUserType.UT_ADMIN)
               System.IO.File.Delete(HomeController.AuthorizationFilesPath + "/" + login + ".txt");
            return RedirectToRoute(new { controller = "Admin", action = "UserList" });
        }
    }
}
