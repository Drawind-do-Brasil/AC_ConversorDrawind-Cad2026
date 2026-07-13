using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;

namespace ConversorDrawind.UI.Wpf.Conversion
{
    public partial class ConversionProgressWindow : Window
    {
        private readonly BackgroundWorker backgroundWorker = new BackgroundWorker();
        private readonly Param1 parametros;
        private Thread acadThreadRun = null;
        private DateTime startTime;
        private int animationIndex;

        public ConversionProgressWindow(Param1 p)
        {
            InitializeComponent();
            VersionTextBlock.Text = ApplicationInfo.ProgramVersionText;
            parametros = p;
            startTime = DateTime.Now;

            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorkerDoWork;
            backgroundWorker.ProgressChanged += BackgroundWorkerProgressChanged;
            backgroundWorker.RunWorkerCompleted += BackgroundWorkerRunWorkerCompleted;
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            if (parametros.desenhosName.Count() > 0)
            {
                animationIndex = 0;
                DrawingProcess.Valor = 0;
                DrawingProcess.Index = 0;

                acadThreadRun = new Thread(new ParameterizedThreadStart(DrawingProcess.GoProcess));
                acadThreadRun.IsBackground = true;
                acadThreadRun.SetApartmentState(ApartmentState.STA);
                acadThreadRun.Start(parametros);

                while (DrawingProcess.Valor < 100)
                {
                    backgroundWorker.ReportProgress(DrawingProcess.Valor);
                    Thread.Sleep(50);
                }

                acadThreadRun.Join();
            }
        }

        private void BackgroundWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progress = Math.Max(0, Math.Min(100, e.ProgressPercentage));
            if ((int)ProgressBar.Value != progress)
            {
                RemainingTimeTextBlock.Text = "Tempo Restante: " + TimeRemaining();
                ProgressBar.Value = progress;
            }

            if (DrawingProcess.IsACADOpen && !CancelButton.IsEnabled)
            {
                CancelButton.IsEnabled = true;
            }

            ElapsedTimeTextBlock.Text = "Tempo Decorrido: " + Time();
            if (!string.IsNullOrWhiteSpace(DrawingProcess.FileOpen))
            {
                ProcessTextBlock.Text = "Convertendo o desenho: " + DrawingProcess.FileOpen;
            }

            StatusTextBlock.Text = Localization.LabelConverting + new string('.', animationIndex + 1).PadRight(3);
            animationIndex++;
            if (animationIndex > 2)
            {
                animationIndex = 0;
            }
        }

        private void BackgroundWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Processo.SetCanceled(true);
            backgroundWorker.CancelAsync();
            Close();
        }

        public string Time()
        {
            string elapsed = new DateTime(TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks).Ticks).ToString("HH:mm:ss");
            Processo.SetTempo(elapsed);
            return elapsed;
        }

        public string TimeRemaining()
        {
            TimeSpan timeRemaining = TimeSpan.FromTicks(DateTime.Now.Subtract(startTime).Ticks * (parametros.desenhosName.Count() - (DrawingProcess.Index + 1)) / (DrawingProcess.Index + 1));
            return new DateTime(timeRemaining.Ticks).ToString("HH:mm:ss");
        }
    }
}



