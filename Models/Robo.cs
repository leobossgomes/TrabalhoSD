using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient.Memcached;
using Newtonsoft.Json;
using System.IO;

namespace InterfacexD.Models
{
    public class Robo : Objeto
    {
        public override string RepresentacaoVisual { get { return "R"; } }
        private Objeto Objetivo = null;
        private const int sleepTime = 3000;

        private Thread SeMover;

        public Robo(int id, int posicaoX, int posicaoY) : base(id, posicaoX, posicaoY) { }

        public void DefinirObjetivo(Objeto objetivo)
        {
            if (Objetivo != null)
                throw new Exception($"O robo de ID {ID} já possui um objetivo");

            Objetivo = objetivo;

            SeMover = new Thread(Mover);
            SeMover.Start();
        }

        protected override void Mover()
        {
            IrAteObjetivo();
            VincularAOutroObjeto(Objetivo);
            VoltarComObjetoParaOGPontoDeEntrega();
            DevolverObjetoParaLocalInicial();
            DesvincularObjeto();
            VoltarParaOLocalInicial();
        }

        public int VerificarDistancia(Objeto objetivo)
        {
            Objetivo = objetivo;
            int distancia = buscarCaminho(false, objetivo.PosicaoX, objetivo.PosicaoY);
            Objetivo = null;
            return distancia;
        }

        public bool EstaDisponivel()
        {
            return Objetivo == null;
        }

        private void IrAteObjetivo(bool utilizarValoresIniciais = false, bool utilizarCorredores = false)
        {
            int posicaoX = utilizarValoresIniciais ? Objetivo.PosicaoXInicial : Objetivo.PosicaoX;
            int posicaoY = utilizarValoresIniciais ? Objetivo.PosicaoYInicial : Objetivo.PosicaoY;


            bool chegou = false;

            matrizProgram[PosicaoXInicial, PosicaoYInicial] = "";
            while (!chegou)
            {
                int quantidadeTentativas = 0;
                buscarCaminho(utilizarCorredores, posicaoX, posicaoY);
                List<Posicao> caminho = reconstruirCaminho(new Posicao(PosicaoX, PosicaoY), new Posicao(posicaoX, posicaoY));
                foreach (Posicao posicao in caminho)
                {
                    while (BancoDeDados.Robos.Any(r => r.PosicaoX == posicao.PosicaoX && r.PosicaoY == posicao.PosicaoY && r.ID != ID))
                    {
                        Thread.Sleep(sleepTime);
                        quantidadeTentativas++;
                        if (quantidadeTentativas > 2)
                            break;
                    }
                    if (quantidadeTentativas > 2)
                        break;
                    PosicaoX = posicao.PosicaoX;
                    PosicaoY = posicao.PosicaoY;
                    AtualizarPosicaoNaInterface();
                    Thread.Sleep(sleepTime);
                }
                chegou = PosicaoX == posicaoX && PosicaoY == posicaoY;
            }
            matrizProgram[PosicaoXInicial, PosicaoYInicial] = RepresentacaoVisual;
        }

        private void VoltarComObjetoParaOGPontoDeEntrega()
        {
            Objetivo = BancoDeDados.PontoDeEntrega;
            IrAteObjetivo(utilizarCorredores: true);
        }

        private void DevolverObjetoParaLocalInicial()
        {
            Objetivo = ObjetoVinculado;
            IrAteObjetivo(true, true);
        }

        private void VoltarParaOLocalInicial()
        {
            Objetivo = this;
            IrAteObjetivo(true);
            Objetivo = null;
        }


        // -----------------------------
        // Métodos para calcular caminho
        // Um,a adaptação do BFS de Grafos
        // -----------------------------

        Queue<int> posicoesColuna = new Queue<int>();
        Queue<int> posicoesLinha = new Queue<int>();

        int nodesDaProximaCamada = 0;

        bool[,] visited = new bool[Program.Comprimento, Program.Largura];

