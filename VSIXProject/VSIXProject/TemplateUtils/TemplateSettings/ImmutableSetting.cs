using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXProject.TemplateUtils.TemplateSettings
{
    public class ImmutableSetting
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public ImmutableSetting(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
