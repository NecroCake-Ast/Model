using Model.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Work
{
    public class WorkRef
    {
        public string WorkName { get; set; }   // Название работы
        public int    WorkID   { get; set; }   // ID работы

        public WorkRef(string workName, int workID)
        {
            WorkName = workName;
            WorkID = workID;
        }
    }

    public class CDetailWorkData
    {
        public EWorkType     Type        { get; set; } // Тип работы
        public string        Name        { get; set; } // Название работы
        public string        ModuleName  { get; set; } // Название модуля (если требуется)
        public int           WorkID      { get; set; } // ID работы
        public int           TeacherID   { get; set; } // ID преподавателя
        public List<CUser>   StudentID   { get; set; } // Обучающиеся
        public List<WorkRef> Require     { get; set; } // Требования
        public string        SettingFile { get; set; } // Файл с данными о работе

        public CDetailWorkData(CWorkData data, IUserFinder userFinder, IWorkFinder workFinder)
        {
            Type = data.Type;
            Name = data.Name;
            ModuleName = data.ModuleName;
            WorkID = data.WorkID;
            TeacherID = data.TeacherID;
            SettingFile = data.SettingFile;
            StudentID = new List<CUser>();
            Require = new List<WorkRef>();

            foreach (int userID in data.StudentID)
                StudentID.Add(userFinder.findByID(userID).user);
            foreach (int WorkID in data.Require)
            {
                CWorkData workData = workFinder.FindWorkByID(WorkID);
                Require.Add(new WorkRef(workData.Name, workData.WorkID));
            }
        }
    }
}
