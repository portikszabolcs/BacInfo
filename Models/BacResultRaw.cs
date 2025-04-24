using System.Text.Json;

namespace BacInfo.Models
{

    public class BacResultRaw
    {
        public int nr { get; set; }
        public string cod_candidat { get; set; }
        public string unitate_invatamant { get; set; }
        public int judet_cod { get; set; }
        public string judet_nume { get; set; }
        public bool promotie_anterioara { get; set; }
        public string forma_invatamant { get; set; }
        public string specializare { get; set; }
        public string limba_romana_competente { get; set; }
        public JsonElement limba_romana_scris { get; set; }
        public float? limba_romana_contestatie { get; set; }
        public JsonElement limba_romana_nota_finala { get; set; }
        public string? limba_materna { get; set; }
        public string? limba_materna_competente { get; set; }
        public JsonElement limba_materna_scris { get; set; }
        public float? limba_materna_contestatie { get; set; }
        public JsonElement limba_materna_nota_finala { get; set; }
        public string limba_moderna { get; set; }
        public string? limba_moderna_nota { get; set; }
        public string disciplina_obligatorie { get; set; }
        public JsonElement disciplina_obligatorie_scris { get; set; }
        public float? disciplina_obligatorie_contestatie { get; set; }
        public JsonElement disciplina_obligatorie_nota_finala { get; set; }
        public string disciplina_aleasa { get; set; }
        public JsonElement disciplina_aleasa_scris { get; set; }
        public float? disciplina_aleasa_contestatie { get; set; }
        public JsonElement disciplina_aleasa_nota_finala { get; set; }
        public string competente_digitale { get; set; }
        public float? media { get; set; }
        public string rezultat_final { get; set; }
    }
}
