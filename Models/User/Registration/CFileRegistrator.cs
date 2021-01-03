using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.User
{
    /********************************************************************\
    КЛАСС.....: CFileRegistrator
    НАСЛЕДУЕТ.: IUserRegistrator
    ОПИСАНИЕ..: Регистратор. Сохраняет данные о пользователе в текстовом
                файле.
    ПОЛЯ......: string m_userFilesWay - путь до папки с данными о 
                пользователях
                string m_IDFileWay - путь до файла сохранения ID
    МЕТОДЫ....: int getNewID() - генерирует новый ID
                bool tryRegistrate(CRegistrationData data) - регистрирует
                нового пользователя с указанными данными
                bool Handshake() - проверка доступа к месту хранения
                данных о пользовател
    \********************************************************************/
    public class CFileRegistrator : IUserRegistrator
    {
        private string m_userFilesWay; // Путь до папки с данными о пользователях
        private string m_IDFileWay;    // Путь до папки сохранения ID

        /********************************************************************\
        КОНСТРУКТОР.: CFileRegistrator
        ПАРАМЕТРЫ...: string userFileWay - путь до папки с данными 
                      о пользователях
                      string idFileWay - путь до папки с ID
        \********************************************************************/
        public CFileRegistrator(string userFileWay, string idFileWay)
        {
            m_userFilesWay = userFileWay;
            m_IDFileWay = idFileWay;
        }

        /********************************************************************\
        МЕТОД.....: getNewID
        ОПИСАНИЕ..: Генерирует новый ID для регестрируемого пользователя
        ВОЗВРАЩАЕТ: int - новый ID
        ПРИМЕЧАНИЕ: Автоматически записывает следующий ID по адресу idFileWay 
        \********************************************************************/
        private int getNewID()
        {
            StreamReader get = new StreamReader(m_IDFileWay);
            int id = int.Parse(get.ReadLine());
            get.Close();
            StreamWriter update = new StreamWriter(m_IDFileWay);
            update.WriteLine(id + 1);
            update.Close();
            return id;
        }

        /********************************************************************\
        МЕТОД.....: tryRegistrate
        ОПИСАНИЕ..: Регистрирует нового пользователя с указанными данными
        ВОЗВРАЩАЕТ: bool - удалось ли зарегистрировать пользователя
        \********************************************************************/
        public bool tryRegistrate(CRegistrationData data)
        {
            try
            {
                CFileUserFinder finder = new CFileUserFinder(m_userFilesWay);
                if (data.Login != null && data.Password != null
                    && data.FirstName != null && data.SecondName != null
                    && data.Type != EUserType.UT_ADMIN && finder.findByLogin(data.Login) == null)
                {
                    StreamWriter writer = new StreamWriter(m_userFilesWay + "/" + data.Login + ".txt");
                    if (writer.BaseStream != null)
                    {
                        writer.WriteLine(data.Password);
                        CUser user = new CUser()
                        {
                            ID = getNewID(),
                            FirstName = data.FirstName,
                            SecondName = data.SecondName,
                            Type = data.Type
                        };
                        writer.WriteLine(JsonConvert.SerializeObject(user));
                        writer.Close();
                        return true;
                    }
                }
            } catch { }
            return false;
        }

        /********************************************************************\
        МЕТОД.....: Handshake
        ОПИСАНИЕ..: Проверка доступа к папке с данными пользователей
        ВОЗВРАЩАЕТ: bool - существует ли файл
        \********************************************************************/
        public bool Handshake()
        {
            try
            {
                PhysicalFileProvider UserData = new PhysicalFileProvider(m_userFilesWay);
                PhysicalFileProvider IDData = new PhysicalFileProvider(m_IDFileWay);
                return UserData != null && IDData != null;
            } catch { }
            return false;
        }
    }
}
