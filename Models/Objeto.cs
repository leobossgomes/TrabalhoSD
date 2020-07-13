using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace InterfacexD.Models
{

    public abstract class Objeto
    {
        [JsonProperty("representacaoVisual")]
        public abstract string RepresentacaoVisual { get; }

        [JsonProperty("id")]
        public long ID { get; protected set; }

        public Objeto ObjetoVinculado { get; private set; }

        [JsonProperty("posicaoX")]
        public int PosicaoX { get; set; }

        [JsonProperty("posicaoY")]
        public int PosicaoY { get; set; }
        public int PosicaoXInicial { get; set; }
        public int PosicaoYInicial { get; set; }
        protected bool ContinuarSeMovendo = false;

        protected abstract void Mover();

        private Thread moverObjetoVinculado;

        public Objeto(int id, int posicaoX, int posicaoY)
        {
            PosicaoX = posicaoX;
            PosicaoY = posicaoY;
            PosicaoXInicial = posicaoX;
            PosicaoYInicial = posicaoY;
            ID = id;
        }

        protected void VincularAOutroObjeto(Objeto objeto) {
            if (ObjetoVinculado != null)
                throw new Exception($"O objeto de representação {RepresentacaoVisual} e ID {ID} já está vinculado ao objeto de representação {ObjetoVinculado.RepresentacaoVisual} e ID {ObjetoVinculado.ID}!");

            if (objeto.PosicaoX != PosicaoX || objeto.PosicaoY != PosicaoY)
                throw new Exception("Os dois objetos devem estar na mesma posicao para poderem serem vinculados!");

            ObjetoVinculado = objeto;
            if (objeto.ObjetoVinculado == null) {
                objeto.VincularAOutroObjeto(this);
                moverObjetoVinculado = new Thread(ObjetoVinculado.Mover);
                ObjetoVinculado.ContinuarSeMovendo = true;
                moverObjetoVinculado.Start();
            }
        }

        protected void DesvincularObjeto() {
            if (ObjetoVinculado == null)
                throw new Exception($"O objeto de representação {RepresentacaoVisual} e ID {ID} não está vinculado a nenhum objeto, portanto não pode ser desvinculado!");

            ObjetoVinculado.ContinuarSeMovendo = false;
            ObjetoVinculado.ObjetoVinculado = null;
            ObjetoVinculado = null;
        }

        //Atualização de posição na interface
        protected void AtualizarPosicaoNaInterface()
        {
            var requisicao = WebRequest.CreateHttp("https://apiinterfacesd.azurewebsites.net/Home/AtualizarPosicao");
            requisicao.Method = "POST";

            requisicao.Headers.Add("Content-Type", "application/json");
            using (var writer = new StreamWriter(requisicao.GetRequestStream()))
            {
                writer.Write($"{{ 'ID': '{ID}', 'posicaoX': '{PosicaoX}', 'posicaoY': '{PosicaoY}', 'representacaoVisual': '{RepresentacaoVisual}' }}");
            }

            requisicao.GetResponse();
        }
    }
}
