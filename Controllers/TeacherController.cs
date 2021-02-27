using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Model.Models.User;
using Model.Models.Work;
using Newtonsoft.Json;
using Model.Models.Statistics.Testing;

namespace Model.Controllers
{
    public class TeacherController : Controller
    {
        private readonly IWorkFinder        m_workFinder;
        private readonly IUserFinder        m_userFinder;
        private readonly ITestResultManager m_resultManager;

        public TeacherController(IUserFinder userFinder, IWorkFinder workFinder, ITestResultManager resultManager)
        {
            m_userFinder = userFinder;
            m_workFinder = workFinder;
            m_resultManager = resultManager;
        }

        private RedirectToRouteResult IsNoLog()
        {
            if (HttpContext.Session.GetString("UserData") != null)
            {
                CUser tmp = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
                // Пользователь авторизован, но не является преподавателем
                if (tmp.Type != EUserType.UT_TEACHER)
                    return RedirectToRoute(new { controller = "Home", action = "NoAccess" });
                // Пользователь авторизован и является преподавателем
                return null;
            }
            // Пользователь не авторизован
            return RedirectToRoute(new { controller = "Home", action = "Login" });
        }

        public ActionResult Journal()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            ViewBag.UserFinder = m_userFinder;
            CUser user = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
            ViewBag.Works = m_workFinder.TeacherWorks(user.ID, m_userFinder, m_workFinder);
            return View();
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
                        return RedirectToRoute(new { controller = "Teacher", action = "Profile" });
                    CFindRecord user = m_userFinder.findByID(int.Parse(ID));
                    if (user != null)
                    {
                        ViewBag.UserData = user.user;
                        ViewBag.ImageWay = "~/ProfileImages/" + user.login + ".png";
                    }
                    else
                        return RedirectToRoute(new { controller = "Teacher", action = "Profile" });
                }
            }
            catch { }
            return View();
        }

        public ActionResult TestDetails(int ID)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            ViewBag.Details = m_resultManager.TestStatistic(ID, m_userFinder);
            return View();
        }

        public ActionResult UserResult(int TestID, int UserID)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            ViewBag.TestResult = m_resultManager.GetResult(TestID, UserID);
            return View();
        }
    }
}
