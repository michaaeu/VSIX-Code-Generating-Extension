using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace VSIXProject.TemplateUtils.TemplateSettings
{
    public class InteractiveSetting
    {
        public InteractiveSetting(Type type, string name, object defaultValue, object[] values)
        {
            Type = type;
            Name = name;
            DefaultValue = defaultValue;
            Values = values;
        }
        public Type Type { get; set; }
        public string Name { get; set; }
        public object DefaultValue { get; set; }
        public object[] Values { get; set; }
    }
}
