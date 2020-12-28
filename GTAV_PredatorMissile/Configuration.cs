using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ini;

class Configuration
{
    public static IniFile ini = new IniFile("scripts\\GTAV_PredatorMissile.ini");

    private static void WriteValue(string section, string key, string value)
    {
        ini.IniWriteValue(section, key, value);
    }

    private static string ReadValue(string section, string key)
    {
        return ini.IniReadValue(section, key);
    }

    public static T GetConfigSetting<T>(string section, string key)
    {
        System.Type type = typeof(T);

        if (type == typeof(bool))
        {
            object setting = Convert.ToBoolean(ReadValue(section, key));
            return (T)setting;
        }
        else if (type == typeof(int))
        {
            object setting = Convert.ToInt32(ReadValue(section, key));
            return (T)setting;
        }
        else if (type == typeof(uint))
        {
            object setting = Convert.ToUInt32(ReadValue(section, key));
            return (T)setting;
        }
        else if (type == typeof(string))
        {
            object setting = ReadValue(section, key);
            return (T)setting;
        }
        else if (type == typeof(double))
        {
            object setting = Convert.ToDouble(ReadValue(section, key));
            return (T)setting;
        }

        else if (type == typeof(Keys))
        {
            object setting = Enum.Parse(typeof(Keys), ReadValue(section, key), true);
            return (T)setting;
        }

        else
            throw new ArgumentException("Not a known type.");
    }

    public static bool ToggleConfigSetting(string section, string key)
    {
        if (ReadValue(section, key) == "true")
        {
            WriteValue(section, key, "false");
            return false;
        }
        else
        {
            WriteValue(section, key, "true");
            return true;
        }
    }
}
