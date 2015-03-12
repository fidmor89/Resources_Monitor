using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;

namespace WindowsFormsApplication1.Models
{
    class ramMonitor
    {
        /// <summary>
        /// tiempo de espera
        /// </summary>
        private int millisecondsTimeout;
        /// <summary>
        /// nombre del thread
        /// </summary>
        private static String threadName = "Thread-Ram-Monitor";

        /// <summary>
        /// referencia a vista
        /// </summary>
        private Form1 viewContext;
        /// <summary>
        /// performance counter, componente de Win NT para contabilizar performance de pc
        /// </summary>
        private PerformanceCounter _memoryCounter = new PerformanceCounter();

        private Double virtualMemTotal;
        private Double virtualMemWired;
        private Double virtualMemPercentage;

        private Double physicalMemTotal;
        private Double physicalMemWired;
        private Double physicalMemPercentage;

        /// <summary>
        /// constructor default.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="cte"></param>
        public ramMonitor(int p, Form1 cte)
        {
            this.millisecondsTimeout = p;
            this.viewContext = cte;
        }

        /// <summary>
        /// crea e inicia el thread
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Starting: " + threadName);
            Thread newThread = new Thread(new ThreadStart(Run));
            newThread.Start();
        }

        /// <summary>
        /// Metodo principal del thread
        /// mientras exista interfaz:
        ///     - obtiene valores
        ///     - manda a imprimir varlores
        ///     - duerme tread
        /// </summary>
        private void Run()
        {
            Thread.CurrentThread.Name = threadName;
            Console.WriteLine("Running: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);

            #region

            while (!viewContext.IsDisposed)
            {
                calcularMemoriaFisica();
                calcularMemoriaVirtual();
                this.viewContext.SetRamVirtual(this.virtualMemTotal, this.virtualMemWired, this.virtualMemPercentage);
                this.viewContext.SetRamFisica(this.physicalMemTotal, this.physicalMemWired, this.physicalMemPercentage);
                Thread.Sleep(millisecondsTimeout);                          //pausar thread por cierto tiempo.
            }


            #endregion

            Console.WriteLine("Exiting: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// calcula valroes de memoria virtual y los almacena en las variables globales reservadas para esto
        /// valrores calculados: total, utilizada, porcentaje
        /// </summary>
        private void calcularMemoriaVirtual()
        {
            this.virtualMemPercentage = GetCounterValue(_memoryCounter, "Memory", "% Committed Bytes In Use", null);
            this.virtualMemWired = GetCounterValue(_memoryCounter, "Memory", "Committed Bytes", null);
            this.virtualMemTotal = GetCounterValue(_memoryCounter, "Memory", "Commit Limit", null);

            this.virtualMemTotal /= (1024 * 1024);
            this.virtualMemWired /= (1024 * 1024);

            this.virtualMemPercentage = Math.Round(this.virtualMemPercentage, 2);
            this.virtualMemTotal = Math.Round(this.virtualMemTotal, 2);
            this.virtualMemWired = Math.Round(this.virtualMemWired, 2);
        }

        /// <summary>
        /// calcula valores para memoria fisica
        /// valores obtenidos: total, usado, porcentaje.
        /// </summary>
        private void calcularMemoriaFisica()
        {
            //obtener memoria total disponible en sistema.
            String s = QueryComputerSystem("totalphysicalmemory");
            this.physicalMemTotal = Convert.ToDouble(s) / 1024 / 1024;

            //calcular memoria conectada
            double d = GetCounterValue(_memoryCounter, "Memory", "Available Bytes", null);
            this.physicalMemWired = physicalMemTotal - (d / 1024 / 1024);

            //calcular porcentaje
            this.physicalMemPercentage = physicalMemWired / physicalMemTotal;
            this.physicalMemPercentage *= 100;

            this.physicalMemWired = Math.Round(this.physicalMemWired, 2);
            this.physicalMemTotal = Math.Round(this.physicalMemTotal, 2);
            this.physicalMemPercentage = Math.Round(this.physicalMemPercentage, 2);
            Console.WriteLine(threadName + " <total>: " + physicalMemTotal);
        }

        /// <summary>
        /// obtiene el siguiente valor de pc para la categoria, nombre e instancia dadas.
        /// </summary>
        /// <param name="pc">performacen counter</param>
        /// <param name="categoryName">nombre de la categoria</param>
        /// <param name="counterName">informacion solicitada</param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        private double GetCounterValue(PerformanceCounter pc, string categoryName, string counterName, string instanceName)
        {
            pc.CategoryName = categoryName;
            pc.CounterName = counterName;
            pc.InstanceName = instanceName;
            return pc.NextValue();
        }

        /// <summary>
        /// funcion que realiza una consulta a Win32 para obtener informacion.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string QueryComputerSystem(string type)
        {
            string str = null;
            ManagementObjectSearcher objCS = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem");
            foreach (ManagementObject objMgmt in objCS.Get())
            {
                str = objMgmt[type].ToString();
            }
            return str;
        }
    }

}
