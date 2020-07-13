using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfacexD.Models {
    public class Estante : Objeto {
        public override string RepresentacaoVisual { get { return "E"; } }
        public List<Produto> Produtos = new List<Produto>();

        public Estante(int id, int posicaoX, int posicaoY) : base(id, posicaoX, posicaoY) { }
    }
}
