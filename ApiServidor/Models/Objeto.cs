using Newtonsoft.Json;
using System;
using System.Threading;

namespace InterfacexD.Models
{

    public abstract class Objeto
    {
        [JsonProperty("representacaoVisual")]
        public abstract string RepresentacaoVisual { get; }

        [JsonProperty("id")]
        public long ID { get; protected set; }

        [JsonProperty("posicaoX")]
        public int PosicaoX { get; set; }

        [JsonProperty("posicaoY")]
        public int PosicaoY { get; set; }
        public int PosicaoXInicial { get; set; }
        public int PosicaoYInicial { get; set; }

        public Objeto(int id, int posicaoX, int posicaoY)
        {
            PosicaoX = posicaoX;
            PosicaoY = posicaoY;
            PosicaoXInicial = posicaoX;
            PosicaoYInicial = posicaoY;
            ID = id;
        }
    }
}
