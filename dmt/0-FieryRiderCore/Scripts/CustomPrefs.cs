using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.ComponentModel;

public class CustomPrefs
{
    private static readonly CustomPrefs _instance = new CustomPrefs();
    private Dictionary<string, string> _preferences;
    private string xmlFileLocation = Utils.GetGamePath() + "/Data/Config/customPrefs.xml";

    private CustomPrefs()
    {
        _preferences = new Dictionary<string, string>();

        XmlDocument settingsFile = new XmlDocument();
        try
        {
            settingsFile.Load(xmlFileLocation);
        }
        catch (FileNotFoundException)
        {
            settingsFile.LoadXml("<preferences></preferences>");
        }
        foreach (XmlElement node in settingsFile.DocumentElement)
        {
            string settingName = node.GetAttribute("name");
            string settingValue = node.InnerText;
            _preferences.Add(settingName,settingValue);
        }
    }

    public static CustomPrefs GetCustomPrefs()
    {
        return _instance;
    }
    public T GetCustomPref<T>(string name)
    {
        string value;
        if (_preferences.TryGetValue(name, out value))
        {
            if (value is T variable)
                return variable;

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
            return (T)converter.ConvertFrom(value);
        }

        throw new System.ArgumentException("There is no preference with name: " + name);
    }

    public void SetCustomPref<T>(string name, T value)
    {
        _preferences[name] = value.ToString();
        Save();
    }

    private void Save()
    {
        XmlDocument settings = new XmlDocument();
        settings.LoadXml("<preferences></preferences>");
        foreach (KeyValuePair<string, string> entry in _preferences)
        {
            XmlElement preferenceElement = settings.CreateElement("preference");
            XmlAttribute setting = settings.CreateAttribute("name");
            setting.Value = entry.Key;
            preferenceElement.Attributes.Append(setting);
            preferenceElement.InnerText = entry.Value;
            settings.DocumentElement.AppendChild(preferenceElement);
        }

        settings.Save(xmlFileLocation);
    }
}