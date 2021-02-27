using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Model.Models.User;
using Newtonsoft.Json;
using Model.Models.Testing;
using Model.Models.Work;
using Model.Models.Testing.Load;
using Model.Models.Statistics.Testing;
using Model.Models.Works;
using Model.Models.Statistics.Work;

namespace Model.Controllers
{
    public class TestingController : Controller
    {
        private readonly IWorkFinder        m_workFinder;
        private readonly ITestMaker         m_testMaker;
        private readonly ITestResultManager m_testManager;
        private readonly IWorkResultManager m_workManager;
        private readonly ISettingReader     m_settingReader;

        public TestingController(IWorkFinder workFinder, ITestMaker testMaker, ITestResultManager testManager,
            IWorkResultManager workManager, ISettingReader settingReader)
        {
            m_workFinder = workFinder;
            m_testMaker = testMaker;
            m_testManager = testManager;
            m_settingReader = settingReader;
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
        

        public ActionResult LoadTest(int ID)
        {
            CUser user = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
            if (user == null || user.Type != EUserType.UT_STUDENT || m_testManager.GetResult(TestID: ID, UserID: user.ID) != null)
                return RedirectToRoute(new { controller = "Student", action = "WorkList" });
            CWorkData workData = m_workFinder.FindWorkByID(ID);
            foreach (int RequireWorkID in workData.Require)
                if (m_testManager.GetResult(RequireWorkID, user.ID) == null
                    && m_workManager.GetResult(RequireWorkID, user.ID) == null)
                    return RedirectToRoute(new { controller = "Student", action = "WorkList" });
            CTest test = m_testMaker.Make(workData.SettingFile, workData.WorkID);
            CActiveTest active = new CActiveTest(test);
            HttpContext.Session.SetString("Test", JsonConvert.SerializeObject(active, Program.JsonSetting));
            return RedirectToRoute(new { controller = "Testing", action = "Test" });
        }

        [HttpGet]
        public ActionResult Test()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            ViewBag.Active = JsonConvert.DeserializeObject<CActiveTest>(HttpContext.Session.GetString("Test"), Program.JsonSetting);
            return View();
        }

        [HttpPost]
        public ActionResult CheckAnswer(string Answer)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            if (Answer == null)
                Answer = "";
            CActiveTest tmp = JsonConvert.DeserializeObject<CActiveTest>(HttpContext.Session.GetString("Test"), Program.JsonSetting);
            tmp.TestData.Answers[tmp.ActiveBlock][tmp.ActiveTask] = Answer;
            if (tmp.ActiveTask < (tmp.Test.Blocks[tmp.ActiveBlock].Tasks.Count - 1))
                tmp.SetActiveTask(tmp.ActiveBlock, tmp.ActiveTask + 1);
            else if (
                tmp.ActiveBlock < (tmp.Test.Blocks.Count - 1)
                && !tmp.TestData.EndedBlocks.Contains(tmp.Test.Blocks[tmp.ActiveBlock + 1].ID)
                && tmp.Test.Blocks[tmp.ActiveBlock + 1].Access(tmp.TestData.EndedBlocks)
            )
                tmp.SetActiveTask(tmp.ActiveBlock + 1, 0);
            HttpContext.Session.SetString("Test", JsonConvert.SerializeObject(tmp, Program.JsonSetting));
            return RedirectToRoute(new { controller = "Testing", action = "Test" });
        }

