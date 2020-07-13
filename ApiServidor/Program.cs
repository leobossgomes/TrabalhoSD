using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfacexD.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InterfacexD
{
    public class Program
    {
        public static Servidor servidor;
        public const int Largura = 8;
        public const int Comprimento = 12;
        public static string[,] matrizArmazem = new string[Comprimento, Largura];

        public static void Main(string[] args)
        {
            servidor = new Servidor();
            BancoDeDados.BuscaObjetosDaTela();

            for (int j = 0; j < Largura; j++)
            {
                for (int i = 0; i < Comprimento; i++)
                {
                    Objeto o = BancoDeDados.TodosOsObjetos.FirstOrDefault(r => r.PosicaoX == i && r.PosicaoY == j);
                    matrizArmazem[i, j] = o == null ? "" : o.RepresentacaoVisual;
                }
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
