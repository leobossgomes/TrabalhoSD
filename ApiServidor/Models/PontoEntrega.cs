using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterfacexD.Models {
    public class PontoEntrega : Objeto {
        public override string RepresentacaoVisual { get { return "P"; } }

        public PontoEntrega(int id, int posicaoX, int posicaoY) : base(id, posicaoX, posicaoY) { }
    }
}
