using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotacaoEstampas.Model
{
    public class Votacao
    {
        public Cliente Cliente;
        public DateTime Data;
        public List<bool> Votos;

        //HACK so porque aparentemente esse grid nao suporta binding com converter..
        public string VotoString0 { get { return Votos.Count > 0 ? (Votos[0] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString1 { get { return Votos.Count > 1 ? (Votos[1] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString2 { get { return Votos.Count > 2 ? (Votos[2] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString3 { get { return Votos.Count > 3 ? (Votos[3] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString4 { get { return Votos.Count > 4 ? (Votos[4] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString5 { get { return Votos.Count > 5 ? (Votos[5] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString6 { get { return Votos.Count > 6 ? (Votos[6] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString7 { get { return Votos.Count > 7 ? (Votos[7] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString8 { get { return Votos.Count > 8 ? (Votos[8] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString9 { get { return Votos.Count > 9 ? (Votos[9] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString10 { get { return Votos.Count > 10 ? (Votos[10] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString11 { get { return Votos.Count > 11 ? (Votos[11] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString12 { get { return Votos.Count > 12 ? (Votos[12] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString13 { get { return Votos.Count > 13 ? (Votos[13] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString14 { get { return Votos.Count > 14 ? (Votos[14] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString15 { get { return Votos.Count > 15 ? (Votos[15] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString16 { get { return Votos.Count > 16 ? (Votos[16] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString17 { get { return Votos.Count > 17 ? (Votos[17] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString18 { get { return Votos.Count > 18 ? (Votos[18] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString19 { get { return Votos.Count > 19 ? (Votos[19] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString20 { get { return Votos.Count > 20 ? (Votos[20] == true ? "SIM" : "NÃO") : ""; } }
        public string VotoString21 { get { return Votos.Count > 21 ? (Votos[21] == true ? "SIM" : "NÃO") : ""; } }
    }
}
