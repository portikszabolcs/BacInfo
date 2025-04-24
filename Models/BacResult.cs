namespace BacInfo.Models
{
    public class BacResult
    {
        public int Nr { get; set; }
        public string CodCandidat { get; set; }
        public string UnitateInvatamant { get; set; }
        public int JudetCod { get; set; }
        public string JudetNume { get; set; }
        public string Specializare { get; set; }
        public Limba? LimbaRomana { get; set; }
        public Limba? LimbaMaterna { get; set; }
        public string LimbaModerna { get; set; }
        public string? LimbaModernaNota { get; set; }
        public Disciplina? DisciplinaObligatorie { get; set; }
        public Disciplina? DisciplinaAleasa { get; set; }
        public string CompetenteDigitale { get; set; }
        public float Media { get; set; }
        public ResultType RezultatFinal { get; set; }
    }

    public enum ResultType
    {
        Reusit, Respins, Neprezentat
    }
}
