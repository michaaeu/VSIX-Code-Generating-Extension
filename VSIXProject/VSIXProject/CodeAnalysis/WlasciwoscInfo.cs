using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace VSIXProject
{
    [Serializable]
    public class WlasciwoscInfo
    {
        public string Nazwa { get; set; }
        public string TypTekst { get; set; }
        public string ModyfikatorDostepu { get; set; }
        public bool MoznaZapisac { get; set; }
        public bool MoznaCzytac { get; set; }
        public List<string> AtrybutLista { get; set; }
    }
}
