using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfacexD.Models {
    public class Pedido {
        [JsonProperty("produtos")]
        public List<Produto> Produtos { get; }

        public Pedido(List<Produto> produtos) {
            Produtos = produtos ?? new List<Produto>();
        }
    }
}
