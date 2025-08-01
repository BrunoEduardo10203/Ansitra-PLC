using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace AnsitraPLC_QR_MIT.Models
{
    internal class DadosJson
    {

        public string ip_servidor { get; set; }

        public RedeEstacao rede_estacao { get; set; }
        public RedeModulo rede_modulo { get; set; }

        public RedeEstacao rede_plc1 { get; set; }
        public RedeModulo rede_sensor1 { get; set; }
        public RedeEstacao rede_plc2 { get; set; }
        public RedeModulo rede_sensor2 { get; set; }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]


        public BancoDados banco_dados { get; set; }
        public string num_estacao { get; set; }
        public string id_rodovia { get; set; }
        public string km_rodovia { get; set; }
        public string local_rodovia { get; set; }
        public string uf_rodovia { get; set; }



        public IdFaixa id_faixa { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DistLaco dist_laco { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public CompLaco comp_laco { get; set; }
        public int comp_maior_valor { get; set; }
        public int comp_maior_gravar { get; set; }
        public int comp_menor_valor { get; set; }
        public int comp_menor_gravar { get; set; }
        public int vel_maior_valor { get; set; }
        public int vel_maior_gravar { get; set; }
        public int vel_menor_valor { get; set; }
        public int vel_menor_gravar { get; set; }
        public Dictionary<string, ClassComp> class_comp { get; set; }
    }

    internal class RedeEstacao
    {
        public string ip { get; set; }
        public string mask { get; set; }
        public string gateway { get; set; }
    }

    internal class RedeModulo
    {
        public string ip { get; set; }
        public string mask { get; set; }
        public string gateway { get; set; }
    }

    internal class BancoDados
    {
        public string tipo { get; set; }
        public string host_banco { get; set; }
        public string usuario { get; set; }
        public string porta { get; set; }
        public string exibicao { get; set; }
        public string servidor { get; set; }
        public string senha { get; set; }
    }

    internal class IdFaixa
    {
        public string FX1 { get; set; }
        public string FX2 { get; set; }
        public string FX3 { get; set; }
        public string FX4 { get; set; }
        public string FX5 { get; set; }
        public string FX6 { get; set; }
        public string FX7 { get; set; }
        public string FX8 { get; set; }
        public string FX9 { get; set; }
        public string FX10 { get; set; }
        public string FX11 { get; set; }
        public string FX12 { get; set; }
        public string FX13 { get; set; }
        public string FX14 { get; set; }
        public string FX15 { get; set; }
        public string FX16 { get; set; }
    }

    internal class CompLaco
    {
        public int FX1 { get; set; }
        public int FX2 { get; set; }
        public int FX3 { get; set; }
        public int FX4 { get; set; }
        public int FX5 { get; set; }
        public int FX6 { get; set; }
        public int FX7 { get; set; }
        public int FX8 { get; set; }
        public int FX9 { get; set; }
        public int FX10 { get; set; }
        public int FX11 { get; set; }
        public int FX12 { get; set; }
        public int FX13 { get; set; }
        public int FX14 { get; set; }
        public int FX15 { get; set; }
        public int FX16 { get; set; }
    }

    internal class DistLaco
    {
        public int FX1 { get; set; }
        public int FX2 { get; set; }
        public int FX3 { get; set; }
        public int FX4 { get; set; }
        public int FX5 { get; set; }
        public int FX6 { get; set; }
        public int FX7 { get; set; }
        public int FX8 { get; set; }
        public int FX9 { get; set; }
        public int FX10 { get; set; }
        public int FX11 { get; set; }
        public int FX12 { get; set; }
        public int FX13 { get; set; }
        public int FX14 { get; set; }
        public int FX15 { get; set; }
        public int FX16 { get; set; }
    }

    internal class ClassComp
    {
        public int tipo { get; set; }
        public string nome { get; set; }
        public int inicio { get; set; }
        public int fim { get; set; }
        public int eixo { get; set; }
    }

    internal class DadosClassJson
    {
        public Dictionary<string, ClassComp> class_comp { get; set; }
    }

}