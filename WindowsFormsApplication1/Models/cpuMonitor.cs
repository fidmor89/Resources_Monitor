using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace WindowsFormsApplication1.Models
{
    class cpuMonitor
    {
        /// <summary>
        /// tiempo para mandar al thread a sleep en cada ciclo.
        /// </summary>
        private int millisecondsTimeout;
        
        /// <summary>
        /// nombre del thread
        /// </summary>
        private static String threadName = "Thread-CPU-Monitor";
        
        /// <summary>
        /// referencia a la interfaz para acuatilizacion de datos y verificacion cuando esta sea recolectada por el GC
        /// </summary>
        private Form1 viewContext;

        /// <summary>
        /// componente de windows NT para contabilizar el performance de la PC
        /// </summary>
        PerformanceCounter cpuCounter = new PerformanceCounter();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="intervalTimeOut">tiempo de sleep</param>
        /// <param name="cte">referencia al contexto de UX</param>
        public cpuMonitor(int intervalTimeOut, Form1 cte)
        {
            this.millisecondsTimeout = intervalTimeOut;
            this.viewContext = cte;
        }

        /// <summary>
        /// metodo para iniciar el treath
        /// crea el nuevo thread y asigna metodo de ejeccucion (run)
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Starting: " + threadName);
            Thread newThread = new Thread(new ThreadStart(Run));
            newThread.Start();
        }

        /// <summary>
        /// flujo principal del thread.
        /// mientras la vista exista, obtiene la informacion y la muestra.
        /// duerme el thread en cada iteracion.
        /// si la vista es recolectada por el GC el thread termina.
        /// </summary>
        private void Run()
        {
            Thread.CurrentThread.Name = threadName;
            Console.WriteLine("Running: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);

            #region
            while (!viewContext.IsDisposed)
            {
                Double usageP = this.GetProcessorData();
                this.viewContext.cpuUsage = usageP;
                this.viewContext.cpuIdle = 100.0 - usageP;
                Console.WriteLine( threadName + ": " + usageP);

                Thread.Sleep(millisecondsTimeout);                          //pausar thread por cierto tiempo.
            }
            #endregion

            Console.WriteLine("Exiting: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// wrapper para simplicidad de funcion GetCounterValue
        /// </summary>
        /// <returns>Double con valor del total del procesador</returns>
        public Double GetProcessorData()
        {
            Double d = GetCounterValue(cpuCounter, "Processor", "% Processor Time", "_Total");
            return d;
        }

        /// <summary>
        /// obtiene el siguiente valor de pc para la categoria, nombre e instancia dadas.
        /// </summary>
        /// <param name="pc">performacen counter</param>
        /// <param name="categoryName">nombre de la categoria</param>
        /// <param name="counterName">informacion solicitada</param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        double GetCounterValue(PerformanceCounter pc, string categoryName, string counterName, string instanceName)
        {
            pc.CategoryName = categoryName;
            pc.CounterName = counterName;
            pc.InstanceName = instanceName;
            return pc.NextValue();
        }
    }
}
