using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Model.Models.User
{
    /********************************************************************\
    КЛАСС.....: CFileUserFinder
    НАСЛЕДУЕТ.: IUserFinder
    ОПИСАНИЕ..: Выполняет поиск пользователей. Данные о пользователях
                должны храниться в файлах
    ПОЛЯ......: string m_storagePath - файл с данными о пользователях
    МЕТОДЫ....: List<CUser> getAllUser() - возвращает данные всех
                зарегистрированных пользователей.
                CUser findByID(int ID) - возвращает данные пользователя с
                заданным ID.
                CUser findByLogin(string login) - возвращает данные 
                пользователя с заданным логином.
    \********************************************************************/
    class CFileUserFinder : IUserFinder
    {
        private string m_storagePath; // Путь до папки с данными о пользователях

        /********************************************************************\
        КОНСТРУКТОР.: CFileUserFinder
        ПАРАМЕТРЫ...: string fileName - путь до папки с данными 
                      о пользователях
        \********************************************************************/
        public CFileUserFinder(string storagepath)
        {
            m_storagePath = storagepath;
        }

        /********************************************************************\
        МЕТОД.....: getAllUser
        ОПИСАНИЕ..: Возвращает данные всех зарегистрированных пользователей
        ВОЗВРАЩАЕТ: List<CFindRecord>  - данные всех пользователей
        \********************************************************************/
        public List<CFindRecord> getAllUser()
        {
            try
            {
                PhysicalFileProvider provider = new PhysicalFileProvider(m_storagePath);
                List<IFileInfo> info = provider.GetDirectoryContents(string.Empty).ToList<IFileInfo>();
                List<CFindRecord> result = new List<CFindRecord>();
                for(int i = 0; i < info.Count; i++)
                {
                    StreamReader reader = new StreamReader(m_storagePath + "/" + info[i].Name);
                    if (reader != null)
                    {
                        reader.ReadLine();
                        result.Add(new CFindRecord()
                        {
                            login = info[i].Name.Substring(0, info[i].Name.IndexOf(".")),
                            user = JsonConvert.DeserializeObject<CUser>(reader.ReadLine())
                        });
                    }
                    reader.Close();
                }
                return result;
            }
            catch { }
            return new List<CFindRecord>();
        }

        /********************************************************************\
        МЕТОД.....: findByID
        ОПИСАНИЕ..: Возвращает данные всех зарегистрированных пользователей
        ПАРАМЕТРЫ.: int ID - ID искомого пользователя
        ВОЗВРАЩАЕТ: CFindRecord - Данные о пользователе с искомым ID
        ПРИМЕЧАНИЕ: Если в системе не зарегистрирован пользователь с
                    указанным логином, то возвращается null
        \********************************************************************/
        public CFindRecord findByID(int ID)
        {
            try
            {
                PhysicalFileProvider provider = new PhysicalFileProvider(m_storagePath);
                List<IFileInfo> info = provider.GetDirectoryContents(string.Empty).ToList<IFileInfo>();
                for (int i = 0; i < info.Count; i++)
                {
                    StreamReader reader = new StreamReader(m_storagePath + "/" + info[i].Name);
                    reader.ReadLine();
                    CFindRecord tmp = new CFindRecord()
                    {
                        login = info[i].Name.Substring(0, info[i].Name.IndexOf(".")),
                        user = JsonConvert.DeserializeObject<CUser>(reader.ReadLine())
                    };
                    reader.Close();
                    if (tmp.user.ID == ID)
                        return tmp;
                }
            } catch { }
            return null;
        }

        /********************************************************************\
        МЕТОД.....: findByLogin
        ОПИСАНИЕ..: Возвращает данные о пользователе с указанным логином
        ПАРАМЕТРЫ.: string login - искомый логин
        ВОЗВРАЩАЕТ: CFindRecord - данные о пользователе
        ПРИМЕЧАНИЕ: Если в системе не зарегистрирован пользователь с
                    указанным логином, то возвращается null
        \********************************************************************/
        public CFindRecord findByLogin(string login)
        {
            try
            {
                CFindRecord result = new CFindRecord();
                StreamReader reader = new StreamReader(m_storagePath + "/" + login + ".txt");
                reader.ReadLine();
                result = new CFindRecord()
                {
                    login = login, user = JsonConvert.DeserializeObject<CUser>(reader.ReadLine())
                };
                reader.Close();
                return result;
            }
            catch { }
            return null;
        }
    }
}
