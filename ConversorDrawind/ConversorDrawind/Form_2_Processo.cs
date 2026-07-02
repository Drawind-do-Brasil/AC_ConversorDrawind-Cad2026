using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ACAD = Autodesk.AutoCAD.Interop;
using System.Threading;

namespace ConversorDrawind
{
    public partial class Form_2_Processo : Form
    {
        Param1 parametros = new Param1();
        private Thread acadThreadRun = null;
        private static bool isCanceled = false;
        private static DateTime startTime;
        public static string tempo = "";
        public Form_2_Processo(Param1 p)
        {
            InitializeComponent();
            this.TopMost = true;
            isCanceled = false;
            parametros = p;
            startTime = DateTime.Now;
            BackgroundWorker1.RunWorkerAsync();
        }

        public void SetTopLevelInfUser(bool valor)
        {
            this.SetTopLevel(valor);
        }

        private int ad = 0;

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (parametros.desenhosName.Count() > 0)
            {
                ad = 0;
                DrawingProcess.Valor = 0;
                DrawingProcess.Index = 0;

                
                acadThreadRun = new Thread(new ParameterizedThreadStart(DrawingProcess.GoProcess));
                acadThreadRun.IsBackground = true;
                acadThreadRun.SetApartmentState(ApartmentState.STA);
                acadThreadRun.Start(parametros);
                
                while (DrawingProcess.Valor < 100)
                {
                    BackgroundWorker1.ReportProgress(DrawingProcess.Valor);
                    Thread.Sleep(50);
                }
                acadThreadRun.Join();
            }
        }


        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (ProgressBar1.Value != e.ProgressPercentage)
            {
                Label_TempoRestante.Text = "Tempo Restante: " + TimeRemaining();
                ProgressBar1.Value = e.ProgressPercentage;
                //ProgressBar1.Refresh();
                //ProgressBar1.CreateGraphics().DrawString(e.ProgressPercentage + "%", new Font("Arial", (float)8.25, FontStyle.Regular), Brushes.Black, new PointF(ProgressBar1.Width / 2 - 10, ProgressBar1.Height / 2 - 7));
               
            }
            if (DrawingProcess.IsACADOpen && !Button_Cancel.Enabled)
                Button_Cancel.Enabled = true;
            Label_TempDecorrido.Text = "Tempo Decorrido: " + Time();
            if (!String.IsNullOrWhiteSpace(DrawingProcess.FileOpen))
                Label_Processo.Text = "Convertendo o desenho: " + DrawingProcess.FileOpen;

            if (ad == 0)
            {
                label1.Text = "Convertendo";
            }
            else if (ad == 1)
            {
                label1.Text = "Convertendo.";
            }
            else if (ad == 2)
            {
                label1.Text = "Convertendo..";
            }
            else if (ad == 3)
            {
                label1.Text = "Convertendo...";
            }
            else if (ad == 4)
            {
                label1.Text = "Convertendo....";
            }
            else if (ad == 5)
            {
                label1.Text = "Convertendo.....";
            }
            else if (ad == 6)
            {
                label1.Text = "Convertendo......";
            }
            else if (ad == 7)
            {
                label1.Text = "Convertendo.......";
                ad = -1;
            }
            ad++;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }

        private void Button_Cancel_Click(object sender, EventArgs e)
        {
            isCanceled = true;
            BackgroundWorker1.CancelAsync();
            this.Close();
        }

        public static bool IsCanceled
        {
            get { return isCanceled; }
        }

        public string Time()
        {
            tempo = new DateTime(TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks).Ticks).ToString("HH:mm:ss");
            return tempo;
        }
        
        public  string TimeRemaining()
        {
            TimeSpan timeRemaining = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (parametros.desenhosName.Count() - (DrawingProcess.Index + 1)) / (DrawingProcess.Index + 1));
            return new DateTime(timeRemaining.Ticks).ToString("HH:mm:ss");
    
        }
    }
}
