using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterfacexD.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InterfacexD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoboController : ControllerBase
    {
        // GET: api/Robo/5/EstaDisponivel
        [HttpGet("{id}/{metodo}", Name = "Get")]
        public string Get(int id, string metodo)
        {
            Robo robo = BuscarRobo(id);
            if (robo == null)
                return "Não foi possível encontrar o Robo de ID " + id;

            if (metodo == "EstaDisponivel")
                return robo.EstaDisponivel().ToString();

            return $"Não existe implementação para o método {metodo}";
        }

        // POST: api/Robo/5/VerificarDistancia
        [HttpPost("{id}/{metodo}")]
        public string Post(int id, string metodo, [FromBody] object estanteBody)
        {
            Robo robo = BuscarRobo(id);
            if (robo == null)
                return $"Não foi possível encontrar o Robo de ID {id}!";

            Estante estante = JsonConvert.DeserializeObject<Estante>(estanteBody.ToString());

            switch (metodo)
            {
                case "VerificarDistancia":
                    return robo.VerificarDistancia(estante).ToString();
                case "DefinirObjetivo":
                    robo.DefinirObjetivo(estante);
                    return $"Objetivo definido com sucesso para o Robo de ID {id}!";
                default:
                    return "É necessário passar um método para ser executado (api/Robo/{id}/{metodo})";
            }
        }

        private Robo BuscarRobo(long id)
        {
            Robo robo = BancoDeDados.Robos.FirstOrDefault(r => r.ID == id);
            return robo;
        }
    }
}
