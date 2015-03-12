using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApplication1.Models;
using WindowsFormsApplication1;

namespace WindowsFormsApplication1.Controllers
{
    /// <summary>
    /// Controlador de vista Form1
    /// </summary>
    class viewController
    {
        private cpuMonitor cpu;
        private diskMonitor disk;
        private networkMonitor network;
        private ramMonitor ram;

        //inicializa todos modelos con los mismos valores
        public viewController(Form1 context, int refreshTime)
        {   //instanciar modelos.
            cpu = new cpuMonitor(refreshTime, context);
            disk = new diskMonitor(refreshTime, context);
            network = new networkMonitor(refreshTime, context);
            ram = new ramMonitor(refreshTime, context);
        }

        /// <summary>
        /// inicia el thread de cada modelo.
        /// </summary>
        public void Start()
        {   //iniciar proceoso de monitoreo.
            cpu.Start();
            disk.Start();
            network.Start();
            ram.Start();
        }
    }
}
