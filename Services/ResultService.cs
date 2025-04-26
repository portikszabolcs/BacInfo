using BacInfo.Models;
using System.Collections.Immutable;
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

        public async Task<ImmutableSortedSet<string>> GetCountyNames()
        {
            if (_bacResults == null) await GetBacResults();

            ImmutableSortedSet<string> counties = _bacResults!
                .DistinctBy(x => x.JudetNume).Select((item) => item.JudetNume)
                .ToImmutableSortedSet();

            return counties;
        }

        public async Task<List<double?>> GetCountyAverages()
        {
            _bacResults ??= await GetBacResults();

            IEnumerable<IGrouping<string, BacResult>> groups = _bacResults!.GroupBy((item) => item.JudetNume).OrderBy((item) => item.Key);

            return groups.Select((group) => (double?) group.Sum((item) => item.Media) / group.Count()).ToList();
        }

        private async Task<List<BacResult>> GetBacResults()
        {
            using (StreamReader r = new(Path.Combine("Data", "bac_2024_full_sesiune_1.json")))
            {
                string json = r.ReadToEnd();
                List<BacResultRaw>? results = JsonSerializer.Deserialize<List<BacResultRaw>>(json);
                if (results == null) return [];
                return await Task.Run(() => results.Select((item) => RawResultToBacResult(item)).ToList());
            }
        }

        public async Task<Dictionary<string, double?>> GetGradesDistribution()
        {
            _bacResults ??= await GetBacResults();
            Dictionary<string, double?> mediaDistribution = new();
            foreach (BacResult result in _bacResults!)
            {
                string key = result.Media.ToString("0");
                if (mediaDistribution.ContainsKey(key))
                {
                    mediaDistribution[key]++;
                }
                else
                {
                    mediaDistribution[key] = 1;
                }
            }
            mediaDistribution["<5"] = mediaDistribution["0"];
            mediaDistribution.Remove("0");
            return mediaDistribution;
        }

        public async Task<Dictionary<string, double?>> GetPassRate()
        {
            _bacResults ??= await GetBacResults();
            Dictionary<string, double?> passRate = new();
            int reusit = _bacResults.Count((item) => item.RezultatFinal == ResultType.Reusit);
            passRate.Add("Reusit", reusit);
            passRate.Add("Respins", _bacResults.Count - reusit);
            return passRate;
        }

        public async Task<Dictionary<string, double?>> GetGradesHistogram()
        {
            _bacResults ??= await GetBacResults();
            List<IGrouping<double, BacResult>> grades = _bacResults!.GroupBy((item) => Math.Floor(item.Media)).ToList();
            Dictionary<string, double?> histogram = new();
            grades.ForEach((group) =>
            {
                histogram.Add("≥" + group.Key.ToString(), group.Count());
            });
            histogram["≥9"] += histogram["≥10"]!.Value;
            histogram["<5"] = histogram["≥0"];
            histogram.Remove("≥0");
            histogram.Remove("≥10");
            return histogram;
        }

        public async Task<BacResult?> GetStudentByCode(string code)
        {
            _bacResults ??= await GetBacResults();
            return _bacResults.Find((x) => x.CodCandidat == code);
        }

        public BacResult RawResultToBacResult(BacResultRaw raw)
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

        public ResultType GetResultFromString(string val)
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
