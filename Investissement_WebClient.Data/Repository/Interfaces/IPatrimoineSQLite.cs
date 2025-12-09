namespace Investissement_WebClient.Data.Repository.Interfaces
{
    public interface IPatrimoineSQLite
    {
        public Dictionary<string, (double, double)> GetQuantiteInvestitParActif();

        public List<string> GetSymboles();

        public Dictionary<string, string> GetSymboleParActif();

        public Dictionary<DateTime, double> GetQuantiteInvestitParDate();

        public Dictionary<DateTime, double> GetValeurPatrimoineParDate();
    }
}
