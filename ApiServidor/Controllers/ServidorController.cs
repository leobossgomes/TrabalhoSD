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
    public class ServidorController : ControllerBase
    {
        // POST: api/Servidor/ProcessarPedido
        [HttpPost("ProcessarPedido")]
        public string ProcessarPedido([FromBody] object pedido)
        {
            Program.servidor.ProcessarPedido(JsonConvert.DeserializeObject<Pedido>(pedido.ToString()));
            return "O pedido está sendo processado!";
        }
    }
}
