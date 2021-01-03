using Model.Models.Statistics.Testing;
using Model.Models.Statistics.Work;
using Model.Models.User;
using System.Collections.Generic;

namespace Model.Models.Work
{
    public interface IWorkFinder
    {
        List<CWorkData> StudentWorks(int StudentID, ITestResultManager TestManager, IWorkResultManager WorkManager);
        List<CDetailWorkData> TeacherWorks(int TeacherID, IUserFinder userFinder, IWorkFinder workFinder);
        CWorkData FindWorkByID(int WorkID);
        bool Handshake();
    }
}