        [HttpGet]
        public RedirectToRouteResult ChangeTask(int blockID, int taskID)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            CActiveTest tmp = JsonConvert.DeserializeObject<CActiveTest>(HttpContext.Session.GetString("Test"), Program.JsonSetting);
            if (!tmp.TestData.EndedBlocks.Contains(tmp.Test.Blocks[blockID].ID)
                && tmp.Test.Blocks[blockID].Access(tmp.TestData.EndedBlocks)
                && tmp.SetActiveTask(blockID, taskID))
                HttpContext.Session.SetString("Test", JsonConvert.SerializeObject(tmp, Program.JsonSetting));
            return RedirectToRoute(new { controller = "Testing", action = "Test" });
        }

        [HttpGet]
        public ActionResult EndBlock(int BlockID)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            CActiveTest tmp = JsonConvert.DeserializeObject<CActiveTest>(HttpContext.Session.GetString("Test"), Program.JsonSetting);

            tmp.TestData.EndedBlocks.Add(tmp.Test.Blocks[BlockID].ID);
            if (BlockID == tmp.ActiveBlock)
            {
                int idx = 0;
                while (
                    idx < tmp.Test.Blocks.Count
                    && ( tmp.TestData.EndedBlocks.Contains(tmp.Test.Blocks[idx].ID)
                        || !tmp.Test.Blocks[idx].Access(tmp.TestData.EndedBlocks))
                )
                    idx++;
                if (idx != tmp.Test.Blocks.Count)
                {
                    tmp.SetActiveTask(idx, 0);
                    HttpContext.Session.SetString("Test", JsonConvert.SerializeObject(tmp, Program.JsonSetting));
                    return RedirectToRoute(new { controller = "Testing", action = "Test" });
                }
                else
                {
                    HttpContext.Session.SetString("Test", JsonConvert.SerializeObject(tmp, Program.JsonSetting));
                    return RedirectToRoute(new { controller = "Testing", action = "Result" });
                }
            }

            HttpContext.Session.SetString("Test", JsonConvert.SerializeObject(tmp, Program.JsonSetting));
            return RedirectToRoute(new { controller = "Testing", action = "Test" });
        }

        public ActionResult Result()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            if (HttpContext.Session.GetString("Test") == null)
                return RedirectToRoute(new { controller = "Student", action = "Statist" });
            CActiveTest Test = JsonConvert.DeserializeObject<CActiveTest>(HttpContext.Session.GetString("Test"), Program.JsonSetting);
            CUser UserData = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
            HttpContext.Session.Remove("Test");
            CTestResult result = new CTestResult(UserData.ID, Test);
            m_testManager.Save(result);
            ViewBag.TestResult = result;
            return View();
        }

        public RedirectToRouteResult LoadWork(int ID)
        {
            if (IsNoLog() != null)
                return IsNoLog();
            CWorkData workData = m_workFinder.FindWorkByID(ID);
            HttpContext.Session.SetString("WorkData", JsonConvert.SerializeObject(workData, Program.JsonSetting));
            return RedirectToRoute(new { controller = "Testing", action = "VirtualWork" });
        }

        public ActionResult VirtualWork()
        {
            if (IsNoLog() != null)
                return IsNoLog();
            try
            {
                CUser User = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
                CWorkData Data = JsonConvert.DeserializeObject<CWorkData>(HttpContext.Session.GetString("WorkData"), Program.JsonSetting);
                foreach (int RequireWorkID in Data.Require)
                    if (m_testManager.GetResult(RequireWorkID, User.ID) == null
                        && m_workManager.GetResult(RequireWorkID, User.ID) == null)
                        return RedirectToRoute(new { controller = "Student", action = "WorkList" });
                ViewBag.Module = Data.ModuleName;
                ViewBag.Setting = m_settingReader.LoadSetting(Data.SettingFile);
                return View();
            } catch { }
            return RedirectToRoute(new { controller = "Student", action = "WorkList" });
        }

        [HttpGet]
        public RedirectToRouteResult SaveAnswer(string answer)
        {
            CUser User = JsonConvert.DeserializeObject<CUser>(HttpContext.Session.GetString("UserData"));
            CWorkData Data = JsonConvert.DeserializeObject<CWorkData>(HttpContext.Session.GetString("WorkData"), Program.JsonSetting);
            m_workManager.Save(Data.WorkID, User.ID, answer);
            HttpContext.Session.Remove("WorkData");
            return RedirectToRoute(new { controller = "Student", action = "WorkList" });
        }
    }
}
