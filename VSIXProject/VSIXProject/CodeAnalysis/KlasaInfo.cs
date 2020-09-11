using System;
using System.Collections.Generic;

namespace VSIXProject
{   
    [Serializable]
    public class KlasaInfo
    {
        public string Nazwa { get; set; }
        public string PrzestrzenNazw { get; set; }
        public string ModyfikatorDostepu { get; set; }
        public List<WlasciwoscInfo> WlasciwosciLista { get; set; }
    }
}
