using System.Collections.Generic;
using System.Windows.Documents;
using Vespertan.Utils.Data;
using VSIXProject.TemplateUtils;
using VSIXProject.TemplateUtils.TemplateSettings;

namespace VSIXProject
{
    public class TemplateInfoRecord : VesBindableBase
    {
        public bool Checked { get => GetProperty<bool>(); set => SetProperty(value); }
        public string Name { get => GetProperty<string>(); set => SetProperty(value); }
        public string NameWithoutExt { get => GetProperty<string>(); set => SetProperty(value); }
        public string Path { get => GetProperty<string>(); set => SetProperty(value); }
        public TTSettingsDictionary SettingsDictionary { get; set; }
        public List<InteractiveSetting> InteractiveSettings { get; set; }
        public List<ImmutableSetting> ImmutableSettings { get; set; }
    }
}