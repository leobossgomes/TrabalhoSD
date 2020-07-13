using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfacexD.Models {
    public class Produto {
        [JsonProperty("id")]
        public long ID { get; private set; }
        [JsonProperty("nome")]
        public string NomeProduto { get; private set; }

        public Produto(string nomeProduto) {
            NomeProduto = nomeProduto;
        }
    }
}
