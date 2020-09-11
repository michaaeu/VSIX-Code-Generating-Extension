using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextTemplating;
using Microsoft.VisualStudio.TextTemplating.VSHost;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using VSIXProject.TemplateUtils.TemplateSettings;

namespace VSIXProject.TemplateUtils
{
    class TemplateExecutor
    {
        readonly ITextTemplating textTemplatingService;
        readonly InfoUtils infoUtils;
        readonly KlasaInfo klasaInfo;
        readonly TTSettingsUtils TTUtils;
        ITextTemplatingSessionHost host;

        public TemplateExecutor()
        {
            textTemplatingService = Package.GetGlobalService(typeof(STextTemplating)) as ITextTemplating;
            infoUtils = new InfoUtils();
            klasaInfo = infoUtils.GetKlasaInfo();
            TTUtils = new TTSettingsUtils();

            host = textTemplatingService as ITextTemplatingSessionHost;
            host.Session = host.CreateSession();
        }
        
        // Generates file from template
        // Returns generated file path
        public string ProcessTemplateToString(TemplateInfoRecord template)
        {
            TTSettingsDictionary settingsDictionary = template.SettingsDictionary;
            string templateFilePath = template.Path;
            host.Session["klasaInfo"] = klasaInfo;
            //host.Session["ustawienia"] = settingsDictionary; 

            string templateContent = File.ReadAllText(templateFilePath);

            T4Callback cb = new T4Callback();
            string result = textTemplatingService.ProcessTemplate(templateFilePath, templateContent, cb);

            return result;
        }
        public string ProcessTemplateToFile(TemplateInfoRecord template)
        {
            TTSettingsDictionary settingsDictionary = template.SettingsDictionary;
            string templateFilePath = template.Path;

            ITextTemplatingSessionHost host = textTemplatingService as ITextTemplatingSessionHost;
            host.Session["klasaInfo"] = klasaInfo;
            host.Session["ustawienia"] = settingsDictionary;

            string templateContent = TTUtils.GetClearTemplate(templateFilePath);

            T4Callback cb = new T4Callback();
            string result = textTemplatingService.ProcessTemplate(templateFilePath, templateContent, cb);
            
            //result = ArrangeUsingRoslyn(result);    //Format code

            string resultFileName = Path.Combine(Path.GetDirectoryName(templateFilePath),
                                                klasaInfo.Nazwa + Path.GetFileNameWithoutExtension(templateFilePath))
                                                + cb.fileExtension;

            // Writing the processed output to file:
            File.WriteAllText(resultFileName, result, cb.outputEncoding);
            // Append any error messages:
            if (cb.errorMessages.Count > 0)
            {
                File.AppendAllLines(resultFileName, cb.errorMessages);
            }
            return resultFileName;
        }

        // Code Formatting 
        public string ArrangeUsingRoslyn(string csCode)
        {
            var tree = CSharpSyntaxTree.ParseText(csCode);
            var root = tree.GetRoot().NormalizeWhitespace();
            var ret = root.ToFullString();
            return ret;
        }
        class T4Callback : ITextTemplatingCallback
        {
            public List<string> errorMessages = new List<string>();
            public string fileExtension = ".txt";
            public Encoding outputEncoding = Encoding.UTF8;

            public void ErrorCallback(bool warning, string message, int line, int column)
            { errorMessages.Add(message); }
            public void SetFileExtension(string extension)
            { fileExtension = extension; }
            public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
            { outputEncoding = encoding; }
        }
    }
}
