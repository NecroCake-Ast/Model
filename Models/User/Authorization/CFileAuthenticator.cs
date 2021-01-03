using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace Model.Models.User
{
    /********************************************************************\
    КЛАСС.....: CFileAuthenticator
    НАСЛЕДУЕТ.: IAuthenticator
    ОПИСАНИЕ..: Аутентификатор. Проверка подлинности логина
                и пароля производится на основе файла
    ПОЛЯ......: string m_fileWay - путь до папки с данными о пользователях
    МЕТОДЫ....: CUser UserWorks(CAuthorizationData data) - возвращает
                данные о пользователе по информации для входа
                public bool Connect() - проверка на существование файла
                с данными
    \********************************************************************/
    public class CFileAuthenticator : IAuthenticator
    {
        private string m_fileWay;
        public CFileAuthenticator(string fileName)
        {
            m_fileWay = fileName;
        }

        /********************************************************************\
        МЕТОД.....: UserWorks
        ОПИСАНИЕ..: Возвращает данные о пользователе по информации для входа
        ПАРАМЕТРЫ.: CAuthorizationData data - данные для входа
        ВОЗВРАЩАЕТ: CUser - данные о пользователе
        ПРИМЕЧАНИЕ: Если в системе не зарегистрирован пользователь с
                    указанным логином и паролем, то возвращается null
        \********************************************************************/
        public CUser Find(CAuthorizationData data)
        {
            try
            {
                PhysicalFileProvider provider = new PhysicalFileProvider(m_fileWay);
                if(provider.GetFileInfo(data.Login + ".txt").Exists)
                {
                    CUser result = null;
                    StreamReader reader = new StreamReader(m_fileWay + "/" + data.Login + ".txt");
                    string tmp = reader.ReadLine();
                    if (tmp == data.Password)
                    {
                        tmp = reader.ReadLine();
                        result = JsonConvert.DeserializeObject<CUser>(tmp);
                    }
                    reader.Close();
                    return result;
                }
            } catch { }
            return null;
        }

        /********************************************************************\
        МЕТОД.....: Handshake
        ОПИСАНИЕ..: Проверка доступа к папке с данными пользователей
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
