using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Models.Works
{
    public interface ISettingReader
    {
        string LoadSetting(string WorkName);
    }
}
