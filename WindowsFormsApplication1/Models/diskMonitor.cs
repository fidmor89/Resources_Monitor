using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Models
{
    class diskMonitor
    {
        /// <summary>
        /// Parametro para tiempo de sleep del thread
        /// </summary>
        private int millisecondsTimeout;
        /// <summary>
        /// nombre del thread
        /// </summary>
        private static String threadName = "Thread-Disk-Monitor";
        /// <summary>
        /// parametro para almacenar referencia al contexto de la vista.
        /// </summary>
        private Form1 viewContext;

        /// <summary>
        /// componente de windows NT para medir el performance de la PC (lectura en disco)
        /// </summary>
        private PerformanceCounter _diskReadCounter = new PerformanceCounter();
        /// <summary>
        /// componente de windows NT para medir el performance de la PC (escritura en disco)
        /// </summary>
        private PerformanceCounter _diskWriteCounter = new PerformanceCounter();

        /// <summary>
        /// diccionario para seleccionar tipo de acceso a medir.
        /// </summary>
        public enum tipoOperacionDisco { ReadAndWrite, Read, Write };

        /// <summary>
        /// variable que mantiene la ultima lectura en el perfrmacne counter
        /// </summary>
        private Double diskReads;
        /// <summary>
        /// variable que mantiene la ultima lectura en el perfrmacne counter
        /// </summary>
        private Double diskWrites;

        /// <summary>
        /// Construcctor de la clase
        /// </summary>
        /// <param name="intervalTimeOut">tiempo para sleep</param>
        /// <param name="cte">contexto de interfaz</param>
        public diskMonitor(int intervalTimeOut, Form1 cte)
        {
            this.millisecondsTimeout = intervalTimeOut;
            this.viewContext = cte;
        }

        /// <summary>
        /// crea un thread y lo inicia
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Starting: " + threadName);
            Thread newThread = new Thread(new ThreadStart(Run));
            newThread.Start();
        }

        /// <summary>
        /// consulta la informacion y la manda a mostrar mientras exista la ventana.
        /// </summary>
        private void Run()
        {
            Thread.CurrentThread.Name = threadName;
            Console.WriteLine("Running: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);

            #region

            while (!viewContext.IsDisposed)
            {
                this.diskReads = GetDiskData(tipoOperacionDisco.Read);
                Console.WriteLine(threadName + " <reads>: " + diskReads);
                this.diskWrites = GetDiskData(tipoOperacionDisco.Write);
                Console.WriteLine(threadName + " <writes>: " + diskWrites);

                this.viewContext.diskReads = this.diskReads;
                this.viewContext.diskWrite = this.diskWrites;

                Thread.Sleep(millisecondsTimeout);                          //pausar thread por cierto tiempo.
            }


            #endregion

            Console.WriteLine("Exiting: " + Thread.CurrentThread.Name + " ID: " + Thread.CurrentThread.ManagedThreadId);
        }

        /// <summary>
        /// funcion para obtener la velodicidad del disco, funciona para lectura, escritura, y lectura/escritura.
        /// </summary>
        /// <param name="tipoAcceso">indica tipo de acceso.</param>
        /// <returns>double con informacion solicitada en Bytes/sec</returns>
        public double GetDiskData(tipoOperacionDisco tipoAcceso)
        {
            return tipoAcceso == tipoOperacionDisco.Read ?
                        GetCounterValue(_diskReadCounter, "PhysicalDisk", "Disk Read Bytes/sec", "_Total") :
                    tipoAcceso == tipoOperacionDisco.Write ?
                        GetCounterValue(_diskWriteCounter, "PhysicalDisk", "Disk Write Bytes/sec", "_Total") :
                    tipoAcceso == tipoOperacionDisco.ReadAndWrite ?
                        GetCounterValue(_diskReadCounter, "PhysicalDisk", "Disk Read Bytes/sec", "_Total") +
                        GetCounterValue(_diskWriteCounter, "PhysicalDisk", "Disk Write Bytes/sec", "_Total") :
                    0;
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
