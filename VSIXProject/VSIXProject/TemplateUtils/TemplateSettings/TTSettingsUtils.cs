using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VSIXProject.TemplateUtils.TemplateSettings
{
    public class TTSettingsUtils
    {
        public string GetClearTemplate(string filePath)
        {
            string templateCode = "";
            string line;
            StreamReader reader = new StreamReader(filePath);
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.StartsWith("!!!!") && !line.StartsWith("####")){
                    templateCode += line + Environment.NewLine;
                }
            }
            return templateCode;
        }
        public List<ImmutableSetting> GetImmutableSettings(TemplateInfoRecord templateInfo)
        {
            TemplateExecutor executor = new TemplateExecutor();
            string preprocessedContent = executor.ProcessTemplateToString(templateInfo);
            //MessageBox.Show(preprocessedContent);

            StringReader reader = new StringReader(preprocessedContent);
            List<ImmutableSetting> imSettings = new List<ImmutableSetting>();

            string line;
            var immutableSettingsSection = false;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("!!!!"))
                {
                    if (immutableSettingsSection)
                    {
                        break;
                    }
                }
                else if (line.StartsWith("####"))
                {
                    immutableSettingsSection = true;

                    string[] elements = line.Substring(4, line.Length - 4).Split('=');
                    if (elements.Length < 2)
                        continue;

                    imSettings.Add(new ImmutableSetting(elements[0], elements[1]));
                }
                else if (line.StartsWith("<#@ "))
                {
                    if (immutableSettingsSection)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return imSettings;
        }

        public List<InteractiveSetting> GetInteractiveSettings(TemplateInfoRecord templateInfo)
        {
            StreamReader reader = new StreamReader(templateInfo.Path);
            List<InteractiveSetting> interSettings = new List<InteractiveSetting>();

            string line;
            var interactiveSettingsSection = false;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("####"))
                {
                    if (interactiveSettingsSection)
                    {
                        break;
                    }
                }
                else if (line.StartsWith("!!!!"))
                {
                    interactiveSettingsSection = true;

                    string[] elements = line.Substring(4, line.Length - 4).Split(';');
                    if (elements.Length < 3)
                        continue;

                    Type type = null;
                    string name = null;
                    object defaultValue = null;
                    object[] values = null;

                    foreach (string elem in elements)
                    {
                        if (!elem.Contains('='))
                            continue;

                        if (elem.Contains("Typ")) type = GetTypeFromLine(elem);
                        else if (elem.Contains("Nazwa")) name = GetNameFromLine(elem);
                        else if (elem.Contains("WartoscDomyslna")) defaultValue = GetDefaultValueFromLine(elem, type);
                        else if (elem.Contains("Wartosci")) values = GetValuesFromLine(elem, type);
                    }

                    if (type != null && name != null)
                    {
                        interSettings.Add(new InteractiveSetting(type, name, defaultValue, values));
                    }
                }
                else if (line.StartsWith("<#@ "))
                {
                    if (interactiveSettingsSection)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return interSettings;
        }
        private object[] GetValuesFromLine(string elem, Type type)
        {

            if (type == null)
            {
                return null;
            }
            try
            {
                elem = elem.Substring(elem.IndexOf("=") + 2).Trim();
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }

            List<object> values = new List<object>();
            string[] strValues = elem.Split('|');
            foreach (string strVal in strValues)
            {
                object val = GetSingleValueFromLine(strVal, type);
                if (val != null)
                    values.Add(val);
            }
            return values.ToArray();
        }

        private object GetSingleValueFromLine(string strValue, Type type)
        {
            if (type == typeof(string))
            {
                return strValue;
            }
            else if (type == typeof(bool))
            {
                bool.TryParse(strValue, out bool value);
                return value;
            }
            else if (type == typeof(int))
            {
                int.TryParse(strValue, out int value);
                return value;
            }
            else if (type == typeof(double))
            {
                double.TryParse(strValue,NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out double value);
                return value;
            }
            return null;
        }

        private object GetDefaultValueFromLine(string elem, Type type)
        {
            if (type == null)
            {
                return null;
            }
            string strValue;
            try
            {
                strValue = elem.Substring(elem.IndexOf("=") + 2).Trim();
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            return GetSingleValueFromLine(strValue, type);
        }

        private string GetNameFromLine(string elem)
        {
            string name;
            try
            {
                name = elem.Substring(elem.IndexOf("=") + 2).Trim();
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
            return name;
        }

        private Type GetTypeFromLine(string strType)
        {
            Type type;
            if (strType.Contains("Bool"))
            {
                type = typeof(bool);
            }
            else if (strType.Contains("String"))
            {
                type = typeof(string);
            }
            else if (strType.Contains("Int"))
            {
                type = typeof(int);
            }
            else if (strType.Contains("Double"))
            {
                type = typeof(double);
            }
            else
            {
                type = null;
            }
            return type;
        }
        internal void UpdateDictionary(TemplateInfoRecord templateInfoRecord)
        {
            if (templateInfoRecord.SettingsDictionary == null)
            {
                templateInfoRecord.SettingsDictionary = new TTSettingsDictionary();
            }
            else
            {
                templateInfoRecord.SettingsDictionary.Clear();
            }

            lock (templateInfoRecord.SettingsDictionary)
            {
                foreach (InteractiveSetting s in templateInfoRecord.InteractiveSettings)
                {
                    templateInfoRecord.SettingsDictionary.Add(
                        s.Name,
                        s.DefaultValue.ToString()
                    );
                }

            } 
           
        }
    }
}
