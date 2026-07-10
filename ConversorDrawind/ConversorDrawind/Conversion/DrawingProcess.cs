using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using ACAD = Autodesk.AutoCAD.Interop;
using ACCOMMON = Autodesk.AutoCAD.Interop.Common;

namespace ConversorDrawind
{
    class DrawingProcess
    {
        public static readonly string DLLPath1 = DrawingProcessPaths.DllPath;
        private static int valor;
        private static int index;
        private static string fileOpen;
        private static bool isACADOpen = false;
        private static Param1 parametros;
        private ACAD.AcadApplication acadApplication = null;
        private ACAD.AcadDocument acadDocument = null;

        private static bool _RunCommand = false;
        private const int CommandTimeoutMs = 900000;
        private const int CommandPollMs = 50;

        List<ACAD.AcadDocument> _desenhoAtual = new List<ACAD.AcadDocument>();
        ACAD.AcadDocument _desenhoAtributado = null;
        private static DrawingProcess myClass = new DrawingProcess();

        const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_ENTER = 0x0D;

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        private const uint KEYEVENTF_KEYUP = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void KeybdEvent(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwProcessId);



        private static Int32 GetWindowProcessID(Int32 hwnd)
        {
            Int32 pid = 1;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;

        }

        public static void PressESC(IntPtr mwh)
        {
            SetForegroundWindow(mwh);
            Thread.Sleep(100);
            KeybdEvent(VK_ENTER, 0, 0, UIntPtr.Zero);
            KeybdEvent(VK_ENTER, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool EnumThreadWindows(int threadId, EnumWindowsProc callback, IntPtr lParam);

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumChildWindows(IntPtr hwndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);



        private static IntPtr FindWindowInThread(int threadId, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;
            EnumThreadWindows(threadId, (hWnd, lParam) =>
            {
                StringBuilder text = new StringBuilder(200);
                GetWindowText(hWnd, text, 200);
                if (compareTitle(text.ToString()))
                {
                    windowHandle = hWnd;
                    return false;
                }
                return true;
            }, IntPtr.Zero);

            return windowHandle;
        }

        public static IntPtr FindWindowInProcess(Process process, Func<string, bool> compareTitle)
        {
            IntPtr windowHandle = IntPtr.Zero;

            foreach (ProcessThread t in process.Threads)
            {
                windowHandle = FindWindowInThread(t.Id, compareTitle);
                if (windowHandle != IntPtr.Zero)
                {
                    break;
                }
            }

            return windowHandle;
        }

        private static ACAD.AcadApplication CreateAutoCADApplication()
        {
            string[] progIds =
            {
                "AutoCAD.Application.25.1",
                "AutoCAD.Application.25",
                "AutoCAD.Application"
            };

            COMException lastComException = null;

            foreach (string progId in progIds)
            {
                Type acadType = Type.GetTypeFromProgID(progId);

                if (acadType == null)
                    continue;

                try
                {
                    return (ACAD.AcadApplication)Activator.CreateInstance(acadType);
                }
                catch (COMException ex)
                {
                    lastComException = ex;
                }
            }

            if (lastComException != null)
                throw new InvalidOperationException("Não foi possível iniciar o AutoCAD 2026 via COM. Verifique se o AutoCAD 2026 está instalado, ativado e registrado no Windows.", lastComException);

            throw new InvalidOperationException("AutoCAD 2026 não encontrado no registro COM do Windows.");
        }




        private static void OpenACAD()
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    myClass._desenhoAtual = new List<ACAD.AcadDocument>();

                    myClass.acadApplication = ComRetry.Invoke(() => CreateAutoCADApplication(), 180, 250);

                    Process processo = Process.GetProcessById(GetWindowProcessID(ComRetry.Invoke(() => (int)myClass.acadApplication.HWND)));

                    IntPtr hWnd = FindWindowInProcess(processo, s => s.EndsWith("acad"));

                    if (hWnd != IntPtr.Zero)
                    {
                        PressESC(hWnd);
                    }

                    myClass.acadApplication.BeginCommand += AcadApplication_BeginCommand;
                    myClass.acadApplication.EndCommand += AcadApplication_EndCommand;

                    IsACADOpen = true;
                    ComRetry.Invoke(() => myClass.acadApplication.WindowState = ACCOMMON.AcWindowState.acMax);

                    myClass._desenhoAtual.Add(ComRetry.Invoke(() => myClass.acadApplication.Documents.Add("Novo Desenho 1"), 120, 100));

                    myClass.acadDocument = myClass._desenhoAtual.First();
                    ComRetry.Invoke(() => myClass.acadDocument.WindowState = ACCOMMON.AcWindowState.acMax);

                    if (parametros.configuration.General.ExchangeFormat)
                    {
                        try
                        {
                            string formatoPath = DrawingProcessPaths.GetExchangeFormatPath(parametros.configuration);

                            myClass.acadDocument = ComRetry.Invoke(() => myClass.acadApplication.Documents.Open(formatoPath, false), 120, 100);
                            ComRetry.Invoke(() => myClass.acadDocument.Application.ZoomExtents());
                            myClass._desenhoAtributado = myClass.acadDocument;
                        }
                        catch (Exception)
                        {
                            throw new ArgumentException("Não foi possível encontrar o formato atributado: " + parametros.configuration.Blocks.TeklaBlockPath);
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void AcadApplication_EndCommand(string CommandName)
        {
            _RunCommand = false;
        }

        private static void AcadApplication_BeginCommand(string CommandName)
        {
            _RunCommand = true;
        }

        private static int GetCommandActive()
        {
            try
            {
                if (myClass.acadDocument == null)
                    return 0;

                object cmdActive = ComRetry.Invoke(() => myClass.acadDocument.GetVariable("CMDACTIVE"));
                return Convert.ToInt32(cmdActive);
            }
            catch (Exception)
            {
                return _RunCommand ? 1 : 0;
            }
        }

        private static void WaitCommandFinished(string commandName)
        {
            Stopwatch sw = Stopwatch.StartNew();
            int idleReadCount = 0;

            while (sw.ElapsedMilliseconds < CommandTimeoutMs)
            {
                if (GetCommandActive() == 0)
                {
                    idleReadCount++;

                    if (idleReadCount >= 3)
                    {
                        _RunCommand = false;
                        return;
                    }
                }
                else
                {
                    idleReadCount = 0;
                }


                Thread.Sleep(CommandPollMs);
            }

            throw new TimeoutException("Tempo limite aguardando execução do comando: " + commandName);
        }

        public void LoadFile(string file)
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    ComRetry.Invoke(() => myClass.acadDocument.SetVariable("FILEDIA", 0));
                    try
                    {
                        if (!String.IsNullOrEmpty(file))
                        {
                            myClass.SendCommand(DrawingCommandBuilder.BuildLoadFileCommand(file));
                        }
                    }
                    finally
                    {
                        ComRetry.Invoke(() => myClass.acadDocument.SetVariable("FILEDIA", 1));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SendCommand(string CommandName)
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    _RunCommand = true;

                    ComRetry.Invoke(() => myClass.acadApplication.ActiveDocument = myClass.acadDocument);
                    ComRetry.Invoke(() => myClass.acadDocument.SendCommand(CommandName));
                    WaitCommandFinished(CommandName);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Teste" + ex.Message);
                throw;
            }
        }

        private static void ShowAutoCADCommandLine()
        {
            try
            {
                if (myClass.acadApplication == null || myClass.acadDocument == null)
                    return;

                myClass.SendCommand(DrawingCommandBuilder.BuildCommandLineCommand());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void CloseACAD()
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    if (myClass.acadApplication == null)
                        return;

                    if (parametros.configuration.General.ExchangeFormat && myClass._desenhoAtributado != null)
                    {
                        myClass.acadDocument = myClass._desenhoAtributado;
                        ComRetry.Invoke(() => myClass.acadDocument.Close(false));
                    }

                    if (myClass._desenhoAtual.Count > 0)
                    {
                        myClass.acadDocument = myClass._desenhoAtual.First();
                        ComRetry.Invoke(() => myClass.acadApplication.ActiveDocument = myClass.acadDocument);
                        ComRetry.Invoke(() => myClass.acadDocument.Close(false));
                        myClass._desenhoAtual.RemoveAt(0);
                    }

                    if (!parametros.closedesenhos)
                    {
                        try
                        {
                            if (ComRetry.Invoke(() => myClass.acadApplication.Documents.Count) == 0)
                                ComRetry.Invoke(() => myClass.acadApplication.Quit(), 120, 100);
                        }
                        catch (Exception e)
                        {
                            System.Windows.MessageBox.Show(e.Message,
                                             Localization.TitleWarningNoExclamation,
                                             System.Windows.MessageBoxButton.OK,
                                             System.Windows.MessageBoxImage.Warning);
                        }
                    }
                }

            }
            catch (Exception)
            {

            }
        }

        private static void RunCommand(string file, bool last = false)
        {
            if (File.Exists(file))
            {
                try
                {
                    File.Copy(file, DrawingProcessPaths.GetBackupPath(file), true);
                }
                catch (Exception)
                {

                }

                myClass.acadDocument = ComRetry.Invoke(() => myClass.acadApplication.Documents.Open(file, false), 120, 100);
                myClass._desenhoAtual.Add(myClass.acadDocument);
                ComRetry.Invoke(() => myClass.acadApplication.ActiveDocument = myClass.acadDocument);

                try
                {
                    myClass.SendCommand("ZOOM E\n");
                    myClass.SendCommand("CDwi_Convert\n");
                    if (parametros.configuration.General.ExchangeFormat)
                    {
                        myClass.SendCommand("CDwi_GetAttributeText\n");

                        myClass.acadDocument = myClass._desenhoAtributado;
                        ComRetry.Invoke(() => myClass.acadApplication.ActiveDocument = myClass.acadDocument);

                        myClass.SendCommand("COPYBASE 0,0,0 all \n");

                        myClass.acadDocument = myClass._desenhoAtual.Last();
                        ComRetry.Invoke(() => myClass.acadApplication.ActiveDocument = myClass.acadDocument);

                        myClass.SendCommand("ZOOM E\n");
                        myClass.SendCommand("REGEN\n");
                        string pasteClipPoint = GetPasteClipInsertionPoint();

                        if (parametros.configuration.General.SourceMode == 1)
                        {

                            List<Block> listAnterior = GetListBlocksS();

                            myClass.SendCommand("PASTECLIP " + pasteClipPoint + "\n");

                            List<Block> listPosterior = GetListBlocksS();

                            for (int j = 0; j < listPosterior.Count; j++)
                            {
                                foreach (var item in listAnterior)
                                {
                                    if (listPosterior[j].blockName == item.blockName)
                                    {
                                        listPosterior.RemoveAt(j);
                                        j--;
                                        break;
                                    }
                                }
                            }
                            if (listPosterior.Count() > 0)
                            {
                                string comando = listPosterior.First().blockName.Replace(" ", "*******");
                                myClass.SendCommand("CDwi_ScaleBlock\n" + comando + "\n");
                            }
                        }

                        else
                        {
                            myClass.SendCommand("PASTECLIP " + pasteClipPoint + "\n");

                        }

                        myClass.SendCommand("CDwi_AttributeBlock\n");

                        if (parametros.configuration.General.SourceMode == 1)
                        {
                            myClass.SendCommand("CDwi_DeleteBlocks\n");
                        }
                    }

                    if (parametros.configuration.General.ConvertLayers)
                    {
                        myClass.SendCommand("CDwi_DeleteLayers\n");
                    }

                    if (parametros.configuration.General.ApplyDrawingScale)
                    {
                        ApplicationRuntime.ControladorT2 = false;

                        myClass.SendCommand("CDwi_Scale\n");

                        ApplicationRuntime.ControladorT2 = true;
                    }


                    if (parametros.configuration.General.ExecuteLisp)
                    {
                        ComRetry.Invoke(() => myClass.acadDocument.SetVariable("FILEDIA", 0));
                        try
                        {
                            foreach (string item in parametros.configuration.Commands.LispCommands)
                            {
                                string[] value = item.Split('@');
                                if (value.Count() != 3)
                                {
                                    myClass.LoadFile(value[1]);
                                    myClass.SendCommand(value[0] + "\n");
                                }
                            }
                            if (last)
                                foreach (string item in parametros.configuration.Commands.LispCommands)
                                {

                                    string[] value = item.Split('@');
                                    if (value.Count() == 3)
                                    {
                                        myClass.LoadFile(value[1]);
                                        myClass.SendCommand(value[0] + "\n");
                                    }

                                }
                        }
                        finally
                        {
                            ComRetry.Invoke(() => myClass.acadDocument.SetVariable("FILEDIA", 1));
                        }
                    }

                    myClass.SendCommand("CDwi_Finalize\n");
                    ComRetry.Invoke(() => myClass.acadDocument.Application.ZoomExtents());

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);

                }
                finally
                {

                }



                try
                {
                    try
                    {
                        if (ApplicationRuntime.ExtensaoGeral == "DWG")
                            ComRetry.Invoke(() => myClass.acadDocument.Save());

                        else
                        {
                            myClass.SendCommand("SaveDXF\n");
                        }

                    }
                    catch (Exception)
                    {
                        ApplicationRuntime.ControladorT2 = false;
                        System.Windows.MessageBox.Show(
                                    Localization.MessageCouldNotSaveFile,
                                    Localization.TitleWarningNoExclamation,
                                    System.Windows.MessageBoxButton.OK,
                                    System.Windows.MessageBoxImage.Warning);
                        ApplicationRuntime.ControladorT2 = true;
                    }

                    if (!parametros.closedesenhos)
                    {
                        int cont = ComRetry.Invoke(() => myClass.acadApplication.Documents.Count);

                        ComRetry.Invoke(() => myClass.acadDocument.Close());

                        Stopwatch waitClose = Stopwatch.StartNew();
                        while (waitClose.ElapsedMilliseconds < 30000)
                        {
                            if (ComRetry.Invoke(() => myClass.acadApplication.Documents.Count) < cont)
                                break;

                            Thread.Sleep(50);
                        }

                        myClass._desenhoAtual.RemoveAt(myClass._desenhoAtual.Count - 1);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(
                                     e.Message,
                                     Localization.TitleWarningNoExclamation,
                                     System.Windows.MessageBoxButton.OK,
                                     System.Windows.MessageBoxImage.Warning);
                }

            }
            else
            {
                System.Windows.MessageBox.Show(Localization.FormatDrawingDoesNotExist(file),
                                   Localization.TitleAttentionPlain,
                                   System.Windows.MessageBoxButton.OK,
                                   System.Windows.MessageBoxImage.Exclamation);
            }
        }


        public static void GoProcess(Object p)
        {

            try
            {
                DrawingProcessPaths.EnsureConvertedLogDirectory();
                if (File.Exists(ApplicationRuntime.LOGarqConvertidos))
                    TryDeleteFile(ApplicationRuntime.LOGarqConvertidos);
            }
            catch (Exception)
            {


            }
            parametros = p as Param1;
            string arqtemp = DrawingProcessPaths.TempCommandFile;

            if (File.Exists(arqtemp))
                TryDeleteFile(arqtemp);

            using (StreamWriter sw = new StreamWriter(arqtemp))
            {
                sw.WriteLine(DrawingProcessPaths.GetConverterTxmlPath(parametros));
            }

            bool converted = false;
            bool deleteLogAfterClose = false;
            using (FileStream logStream = new FileStream(ApplicationRuntime.LOGarqConvertidos, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (StreamWriter swlog = new StreamWriter(logStream))
            {
                try
                {
                    IsACADOpen = false;

                    using (MessageFilter.ScopedRegistration())
                    {
                        OpenACAD();
                        myClass.LoadFile(DLLPath1);
                    }

                    int index = 1;
                    foreach (string file in parametros.desenhosName)
                    {
                        if (Processo.IsCanceled)
                            break;

                        FileOpen = Path.GetFileName(file);
                        if (index == parametros.desenhosName.Count())
                            RunCommand(file, true);
                        else
                            RunCommand(file);
                        double dProgressPercentage = ((double)index / parametros.desenhosName.Count());
                        Valor = (int)(dProgressPercentage * 100);
                        Index = index;
                        index++;
                        swlog.WriteLine(file);
                        converted = true;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    if (!converted)
                        deleteLogAfterClose = true;

                    ShowAutoCADCommandLine();
                    CloseACAD();
                    Valor = 100;

                }
            }

            if (deleteLogAfterClose)
                TryDeleteFile(ApplicationRuntime.LOGarqConvertidos);

        }

        private static void TryDeleteFile(string path)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    if (File.Exists(path))
                        File.Delete(path);

                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
                catch (UnauthorizedAccessException)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static int Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        public static int Index
        {
            get { return index; }
            set { index = value; }
        }

        public static string FileOpen
        {
            get { return fileOpen; }
            set { fileOpen = value; }
        }

        public static bool IsACADOpen
        {
            get { return isACADOpen; }
            set { isACADOpen = value; }
        }

        public static List<Block> GetListBlocksS()
        {
            List<Block> myListBlock = new List<Block>();
            try
            {

                string arq = Path.GetTempPath() + "ConversorDrawindTemp\\";
                if (!Directory.Exists(arq))
                    Directory.CreateDirectory(arq);
                arq += "TempImporBlocks.Temp";

                myClass.SendCommand("CDwi_GetBlocks\n");

                if (File.Exists(arq))
                {
                    StreamReader sr = new StreamReader(arq);
                    string temp = "---";
                    if (!sr.EndOfStream)
                        temp = sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        Block blockClass = new Block();
                        if (temp.Substring(0, 3) == "***")
                        {
                            if (temp.Length > 3)
                                blockClass.blockName = temp.Substring(3);
                        }
                        temp = sr.ReadLine();

                        while (temp.Substring(0, 3) != "***")
                        {
                            TagBlock tagTemp = new TagBlock();
                            string[] stringtemp = temp.Substring(3).Split(';');
                            tagTemp.tag = stringtemp[0];
                            tagTemp.widthfactor = stringtemp[1];
                            blockClass.listTags.Add(tagTemp);
                            if (!sr.EndOfStream)
                                temp = sr.ReadLine();
                            else
                                temp = "***";
                        }
                        myListBlock.Add(blockClass);
                    }
                    {
                        Block blockClass = new Block();
                        if (temp.Substring(0, 3) == "***")
                        {
                            if (temp.Length > 3)
                                blockClass.blockName = temp.Substring(3);
                            myListBlock.Add(blockClass);
                        }
                    }
                    sr.Close();
                    File.Delete(arq);
                }


            }
            catch (Exception)
            {

            }
            return myListBlock;

        }

        private static string GetPasteClipInsertionPoint()
        {
            double x;
            double y;
            double z;

            if (TryGetPtMin(out x, out y, out z))
                return FormatPoint(x, y, z);

            if (TryGetMinimumPointFromLayer(parametros.configuration.Layers.BlockAttributeLayer, out x, out y, out z))
                return FormatPoint(x, y, z);

            return "0,0,0";
        }

        private static bool TryGetPtMin(out double x, out double y, out double z)
        {
            x = 0;
            y = 0;
            z = 0;

            if (myClass.acadDocument == null)
                return false;

            try
            {
                object ptMin = ComRetry.Invoke(() => myClass.acadDocument.GetVariable("EXTMIN"));
                return TryGetPointCoordinates(ptMin, out x, out y, out z);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryGetMinimumPointFromLayer(string layerName, out double minX, out double minY, out double minZ)
        {
            minX = double.MaxValue;
            minY = double.MaxValue;
            minZ = double.MaxValue;

            if (myClass.acadDocument == null || string.IsNullOrWhiteSpace(layerName))
                return false;

            bool found = false;

            try
            {
                foreach (object item in myClass.acadDocument.ModelSpace)
                {
                    dynamic entity = item;
                    string entityLayer;

                    try
                    {
                        entityLayer = Convert.ToString(entity.Layer);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (!string.Equals(entityLayer, layerName, StringComparison.OrdinalIgnoreCase))
                        continue;

                    object minPoint;
                    object maxPoint;


                    try
                    {
                        entity.GetBoundingBox(out minPoint, out maxPoint);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    double x;
                    double y;
                    double z;

                    if (!TryGetPointCoordinates(minPoint, out x, out y, out z))
                        continue;

                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    minZ = Math.Min(minZ, z);
                    found = true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return found;
        }

        private static bool TryGetPointCoordinates(object point, out double x, out double y, out double z)
        {
            x = 0;
            y = 0;
            z = 0;

            Array coordinates = point as Array;
            if (coordinates == null || coordinates.Length < 2)
                return false;

            try
            {
                int lowerBound = coordinates.GetLowerBound(0);
                x = Convert.ToDouble(coordinates.GetValue(lowerBound), CultureInfo.InvariantCulture);
                y = Convert.ToDouble(coordinates.GetValue(lowerBound + 1), CultureInfo.InvariantCulture);

                if (coordinates.Length > 2)
                    z = Convert.ToDouble(coordinates.GetValue(lowerBound + 2), CultureInfo.InvariantCulture);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string FormatPoint(double x, double y, double z)
        {
            return FormatCoordinate(x) + "," + FormatCoordinate(y) + "," + FormatCoordinate(z);
        }

        private static string FormatCoordinate(double value)
        {
            return value.ToString("0.###############", CultureInfo.InvariantCulture);
        }
    }
}







