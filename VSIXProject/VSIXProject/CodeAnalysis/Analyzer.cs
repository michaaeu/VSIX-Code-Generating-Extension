using EnvDTE;
using EnvDTE80;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace VSIXProject.TemplateUtils
{
    class InfoUtils
    {
        private IEnumerable<MemberDeclarationSyntax> members;
        private async void Initialize()
        {
            //await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            UIHierarchy UIH = GetDTE().ToolWindows.SolutionExplorer;
            Array selectedItems = (Array)UIH.SelectedItems;

            if (selectedItems == null)
                return;

            UIHierarchyItem UIHItem = (UIHierarchyItem)selectedItems.GetValue(0);
            ProjectItem projectItem = UIHItem.Object as ProjectItem;
            string filePath = projectItem.Properties.Item("FullPath").Value.ToString();

            var solution = GetSComponentModel().GetService<Microsoft.VisualStudio.LanguageServices.VisualStudioWorkspace>().CurrentSolution;
            var documentId = solution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault();
            var document = solution.GetDocument(solution.GetDocumentIdsWithFilePath(filePath).FirstOrDefault());

            SyntaxTree tree = await document.GetSyntaxTreeAsync();
            members = tree.GetRoot().DescendantNodes().OfType<MemberDeclarationSyntax>();
        }

        public KlasaInfo GetKlasaInfo()
        {
            Initialize();
            KlasaInfo klasaInfo = new KlasaInfo();

            foreach (var member in members)
            {
                //  PrzestrzenNazw
                if (member is NamespaceDeclarationSyntax nameSpace)
                {
                    klasaInfo.PrzestrzenNazw = nameSpace.Name.ToString();
                }
                if (member is ClassDeclarationSyntax name)
                {
                    // Nazwa
                    klasaInfo.Nazwa = name.Identifier.ValueText;
                    // Modyfikator Dostępu
                    klasaInfo.ModyfikatorDostepu = name.Modifiers.ToString();
                    if (klasaInfo.ModyfikatorDostepu == null)
                    {
                        klasaInfo.ModyfikatorDostepu = "private";
                    }
                }
            }
            klasaInfo.WlasciwosciLista = GetWlasciwosciInfoLista();
            return klasaInfo;
        }
        private List<WlasciwoscInfo> GetWlasciwosciInfoLista()
        {
            List<WlasciwoscInfo> list = new List<WlasciwoscInfo>();
            foreach (var member in members)
            {
                if (member is PropertyDeclarationSyntax property)
                {
                    WlasciwoscInfo wlasciwoscInfo = new WlasciwoscInfo
                    {
                        Nazwa = property.Identifier.ValueText,
                        TypTekst = property.Type.ToString(),
                        ModyfikatorDostepu = property.Modifiers.ToString(),
                        MoznaZapisac = property.AccessorList.ToString().Contains("set"),
                        MoznaCzytac = property.AccessorList.ToString().Contains("get"),
                        AtrybutLista = new List<string>()
                    };
                    foreach (AttributeListSyntax attr in property.AttributeLists)
                    {
                        wlasciwoscInfo.AtrybutLista.Add(attr.ToString());
                    }
                    //PrintWlasciwosc(wlasciwoscInfo);     
                    list.Add(wlasciwoscInfo);
                }
            }
            return list;
        }
        private void PrintWlasciwosc(WlasciwoscInfo wlasciwoscInfo)
        {
            string atrybuty = wlasciwoscInfo.AtrybutLista != null ? string.Join(",", wlasciwoscInfo.AtrybutLista) : "Brak";
            MessageBox.Show(
                "Modyfikator:   \t" + wlasciwoscInfo.ModyfikatorDostepu + "\n" +
                "TypTekst:      \t" + wlasciwoscInfo.TypTekst + "\n" +
                "Nazwa:         \t" + wlasciwoscInfo.Nazwa + "\n" +
                "Czytanie:      \t" + wlasciwoscInfo.MoznaCzytac.ToString() + "\n" +
                "Zapisywanie:   \t" + wlasciwoscInfo.MoznaZapisac.ToString() + "\n" +
                "Atrybuty:      \t" + atrybuty
                );
        }
        private IComponentModel GetSComponentModel()
        {
            return Package.GetGlobalService(typeof(SComponentModel)) as IComponentModel;
        }
        private static DTE2 GetDTE()
        {
            return Package.GetGlobalService(typeof(DTE)) as DTE2;
        }
    }
}
