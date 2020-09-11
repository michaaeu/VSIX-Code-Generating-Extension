using EnvDTE80;
using Microsoft.VisualStudio.PlatformUI;
using Vespertan.Utils.Data;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Project = EnvDTE.Project;
using System;
using System.Windows.Forms;
using VSIXProject.TemplateUtils;
using System.IO;
using VSIXProject.TemplateUtils.TemplateSettings;

namespace VSIXProject
{
    public class TemplateInfoViewModel : VesBindableBase
    {
        TemplateListView view;              //  Window Object                          
        Project activeProject;

        public TemplateInfoViewModel(TemplateListView view)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.view = view;
            activeProject = GetActiveProject();

            ProjectItem ttFolder = GetTTFolder();
            if (ttFolder != null)
            {
                TemplatesInfoList = new VesObservableCollection<TemplateInfoRecord>();

                ProjectItems templates = ttFolder.ProjectItems;
                if (templates.Count > 0)
                {
                    foreach (ProjectItem template in templates)
                    {
                        string fileName = template.Name;
                        if (fileName.Substring(fileName.Length - Math.Min(3, fileName.Length)) == ".tt")
                        {
                            TemplatesInfoList.Add(new TemplateInfoRecord
                            {
                                Checked = false,
                                Name = template.Name,
                                NameWithoutExt = Path.GetFileNameWithoutExtension(template.Name),
                                Path = template.Properties.Item("FullPath").Value.ToString()
                            });
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Brak szablonów w folderze TT");
                    view.Close();
                }
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Czy chcesz utworzyć folder szablonów ?", "Brak folderu TT", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Project project = GetActiveProject();
                    project.ProjectItems.AddFolder("TT");
                }
                view.Close();
            }

        }
        private ProjectItem GetTTFolder()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            if (activeProject != null)
            {
                foreach (ProjectItem projectItem in activeProject.ProjectItems)
                {
                    if (projectItem.Kind == Constants.vsProjectItemKindPhysicalFolder && projectItem.Name == "TT")
                    {
                        return projectItem;
                    }
                }
            }
            return null;
        }
        public VesObservableCollection<TemplateInfoRecord> TemplatesInfoList { get => GetProperty<VesObservableCollection<TemplateInfoRecord>>(); set => SetProperty(value); }
        public DelegateCommand ExecuteCommand { get => GetPropertyOrDefault<DelegateCommand>(() => new DelegateCommand(ExecuteButton)); set => SetProperty(value); }
        public DelegateCommand CancelCommand { get => GetPropertyOrDefault<DelegateCommand>(() => new DelegateCommand(CancelButton)); set => SetProperty(value); }

        private void ExecuteButton()
        {
            
            ThreadHelper.ThrowIfNotOnUIThread();
            TemplateExecutor templateExecutor = new TemplateExecutor();
            foreach (TemplateInfoRecord templateInfo in TemplatesInfoList)
            {
                if (templateInfo.Checked)
                {
                    string folderName = "GeneratedFiles";
                    ImmutableSetting folder = templateInfo.ImmutableSettings.Find(x => x.Name.Contains("Katalog"));
                    if(folder != null)
                    {
                        if(folder.Value != null)
                        {
                            folderName = folder.Value;
                        }
                    }
                    CreateFolderInProject(folderName);

                    string generatedFilePath = templateExecutor.ProcessTemplateToFile(templateInfo);
                    string destinationFilepath = Directory.GetParent(Directory.GetParent(generatedFilePath).FullName) + "\\" + folderName + "\\" + Path.GetFileName(generatedFilePath);

                    if (File.Exists(destinationFilepath))
                    {
                        DialogResult dialogResult = MessageBox.Show("Czy chcesz nadpisać plik:\n" + destinationFilepath + "?", "Plik już istnieje", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            File.Delete(destinationFilepath);
                            File.Move(generatedFilePath, destinationFilepath);
                            view.Owner = null;
                            view.Close();
                        }
                        else
                        {
                            File.Delete(generatedFilePath);
                        }
                    }
                    else{
                        File.Move(generatedFilePath, destinationFilepath);
                        view.Owner = null;
                        view.Close();
                    }
                }
            }
        }
        private void CancelButton()
        {
            view.Owner = null;
            view.Close();
        }
        private ProjectItem CreateFolderInProject(string folderName)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ProjectItem folderItem = null;
            if (activeProject != null)
            {
                try
                {
                    folderItem = activeProject.ProjectItems.AddFolder(folderName, Constants.vsProjectItemKindPhysicalFolder);
                }
                catch
                {
                    /* folder already exists, nothing to do */
                }
            }
            return folderItem;
        }
        private static DTE2 GetDTE()
        {
            return Package.GetGlobalService(typeof(DTE)) as DTE2;
        }
        private static Project GetActiveProject()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Project activeProject = null;
            Array activeSolutionProjects = GetDTE().ActiveSolutionProjects as Array;
            if (activeSolutionProjects != null)
                if (activeSolutionProjects.Length > 0)
                    activeProject = activeSolutionProjects.GetValue(0) as Project;

            return activeProject;
        }
    }
}