        int[] direcoesLinha = new int[] { -1, 1, 0, 0 };
        int[] direcoesColuna = new int[] { 0, 0, 1, -1 };

        Posicao[,] posicoesAnteriores = new Posicao[Program.Comprimento, Program.Largura];
        string[,] matrizProgram = Program.matrizArmazem;

        private int buscarCaminho(bool utilizarApenasCorredores, int posicaoX, int posicaoY)
        {
            int quantidadeMovimentos = 0;
            int nodesFaltantesNaCamada = 1;

            bool chegouAoFinal = false;

            nodesDaProximaCamada = 0;
            posicoesColuna = new Queue<int>();
            posicoesLinha = new Queue<int>();
            posicoesAnteriores = new Posicao[Program.Comprimento, Program.Largura];
            visited = new bool[Program.Comprimento, Program.Largura];

            matrizProgram[posicaoX, posicaoY] = "O";
            posicoesLinha.Enqueue(PosicaoX);
            posicoesColuna.Enqueue(PosicaoY);

            visited[PosicaoX, PosicaoY] = true;

            while (posicoesColuna.Count > 0)
            {
                int linha = posicoesLinha.Dequeue();
                int coluna = posicoesColuna.Dequeue();
                if (matrizProgram[linha, coluna] == "O")
                {
                    chegouAoFinal = true;
                    break;
                }
                ExplorarVizinhos(linha, coluna, utilizarApenasCorredores);
                nodesFaltantesNaCamada--;
                if (nodesFaltantesNaCamada == 0)
                {
                    nodesFaltantesNaCamada = nodesDaProximaCamada;
                    nodesDaProximaCamada = 0;
                    quantidadeMovimentos++;
                }
            }
            matrizProgram[posicaoX, posicaoY] = Objetivo.RepresentacaoVisual;

            if (chegouAoFinal)
                return quantidadeMovimentos;
            return -1;
        }

        private void ExplorarVizinhos(int linha, int coluna, bool utilizarApenasCorredores)
        {
            for (int i = 0; i < 4; i++)
            { // 4 = quantidade de direções possíveis
                int proximaLinha = linha + direcoesLinha[i];
                int proximaColuna = coluna + direcoesColuna[i];

                if (proximaLinha < 0 || proximaColuna < 0) continue;
                if (proximaColuna >= Program.Largura || proximaLinha >= Program.Comprimento) continue;

                if (visited[proximaLinha, proximaColuna]) continue;
                if (BancoDeDados.Robos.Any(r => r.PosicaoX == proximaLinha && r.PosicaoY == proximaColuna)) continue;
                if (utilizarApenasCorredores && (!string.IsNullOrEmpty(matrizProgram[proximaLinha, proximaColuna]) && matrizProgram[proximaLinha, proximaColuna] != "O")) continue;

                posicoesColuna.Enqueue(proximaColuna);
                posicoesLinha.Enqueue(proximaLinha);
                visited[proximaLinha, proximaColuna] = true;
                visited[linha, coluna] = true;
                posicoesAnteriores[proximaLinha, proximaColuna] = new Posicao(linha, coluna);
                nodesDaProximaCamada++;
            }
        }

        public List<Posicao> reconstruirCaminho(Posicao inicio, Posicao final)
        {
            List<Posicao> caminhoFinal = new List<Posicao>();
            Posicao atual = final;

            while (atual != null)
            {
                caminhoFinal.Add(atual);
                atual = posicoesAnteriores[atual.PosicaoX, atual.PosicaoY];
            }

            caminhoFinal.Reverse();

            if (caminhoFinal[0].PosicaoX == inicio.PosicaoX && caminhoFinal[0].PosicaoY == inicio.PosicaoY)
                return caminhoFinal;
            return new List<Posicao>();
        }
    }

    public class Posicao
    {
        public int PosicaoX { get; set; }
        public int PosicaoY { get; set; }

        public Posicao(int posicaoX, int posicaoY)
        {
            PosicaoX = posicaoX;
            PosicaoY = posicaoY;
        }
    }
}
