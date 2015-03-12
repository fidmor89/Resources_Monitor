using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Models
{
    class networkMonitor
    {
        /// <summary>
        /// tiempo de espera.
        /// </summary>
        private int millisecondsTimeout;
        /// <summary>
        /// nombre de thread
        /// </summary>
        private static String threadName = "Thread-Network-Monitor";
        //refrencia a vista
        private Form1 viewContext;

        /// <summary>
        /// diccionario para seleccionar tipo de informacion a contar.
        /// </summary>
        private enum NetData { ReceivedAndSent, Received, Sent };
        /// <summary>
        /// listado de interfaces de red
        /// </summary>
        string[] instanceNames;
        PerformanceCounter[] _netRecvCounters;
        PerformanceCounter[] _netSentCounters;
        private Double inData;
        private Double outData;

        /// <summary>
        /// contructor.
        /// obtiene y almacena las interfaces de red.
        /// </summary>
        /// <param name="p">tiempo para sleep en cada iteracion</param>
        /// <param name="cte">referencia a vista.</param>
        public networkMonitor(int p, Form1 cte)
        {
            this.millisecondsTimeout = p;
            this.viewContext = cte;

            PerformanceCounterCategory cat = new PerformanceCounterCategory("Network Interface");
            instanceNames = cat.GetInstanceNames();
            _netRecvCounters = new PerformanceCounter[instanceNames.Length];
            for (int i = 0; i < instanceNames.Length; i++)
                _netRecvCounters[i] = new PerformanceCounter();

            _netSentCounters = new PerformanceCounter[instanceNames.Length];
            for (int i = 0; i < instanceNames.Length; i++)
                _netSentCounters[i] = new PerformanceCounter();

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
        /// mientras la vista exista:
        /// obtiene informacion de datos enviados y recibidos y los manda a mostrar.
        /// si la interfaz es cerrada el thread termina
        /// pausa en cada iteracion.
        /// </summary>
        private void Run()
        {
            Thread.CurrentThread.Name = threadName;
            Console.WriteLine("Running: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);

            #region

            while (!viewContext.IsDisposed)
            {
                Console.WriteLine("network");

                this.outData = GetNetData(NetData.Sent);
                Console.WriteLine(threadName + " <out>: " + outData);
                this.inData = GetNetData(NetData.Sent);
                Console.WriteLine(threadName + " <int>: " + inData);

                this.viewContext.netOuts = outData;
                this.viewContext.netIns = inData;

                Thread.Sleep(millisecondsTimeout);                          //pausar thread por cierto tiempo.

            }


            #endregion

            Console.WriteLine("Exiting: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// obtiene informacion de red
        /// </summary>
        /// <param name="nd">tipo de datos(recibido, enviado o ambos)</param>
        /// <returns></returns>
        private double GetNetData(NetData nd)
        {
            if (instanceNames.Length == 0)
                return 0;

            double d = 0;
            for (int i = 0; i < instanceNames.Length; i++)
            {
                d += nd == NetData.Received ?
                        GetCounterValue(_netRecvCounters[i], "Network Interface", "Bytes Received/sec", instanceNames[i]) :
                    nd == NetData.Sent ?
                        GetCounterValue(_netSentCounters[i], "Network Interface", "Bytes Sent/sec", instanceNames[i]) :
                    nd == NetData.ReceivedAndSent ?
                        GetCounterValue(_netRecvCounters[i], "Network Interface", "Bytes Received/sec", instanceNames[i]) +
                        GetCounterValue(_netSentCounters[i], "Network Interface", "Bytes Sent/sec", instanceNames[i]) :
                    0;
            }

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
        private double GetCounterValue(PerformanceCounter pc, string categoryName, string counterName, string instanceName)
        {
            pc.CategoryName = categoryName;
            pc.CounterName = counterName;
            pc.InstanceName = instanceName;
            return pc.NextValue();
        }
    }
}
