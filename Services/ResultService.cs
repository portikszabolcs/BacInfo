using BacInfo.Models;
using System.Text.Json;

namespace BacInfo.Services
{
    public class ResultService
    {
        private List<BacResult>? _bacResults = null;
        public async Task<List<BacResult>> GetAllResults()
        {
            _bacResults ??= await GetBacResults();
            return _bacResults!;
        }

        private static async Task<List<BacResult>> GetBacResults()
        {
            using (StreamReader r = new(Path.Combine("Data", "bac_2024_full_sesiune_1.json")))
            {
                string json = r.ReadToEnd();
                List<BacResultRaw>? results = JsonSerializer.Deserialize<List<BacResultRaw>>(json);
                if (results == null) return [];
                return await Task.Run(() => results.Select((item) => RawResultToBacResult(item)).ToList());
            }
        }

        private static BacResult RawResultToBacResult(BacResultRaw raw)
        {
            Limba? limbaRomana = null;
            if (raw.limba_romana_scris.ValueKind == JsonValueKind.Number &&
                raw.limba_romana_nota_finala.ValueKind == JsonValueKind.Number)
            {
                limbaRomana = new Limba()
                {
                    Nume = "Limba Romana",
                    Competente = raw.limba_romana_competente,
                    Scris = raw.limba_romana_scris.GetSingle(),
                    Contestatie = raw.limba_romana_contestatie,
                    NotaFinala = raw.limba_romana_nota_finala.GetSingle()
                };
            }

            Limba? limbaMaterna = null;
            if (raw.limba_materna != null && raw.limba_materna_scris.ValueKind == JsonValueKind.Number &&
                raw.limba_materna_nota_finala.ValueKind == JsonValueKind.Number)
            {
                limbaMaterna = new Limba()
                {
                    Nume = raw.limba_materna,
                    Competente = raw.limba_materna_competente ?? "",
                    Scris = raw.limba_materna_scris.GetSingle(),
                    Contestatie = raw.limba_materna_contestatie,
                    NotaFinala = raw.limba_materna_nota_finala.GetSingle()
                };
            }

            Disciplina? disciplinaObligatorie = null;
            if (raw.disciplina_obligatorie_scris.ValueKind == JsonValueKind.Number &&
                raw.disciplina_obligatorie_nota_finala.ValueKind == JsonValueKind.Number)
            {
                disciplinaObligatorie = new Disciplina()
                {
                    Nume = raw.disciplina_obligatorie,
                    Scris = raw.disciplina_obligatorie_scris.GetSingle(),
                    Contestatie = raw.disciplina_obligatorie_contestatie,
                    NotaFinala = raw.disciplina_obligatorie_nota_finala.GetSingle()
                };
            }

            Disciplina? disciplinaAleasa = null;
            if (raw.disciplina_aleasa_scris.ValueKind == JsonValueKind.Number &&
                raw.disciplina_aleasa_nota_finala.ValueKind == JsonValueKind.Number)
            {
                disciplinaAleasa = new Disciplina()
                {
                    Nume = raw.disciplina_aleasa,
                    Scris = raw.disciplina_aleasa_scris.GetSingle(),
                    Contestatie = raw.disciplina_aleasa_contestatie,
                    NotaFinala = raw.disciplina_aleasa_nota_finala.GetSingle()
                };
            }

            BacResult bacResult = new()
            {
                Nr = raw.nr,
                CodCandidat = raw.cod_candidat,
                UnitateInvatamant = raw.unitate_invatamant,
                JudetCod = raw.judet_cod,
                JudetNume = raw.judet_nume,
                Specializare = raw.specializare,
                LimbaRomana = limbaRomana,
                LimbaMaterna = limbaMaterna,
                LimbaModerna = raw.limba_moderna,
                LimbaModernaNota = raw.limba_moderna_nota,
                DisciplinaObligatorie = disciplinaObligatorie,
                DisciplinaAleasa = disciplinaAleasa,
                CompetenteDigitale = raw.competente_digitale,
                Media = raw.media ?? 0,
                RezultatFinal = GetResultFromString(raw.rezultat_final)
            };
            return bacResult;
        }

        public static ResultType GetResultFromString(string val)
        {
            return val switch
            {
                "Reusit" => ResultType.Reusit,
                "Respins" => ResultType.Respins,
                _ => ResultType.Neprezentat,
            };
        }
    }
}
