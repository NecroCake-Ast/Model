using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Model.Models.User;
using Newtonsoft.Json;
using Model.Models.Work;
using Model.Models.Testing.Load;
using Model.Models.Statistics.Testing;
using System.IO;
using Model.Models.Statistics.Work;

namespace Model.Controllers
{
    public class StudentController : Controller
    {
        private readonly IUserFinder        m_userFinder;
        private readonly IWorkFinder        m_workFinder;
        private readonly ITestResultManager m_testManager;
        private readonly IWorkResultManager m_workManager;

        public StudentController(IUserFinder userFinder, IWorkFinder workFinder,
            IWorkResultManager workManager, ITestResultManager testManager)
        {
            m_userFinder = userFinder;
            m_workFinder = workFinder;
            m_testManager = testManager;
            m_workManager = workManager;
        }

        private RedirectToRouteResult IsNoLog()
        {
            if (HttpContext.Session.GetString("UserData") != null)
            {
                CUser tmp = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
                // Пользователь авторизован, но не является обучающимся
                if (tmp.Type != EUserType.UT_STUDENT)
                    return RedirectToRoute(new { controller = "Home", action = "NoAccess" });
                // Пользователь авторизован и является обучающимся
                return null;
            }
            // Пользователь не авторизован
            return RedirectToRoute(new { controller = "Home", action = "Login" });
        }
        
        public ActionResult Profile(string ID)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            try
            {
                if (ID == null)
                {
                    ViewBag.isOwner = true;
                    ViewBag.UserData = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
                    ViewBag.ImageWay = "~/ProfileImages/" + HttpContext.Session.GetString("Login") + ".png";
                }
                else
                {
                    if (int.Parse(ID) == JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData")).ID)
                        return RedirectToRoute(new { controller = "Student", action = "Profile" });
                    CFindRecord user = m_userFinder.findByID(int.Parse(ID));
                    if (user != null)
                    {
                        ViewBag.UserData = user.user;
                        ViewBag.ImageWay = "~/ProfileImages/" + user.login + ".png";
                    }
                    else
                        return RedirectToRoute(new { controller = "Student", action = "Profile" });
                }
            }
            catch { }
            return View();
        }

        public ActionResult NewsWall()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            return View();
        }
        
        public ActionResult WorkList()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            CUser tmp = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
            ViewBag.Works = m_workFinder.StudentWorks(tmp.ID, m_testManager, m_workManager);
            return View();
        }

        public ActionResult Statist()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            CUser tmp = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
            ViewBag.Results = m_testManager.UserStatistic(tmp.ID, m_workFinder);
            return View();
        }
    }
}
