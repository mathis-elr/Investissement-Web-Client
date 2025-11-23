using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IPatrimoineSQLite
    {
        public Dictionary<string, double> ReadQuantiteInvestitParActif();

        public List<string> GetSymboles();

        public Dictionary<string, string> GetSymboleParActif();

        //public double GetQuantiteTotaleActif(string nomActif);
    }
}
