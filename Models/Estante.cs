using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace InterfacexD.Models {
    public class Estante : Objeto {
        public override string RepresentacaoVisual { get { return "E"; } }

        public Estante(int id, int posicaoX, int posicaoY) : base(id, posicaoX, posicaoY) { }

        protected override void Mover()
        {
            while (ContinuarSeMovendo)
            {
                if (ObjetoVinculado == null && ContinuarSeMovendo)
                    throw new Exception("Uma estante não pode se mover se não tiver nenhum objeto vinculado a ela!");

                if (ContinuarSeMovendo)
                    PosicaoX = ObjetoVinculado.PosicaoX;
                if (ContinuarSeMovendo)
                    PosicaoY = ObjetoVinculado.PosicaoY;

                AtualizarPosicaoNaInterface();

                Thread.Sleep(500);
            }
        } 
    }
}
