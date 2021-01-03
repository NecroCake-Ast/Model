using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Model.Models.Statistics.Testing;
using Model.Models.Statistics.Work;
using Model.Models.User;
using Newtonsoft.Json;

namespace Model.Models.Work
{
    /********************************************************************\
    КЛАСС.....: CFileWorkFinder
    НАСЛЕДУЕТ.: IWorkFinder
    ОПИСАНИЕ..: Класс, занимающийся поиском данных о выданных работах.
                Данные хранятся в текстовых файлах.
    ПОЛЯ......: string m_fileWay - путь до папки с данными о работах
    МЕТОДЫ....: List<CWorkData> UserWorks() - возвращает список выданных работ
                public bool Connect() - проверка на существование файла
                с данными
    \********************************************************************/
    public class CFileWorkFinder : IWorkFinder
    {
        private readonly string m_fileWay;
        public CFileWorkFinder(string fileName)
        {
            m_fileWay = fileName;
        }

        /********************************************************************\
        МЕТОД.....: UserWorks
        ОПИСАНИЕ..: Возвращает данные о работах, выданных обучающемуся с
                    указанным id.
        ПАРАМЕТРЫ.: int ID - ID обучающегося.
                    ITestResultManager manager - менеджер результатов тестов
        ВОЗВРАЩАЕТ: List<CWorkData> - данные о выданных работах.
        \********************************************************************/
        public List<CWorkData> StudentWorks(int StudentID, ITestResultManager TestManager, IWorkResultManager WorkManager)
        {
            List<CWorkData> data = new List<CWorkData>();
            try
            {
                var Files = Directory.EnumerateFiles(m_fileWay);
                foreach (var curfile in Files)
                {
                    StreamReader reader = new StreamReader(curfile);
                    string tmp = reader.ReadLine();
                    reader.Close();
                    CWorkData workInfo = JsonConvert.DeserializeObject<CWorkData>(tmp, Program.JsonSetting);
                    if (workInfo.StudentID.Contains(StudentID) && TestManager.GetResult(workInfo.WorkID, StudentID) == null
                        && WorkManager.GetResult(workInfo.WorkID, StudentID) == null)
                    {
                        bool access = true;
                        for(int i = 0; i < workInfo.Require.Count; i++)
                            if (TestManager.GetResult(workInfo.Require[i], StudentID) == null
                                && WorkManager.GetResult(workInfo.Require[i], StudentID) == null)
                            {
                                access = false;
                                break;
                            }
                        if (access)
                            data.Add(workInfo);
                    }
                }
            } catch { }
            return data;
        }

        /********************************************************************\
        МЕТОД.....: TeacherWorks
        ОПИСАНИЕ..: Возвращает данные о работах, выданных преподавателем с
                    указанным id.
        ПАРАМЕТРЫ.: int ID - ID преподавателя.
        ВОЗВРАЩАЕТ: List<CWorkData> - данные о выданных работах.
        \********************************************************************/
        public List<CDetailWorkData> TeacherWorks(int TeacherID, IUserFinder userFinder, IWorkFinder workFinder)
        {
            List<CDetailWorkData> data = new List<CDetailWorkData>();
            try
            {
                var Files = Directory.EnumerateFiles(m_fileWay);
                foreach (var curfile in Files)
                {
                    StreamReader reader = new StreamReader(curfile);
                    string tmp = reader.ReadLine();
                    reader.Close();
                    CWorkData workInfo = JsonConvert.DeserializeObject<CWorkData>(tmp, Program.JsonSetting);
                    if (workInfo.TeacherID == TeacherID)
                        data.Add(new CDetailWorkData(workInfo, userFinder, workFinder));
                }
            }
            catch { }
            return data;
        }

        /********************************************************************\
        МЕТОД.....: FindWorkByID
        ОПИСАНИЕ..: Возвращает данные о работе с заданным ID.
        ПАРАМЕТРЫ.: int WorkID - ID работы.
        ВОЗВРАЩАЕТ: CWorkData - данные о выданной работе.
        \********************************************************************/
        public CWorkData FindWorkByID(int WorkID)
        {
            try
            {
                var Files = Directory.EnumerateFiles(m_fileWay);
                foreach (var curfile in Files)
                {
                    StreamReader reader = new StreamReader(curfile);
                    string tmp = reader.ReadLine();
                    reader.Close();
                    CWorkData workInfo = JsonConvert.DeserializeObject<CWorkData>(tmp, Program.JsonSetting);
                    if (workInfo.WorkID == WorkID)
                        return workInfo;
                }
            }
            catch { }
            return null;
        }

        /********************************************************************\
        МЕТОД.....: Handshake
        ОПИСАНИЕ..: Проверка доступа к папке с данными о работах
        ПАРАМЕТРЫ.: нет
        ВОЗВРАЩАЕТ: bool - существует ли файл
        \********************************************************************/
        public bool Handshake()
        {
            try
            {
                PhysicalFileProvider provider = new PhysicalFileProvider(m_fileWay);
                return provider != null;
            } catch { }
            return false;
        }
    }
}
