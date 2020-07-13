using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace InterfacexD
{
    public class Interface
    {
        public Thread thread;

        public Interface()
        {
            thread = new Thread(RenderizarObjetos);
        }

        public void RenderizarObjetos()
        {
            Thread.Sleep(500);            
        }
    }
}
