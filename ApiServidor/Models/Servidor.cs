using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace InterfacexD.Models
{
    public class Servidor
    {
        private Queue<Pedido> pedidos = new Queue<Pedido>();

        public void ProcessarPedido(Pedido pedido)
        {
            // Recebe um JSON com o pedido e busca a estante onde está o produto.

            List<Estante> estantesComOsProdutosDoPedidoDuplicado = new List<Estante>();

            foreach (Produto produto in pedido.Produtos)
            {
                var estantesComProduto = BancoDeDados.Select("produto", new List<string> { "idEstante" }, $"nome like '{produto.NomeProduto.ToLower()}'");
                int idEstante = int.Parse(((object[])estantesComProduto[0])[0].ToString());

                estantesComOsProdutosDoPedidoDuplicado.Add(BancoDeDados.Estantes.FirstOrDefault(r => r.ID == idEstante));
            }

            List<Estante> estantesComOsProdutosDoPedido = new List<Estante>();
            foreach (Estante e in estantesComOsProdutosDoPedidoDuplicado)
            {
                if (!estantesComOsProdutosDoPedido.Contains(e))
                    estantesComOsProdutosDoPedido.Add(e);
            }

            // Descobrir quais robôs estão disponíveis
            List<Objeto> robosDisponiveis = new List<Objeto>();
            foreach (Objeto robo in BancoDeDados.Robos)
            {
                string retorno = RealizarRequisicao($"{robo.ID}/EstaDisponivel", "get");
                if (bool.Parse(retorno))
                    robosDisponiveis.Add(robo);
            }

            // Enviar a solicitação para todos os robos disponíveis via API para descobrir qual é o mais próximo do objetivo
            foreach (Estante e in estantesComOsProdutosDoPedido)
            {
                int menorCaminho = int.MaxValue;
                Robo roboQueVaiPraEstante = null;

                string estanteSerializada = JsonConvert.SerializeObject(e);

                if (robosDisponiveis.Any())
                {
                    foreach (Robo robo in robosDisponiveis)
                    {
                        int distancia = int.Parse(RealizarRequisicao($"{robo.ID}/VerificarDistancia", "post", estanteSerializada));

                        if (menorCaminho > distancia)
                        {
                            menorCaminho = distancia;
                            roboQueVaiPraEstante = robo;
                        }
                    }
                }
                else
                {
                    pedidos.Append(pedido);
                }

                // Agora que se sabe qual é o robô de menor caminho, envia o robô para a estante
                RealizarRequisicao($"{roboQueVaiPraEstante.ID}/DefinirObjetivo", "post", estanteSerializada);

                robosDisponiveis.Remove(roboQueVaiPraEstante);
            }

            //Retornar um status 200 (OK) com a mensagem de "Robo de id {ID} está indo buscar o produto"
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalLink">o que vem depois de api/Robo/</param>
        /// <param name="tipoMetodo">get, post, pathc, delete, etc.</param>
        private static string RealizarRequisicao(string finalLink, string tipoMetodo, string body = "")
        {
            var requisicao = WebRequest.CreateHttp("https://apirobos.azurewebsites.net/api/Robo/" + finalLink);
            requisicao.Method = tipoMetodo.ToUpper();

            if (!string.IsNullOrEmpty(body))
            {
                requisicao.Headers.Add("Content-Type", "application/json");

                using (var writer = new StreamWriter(requisicao.GetRequestStream()))
                {
                    writer.Write(body);
                }
            }

            using (WebResponse resposta = requisicao.GetResponse())
            {
                using (var reader = new StreamReader(resposta.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
