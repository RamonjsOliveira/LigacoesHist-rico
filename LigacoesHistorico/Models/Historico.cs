using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LigacoesHistorico.Models
{
    public class Historico
    {
           public DateTime DataHora { get; set; }
        public string TelefoneOrigem { get; set; }
        public string TelefoneDestino { get; set; }
        public int Duracao { get; set; }
        public int DDDOrigem { get; set; }
        public int DDDDestino { get; set; }
    }
}
