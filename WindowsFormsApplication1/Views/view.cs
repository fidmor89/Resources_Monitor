using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApplication1.Controllers;
using WindowsFormsApplication1.Models;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private viewController vc;      //controlador de la vista

        /// <summary>
        /// Constructor de la clase de vista
        /// inicializa componentes y controlador de vista (MVC)
        /// //llama funcion para el estilo de las graficas
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            vc = new viewController(this, 250);
            vc.Start();                                                     //iniciar controlador

            lookAndFeel();                                                  //estilo de graficas
        }

        /// <summary>
        /// Asigna estilos visuales a la interfaz grafica
        /// gruopbox, labels y graficas.
        /// </summary>
        private void lookAndFeel()
        {
            this.BackColor = Color.Black;

            #region GroupBox
            this.groupBox1.BackColor = Color.Black;
            this.groupBox1.ForeColor = Color.White;
            this.groupBox2.ForeColor = Color.White;
            this.groupBox3.ForeColor = Color.White;
            this.groupBox4.ForeColor = Color.White;
            this.groupBox5.ForeColor = Color.White;
            this.groupBox6.ForeColor = Color.White;
            this.groupBox7.ForeColor = Color.White;
            this.groupBox8.ForeColor = Color.White;
            #endregion

            #region Historial CPU
            ChartArea area = this.chartCpuHistory.ChartAreas[0];
            Series series = this.chartCpuHistory.Series[0];

            area.BackColor = Color.Black;
            area.AxisX.MajorGrid.LineColor = Color.DarkGreen;
            area.AxisY.MajorGrid.LineColor = Color.DarkGreen;
            this.chartCpuHistory.BackColor = Color.Black;
            series.Color = Color.Green;
            series.ChartType = SeriesChartType.StackedArea;
            #endregion

            #region CPU bar chart
            area = this.chartCpuActual.ChartAreas[0];
            series = this.chartCpuActual.Series[0];
            this.chartCpuActual.Series["CPU"].Points.Add(100);//iniciar en 0%

            area.BackColor = Color.Transparent;
            area.AxisX.MajorGrid.LineColor = Color.Transparent;
            area.AxisY.MajorGrid.LineColor = Color.Transparent;
            area.AxisX.LabelStyle.ForeColor = Color.Transparent;
            area.AxisX2.LineColor = Color.Transparent;

            chartCpuActual.BackColor = Color.Transparent;
            series.ChartType = SeriesChartType.Bar;
            #endregion

            #region pie RAM
            chartRamPie.Series[0].ChartType = SeriesChartType.Pie;
            area = this.chartRamPie.ChartAreas[0];
            area.BackColor = Color.Transparent;
            area.AxisY.MajorGrid.LineColor = Color.Transparent;
            chartRamPie.BackColor = Color.Transparent;
            #endregion

            #region Historial RAM
            barGreenStyle(this.chartRamHistory, false);
            #endregion

            #region Network & Disk Charts
            barGreenStyle(this.chartDiskReads, true);
            barGreenStyle(this.chartDiskWrites, true);
            barGreenStyle(this.chartNetIn, true);
            barGreenStyle(this.chartNetOut, true);
            #endregion
        }

        /// <summary>
        /// funcion para asignar el estilo de una grafica similar al taskmgr de windows 7
        /// fondo negro y grafica verde
        /// </summary>
        /// <param name="ch">grafica afectada</param>
        /// <param name="isColumn">true: hace grafica de columnas, false: no modifica el tipo de grafica</param>
        private void barGreenStyle(Chart ch, Boolean isColumn)
        {
            ChartArea area = ch.ChartAreas[0];
            Series series = ch.Series[0];
            ch.BackColor = Color.Black;
            area.BackColor = Color.Black;
            area.AxisX.MajorGrid.LineColor = Color.DarkGreen;
            area.AxisY.MajorGrid.LineColor = Color.DarkGreen;


            area.AxisY.LabelStyle.ForeColor = Color.White;
            if (isColumn)
            {
                series.Color = Color.Green;
                series.ChartType = SeriesChartType.Column;
            }
        }

        #region Setters
        /// <summary>
        /// metodo para asignar el uso de CPU
        /// redondea el valor a dos digitos hacia arriba  y utiliza un metodo auxiliar para asignarlo.
        /// </summary>
        public Double cpuUsage
        {
            set
            {
                value = Math.Round(value, 2);
                SetCpuUsage(value);
            }
        }

        /// <summary>
        /// metodo para asignar el del CPU que no esta uso.
        /// redondea el valor a dos digitos haci
        /// a arriba y utiliza un metodo auxiliar para asignarlo.
        /// </summary>
        public Double cpuIdle
        {
            set
            {
                value = Math.Round(value, 2);
                SetCpuIdle("Idle: " + value.ToString() + "%");
            }
        }

        /// <summary>
        /// metodo para asignar la velocidad de escritrua del disco.
        /// redondea el valor a dos digitos hacia arriba  y utiliza un metodo auxiliar para asignarlo.
        /// </summary>
        public Double diskWrite
        {
            set
            {
                value = Math.Round(value, 2);
                SetDiskWrites(value);
            }
        }

        /// <summary>
        /// metodo para asignar la velocidad de lectura del disco.
        /// Redondea el valor a dos digitos hacia arriba  y utiliza un metodo auxiliar para asignarlo.
        /// </summary>
        public Double diskReads
        {
            set
            {
                value = Math.Round(value, 2);
                SetDiskReads(value);
            }
        }

        /// <summary>
        /// Metodo para asignar la velocidad de bites enviados por red
        /// redondea el valor a dos digitos hacia arriba y utiliza un metodo auxiliar para asignarlo.
        /// </summary>
        public Double netOuts
        {
            set
            {
                value = Math.Round(value, 2);
                SetNetOuts(value);
            }
        }

        /// <summary>
        /// Metodo para asignar la velocidad de bites recibidos por red
        /// redondea el valor a dos digitos hacia arriba y utiliza un metodo auxiliar para asignarlo.
        /// </summary>
        public Double netIns
        {
            set
            {
                value = Math.Round(value, 2);
                SetNetIns(value);
            }
        }

        #endregion Setters

        #region Delegates para sincronizacion de vista

        /// <summary>
        /// Delegate utilizado para validar que se este actualizando la interfaz desde el thread de interfaz
        /// </summary>
        /// <param name="text">valor a asignar</param>
        delegate void SetTextCallback(string text);
        /// <summary>
        /// Delegate utilizado para validar que se este actualizando la interfaz desde el thread de interfaz
        /// </summary>
        /// <param name="text">valor a asignar</param>
        delegate void SetTextCallback3(Double value);

        /// <summary>
        /// Delegate utilizado para validar que se este actualizando la interfaz desde el thread de interfaz
        /// </summary>
        /// <param name="total"></param>
        /// <param name="usada"></param>
        /// <param name="porcentaje"></param>
        delegate void SetTextCallback2(Double total, Double usada, Double porcentaje);

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// </summary>
        /// <param name="text"></param>
        private void SetNetOuts(Double text)
        {
            if (this.groupBox8.InvokeRequired)
            {   //se intento asignar desde thread distinto.
                //invocar por medio de delegate
                SetTextCallback3 d = new SetTextCallback3(SetNetOuts);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { text });
                    }
                    catch (Exception ex)
                    {
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                        //Console.WriteLine(ex);
                    }
                }
            }
            else
            {
                this.groupBox8.Text = "Network Out: " + text.ToString() + "B/sec";
                try
                {
                    if (this.chartNetOut.Series["Series1"].Points.Count >= 30)
                    {
                        this.chartNetOut.Series["Series1"].Points.RemoveAt(0);
                        this.chartNetOut.Series["Series1"].Points.Add(text);
                    }
                    else
                    {
                        this.chartNetOut.Series["Series1"].Points.Add(text);
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// actualiza grafica y labels necesarios
        /// </summary>
        /// <param name="text"></param>
        private void SetNetIns(Double text)
        {
            if (this.groupBox7.InvokeRequired)
            {   //se intento asignar desde thread distinto.
                //invocar por medio de delegate
                SetTextCallback3 d = new SetTextCallback3(SetNetIns);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { text });
                    }
                    catch (Exception ex)
                    {
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                        //Console.WriteLine(ex);
                    }
                }

            }
            else
            {
                //debug
                //if (text != 0)
                //{
                //    this.chartNetIn.Series["Series1"].Points.AddY(text);
                //    MessageBox.Show(text.ToString());
                //}

                this.groupBox7.Text = "Network In: " + text.ToString() + "B/sec";
                try
                {
                    if (this.chartNetIn.Series["Series1"].Points.Count >= 30)
                    {
                        this.chartNetIn.Series["Series1"].Points.RemoveAt(0);
                        this.chartNetIn.Series["Series1"].Points.Add(text);

                    }
                    else
                    {

                        this.chartNetIn.Series["Series1"].Points.Add(text);
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }

            }
        }

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// actualiza grafica y labels necesarios
        /// </summary>
        /// <param name="text"></param>
        private void SetCpuIdle(string text)
        {
            if (this.label2.InvokeRequired)
            {   //se intento asignar desde thread distinto.
                //invocar por medio de delegate
                SetTextCallback d = new SetTextCallback(SetCpuIdle);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { text });
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex);
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                    }
                }
            }
            else
            {
                this.label2.Text = text;
            }
        }

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// actualiza grafica y labels necesarios
        /// </summary>
        /// <param name="text"></param>
        private void SetCpuUsage(Double value)
        {
            if (this.label1.InvokeRequired)
            {   //se intento asignar desde thread distinto.
                //invocar por medio de delegate
                SetTextCallback3 d = new SetTextCallback3(SetCpuUsage);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { value });
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex);
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                    }
                }
            }
            else
            {
                try
                {
                    this.label1.Text = "CPU Load: " + value.ToString() + "%";

                    if (this.chartCpuActual.Series["CPU"].Points.Count > 1)
                    {
                        this.chartCpuActual.Series["CPU"].Points.RemoveAt(0);
                    }
                    this.chartCpuActual.Series["CPU"].Points.Add(value);







                    #region Color Barra
                    if (value > 66)
                    {
                        this.chartCpuActual.Series[0].Color = Color.Red;
                    }
                    else if (value > 33)
                    {
                        this.chartCpuActual.Series[0].Color = Color.Orange;
                    }
                    else
                    {
                        this.chartCpuActual.Series[0].Color = Color.Green;
                    }
                    #endregion

                    if (this.chartCpuHistory.Series["CPU History"].Points.Count >= 30)
                    {
                        this.chartCpuHistory.Series["CPU History"].Points.RemoveAt(0);
                        this.chartCpuHistory.Series["CPU History"].Points.AddY(value);
                    }
                    else
                    {
                        this.chartCpuHistory.Series["CPU History"].Points.AddY(value);
                    }


                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// actualiza grafica y labels necesarios
        /// </summary>
        /// <param name="text"></param>
        private void SetDiskReads(Double text)
        {
            if (this.groupBox6.InvokeRequired)
            {   //se intento asignar desde thread distinto.
                //invocar por medio de delegate
                SetTextCallback3 d = new SetTextCallback3(SetDiskReads);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { text });
                    }
                    catch (Exception e)
                    {
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                        //Console.WriteLine(e);
                    }
                }
            }
            else
            {
                this.groupBox6.Text = "Disk Reads: " + text.ToString() + "B/sec";
                try
                {
                    if (this.chartDiskReads.Series["Series1"].Points.Count >= 30)
                    {
                        this.chartDiskReads.Series["Series1"].Points.RemoveAt(0);
                        this.chartDiskReads.Series["Series1"].Points.AddY(text);
                    }
                    else
                    {
                        this.chartDiskReads.Series["Series1"].Points.AddY(text);
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// actualiza grafica y labels necesarios
        /// </summary>
        /// <param name="text"></param>
        private void SetDiskWrites(Double text)
        {
            if (this.groupBox5.InvokeRequired)
            {   //se intento asignar desde thread distinto.
                //invocar por medio de delegate
                SetTextCallback3 d = new SetTextCallback3(SetDiskWrites);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { text });
                    }
                    catch (Exception ex)
                    {
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                        //Console.WriteLine(ex);
                    }
                }
            }
            else
            {
                this.groupBox5.Text = "Disk Writes: " + text.ToString() + "B/sec";
                try
                {
                    if (this.chartDiskWrites.Series["Series1"].Points.Count >= 30)
                    {
                        this.chartDiskWrites.Series["Series1"].Points.RemoveAt(0);
                        this.chartDiskWrites.Series["Series1"].Points.AddY(text);
                    }
                    else
                    {
                        this.chartDiskWrites.Series["Series1"].Points.AddY(text);
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }

            }
        }

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// actualiza grafica y labels necesarios
        /// </summary>
        /// <param name="text"></param>
        public void SetRamVirtual(Double total, Double usada, Double porcentaje)
        {

            if (this.ramVirtualLbl.InvokeRequired)
            {
                SetTextCallback2 d = new SetTextCallback2(SetRamVirtual);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { total, usada, porcentaje });
                    }
                    catch (Exception ex)
                    {
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                        //Console.WriteLine(ex);
                    }
                }
            }
            else
            {
                try
                {

                    if (this.chartRamHistory.Series["RAM V History"].Points.Count >= 30)
                    {
                        this.chartRamHistory.Series["RAM V History"].Points.RemoveAt(0);
                        this.chartRamHistory.Series["RAM V History"].Points.Add(porcentaje);
                    }
                    else
                    {
                        this.chartRamHistory.Series["RAM V History"].Points.Add(porcentaje);
                    }


                    String s = "Memoria Virtual: " + usada + "/" + total + " MB (" + porcentaje + "%)";
                    this.ramVirtualLbl.Text = s;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// metodo auxiliar para actualizar interfaz de usuario.
        /// verifica si fue invocada por otro thread y de ser asi obtiene la firma del thread de de UX y se invoca a si mismo por medio del delegate 
        /// actualiza grafica y labels necesarios
        /// </summary>
        /// <param name="text"></param>
        public void SetRamFisica(Double total, Double usada, Double porcentaje)
        {

            if (this.ramFisicaLbl.InvokeRequired)
            {
                SetTextCallback2 d = new SetTextCallback2(SetRamFisica);
                if (!this.IsDisposed)
                {
                    try
                    {
                        this.Invoke(d, new object[] { total, usada, porcentaje });
                    }
                    catch (Exception ex)
                    {
                        //se intento invocar metodo en vista cuando esta ya habia sido recolectada por GC
                        //Console.WriteLine(ex);
                    }
                }
            }
            else
            {
                try
                {
                    if (this.chartRamHistory.Series["RAM F History"].Points.Count >= 30)
                    {
                        this.chartRamHistory.Series["RAM F History"].Points.RemoveAt(0);
                        this.chartRamHistory.Series["RAM F History"].Points.Add(porcentaje);
                    }
                    else
                    {
                        this.chartRamHistory.Series["RAM F History"].Points.Add(porcentaje);
                    }

                    this.chartRamPie.Series["RAM"].Points.Clear();
                    this.chartRamPie.Series["RAM"].Points.Add(total - usada);   //memoria libre
                    this.chartRamPie.Series["RAM"].Points.Add(usada);

                    String s = "Memoria Fisica: " + usada + "/" + total + " MB (" + porcentaje + "%)";
                    this.ramFisicaLbl.Text = s;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                }
            }
        }

        #endregion delegates
    }
}
