using System;
using System.Collections.Generic;
using System.Text;

namespace TestApplication
{
    public class TestClass
    {
        [Obsolete]
        public int ID { get; set; }
        public int Code { get; set; }
        public string Name { get; set; }
    }
}
