using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using ACAD = Autodesk.AutoCAD.Interop;
using ACCOMMON = Autodesk.AutoCAD.Interop.Common;

namespace ConversorDrawind
{
    partial class DrawingProcess
    {
        public static readonly string DLLPath1 = DrawingProcessPaths.DllPath;
        private static int valor;
        private static int index;
        private static string fileOpen;
        private static bool isACADOpen = false;
        private static Param1 parametros;
        private static bool _RunCommand = false;
        private const int CommandTimeoutMs = 900000;
        private const int CommandPollMs = 50;

        private static readonly AutoCadSession session = new AutoCadSession();

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
                    session.Reset();
                    session.Application = ComRetry.Invoke(() => CreateAutoCADApplication(), 180, 250);

                    AutoCadWindowActivator.CancelPendingInput(
                        ComRetry.Invoke(() => (int)session.Application.HWND));

                    session.Application.BeginCommand += AcadApplication_BeginCommand;
                    session.Application.EndCommand += AcadApplication_EndCommand;

                    IsACADOpen = true;
                    ComRetry.Invoke(() => session.Application.WindowState = ACCOMMON.AcWindowState.acMax);

                    session.OpenedDocuments.Add(ComRetry.Invoke(() => session.Application.Documents.Add("Novo Desenho 1"), 120, 100));

                    session.CurrentDocument = session.OpenedDocuments.First();
                    ComRetry.Invoke(() => session.CurrentDocument.WindowState = ACCOMMON.AcWindowState.acMax);

                    if (parametros.configuration.General.ExchangeFormat)
                    {
                        try
                        {
                            string formatoPath = DrawingProcessPaths.GetExchangeFormatPath(parametros.configuration);

                            session.CurrentDocument = ComRetry.Invoke(() => session.Application.Documents.Open(formatoPath, false), 120, 100);
                            ComRetry.Invoke(() => session.CurrentDocument.Application.ZoomExtents());
                            session.AttributeDocument = session.CurrentDocument;
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
                if (session.CurrentDocument == null)
                    return 0;

                object cmdActive = ComRetry.Invoke(() => session.CurrentDocument.GetVariable("CMDACTIVE"));
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

        private static void LoadFile(string file)
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    ComRetry.Invoke(() => session.CurrentDocument.SetVariable("FILEDIA", 0));
                    try
                    {
                        if (!String.IsNullOrEmpty(file))
                        {
                            SendCommand(DrawingCommandBuilder.BuildLoadFileCommand(file));
                        }
                    }
                    finally
                    {
                        ComRetry.Invoke(() => session.CurrentDocument.SetVariable("FILEDIA", 1));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void SendCommand(string commandName)
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    _RunCommand = true;

                    ComRetry.Invoke(() => session.Application.ActiveDocument = session.CurrentDocument);
                    ComRetry.Invoke(() => session.CurrentDocument.SendCommand(commandName));
                    WaitCommandFinished(commandName);

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
                if (session.Application == null || session.CurrentDocument == null)
                    return;

                SendCommand(DrawingCommandBuilder.BuildCommandLineCommand());
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
                    if (session.Application == null)
                        return;

                    if (parametros.configuration.General.ExchangeFormat && session.AttributeDocument != null)
                    {
                        session.CurrentDocument = session.AttributeDocument;
                        ComRetry.Invoke(() => session.CurrentDocument.Close(false));
                    }

                    if (session.OpenedDocuments.Count > 0)
                    {
                        session.CurrentDocument = session.OpenedDocuments.First();
                        ComRetry.Invoke(() => session.Application.ActiveDocument = session.CurrentDocument);
                        ComRetry.Invoke(() => session.CurrentDocument.Close(false));
                        session.OpenedDocuments.RemoveAt(0);
                    }

                    if (!parametros.closedesenhos)
                    {
                        try
                        {
                            if (ComRetry.Invoke(() => session.Application.Documents.Count) == 0)
                                ComRetry.Invoke(() => session.Application.Quit(), 120, 100);
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
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                ReleaseAutoCadReferences();
            }
        }

        private static void ReleaseAutoCadReferences()
        {
            List<object> releasedObjects = new List<object>();

            try
            {
                if (session.Application != null)
                {
                    session.Application.BeginCommand -= AcadApplication_BeginCommand;
                    session.Application.EndCommand -= AcadApplication_EndCommand;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                ReleaseComObjectOnce(releasedObjects, session.CurrentDocument);
                ReleaseComObjectOnce(releasedObjects, session.AttributeDocument);

                foreach (ACAD.AcadDocument document in session.OpenedDocuments)
                    ReleaseComObjectOnce(releasedObjects, document);

                ReleaseComObjectOnce(releasedObjects, session.Application);

                session.Reset();
                IsACADOpen = false;
                _RunCommand = false;
            }
        }

        private static void ReleaseComObjectOnce(List<object> releasedObjects, object comObject)
        {
            if (comObject == null || releasedObjects.Any(item => ReferenceEquals(item, comObject)))
                return;

            releasedObjects.Add(comObject);

            try
            {
                if (Marshal.IsComObject(comObject))
                    Marshal.FinalReleaseComObject(comObject);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private static bool RunCommand(string file, bool last = false)
        {
            if (File.Exists(file))
            {
                ACAD.AcadDocument drawingDocument = null;

                try
                {
                    File.Copy(file, DrawingProcessPaths.GetBackupPath(file), true);
                }
                catch (Exception)
                {

                }

                try
                {
                    drawingDocument = ComRetry.Invoke(() => session.Application.Documents.Open(file, false), 120, 100);
                    session.CurrentDocument = drawingDocument;
                    session.OpenedDocuments.Add(drawingDocument);
                    ComRetry.Invoke(() => session.Application.ActiveDocument = drawingDocument);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    CloseFailedDrawing(drawingDocument);
                    return false;
                }

                try
                {
                    SendCommand("ZOOM E\n");
                    SendCommand("CDwi_Convert\n");
                    if (parametros.configuration.General.ExchangeFormat)
                    {
                        SendCommand("CDwi_GetAttributeText\n");

                        session.CurrentDocument = session.AttributeDocument;
                        ComRetry.Invoke(() => session.Application.ActiveDocument = session.CurrentDocument);

                        SendCommand("COPYBASE 0,0,0 all \n");

                        session.CurrentDocument = session.OpenedDocuments.Last();
                        ComRetry.Invoke(() => session.Application.ActiveDocument = session.CurrentDocument);

                        SendCommand("ZOOM E\n");
                        SendCommand("REGEN\n");
                        string pasteClipPoint = GetPasteClipInsertionPoint();

                        if (parametros.configuration.General.SourceMode == 1)
                        {

                            List<Block> listAnterior = GetListBlocksS();

                            SendCommand("PASTECLIP " + pasteClipPoint + "\n");

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
                                SendCommand("CDwi_ScaleBlock\n" + comando + "\n");
                            }
                        }

                        else
                        {
                            SendCommand("PASTECLIP " + pasteClipPoint + "\n");

                        }

                        SendCommand("CDwi_AttributeBlock\n");

                        if (parametros.configuration.General.SourceMode == 1)
                        {
                            SendCommand("CDwi_DeleteBlocks\n");
                        }
                    }

                    if (parametros.configuration.General.ConvertLayers)
                    {
                        SendCommand("CDwi_DeleteLayers\n");
                    }

                    if (parametros.configuration.General.ApplyDrawingScale)
                    {
                        ApplicationRuntime.ControladorT2 = false;
                        try
                        {
                            SendCommand("CDwi_Scale\n");
                        }
                        finally
                        {
                            ApplicationRuntime.ControladorT2 = true;
                        }
                    }


                    if (parametros.configuration.General.ExecuteLisp)
                    {
                        IReadOnlyList<LispCommandDefinition> lispCommands =
                            LispCommandDefinition.ParseAll(parametros.configuration.Commands.LispCommands);

                        ComRetry.Invoke(() => session.CurrentDocument.SetVariable("FILEDIA", 0));
                        try
                        {
                            foreach (LispCommandDefinition command in lispCommands.Where(item => !item.ExecuteAfterConversion))
                            {
                                LoadFile(command.SourceFile);
                                SendCommand(command.Command + "\n");
                            }

                            if (last)
                                foreach (LispCommandDefinition command in lispCommands.Where(item => item.ExecuteAfterConversion))
                                {
                                    LoadFile(command.SourceFile);
                                    SendCommand(command.Command + "\n");
                                }
                        }
                        finally
                        {
                            ComRetry.Invoke(() => session.CurrentDocument.SetVariable("FILEDIA", 1));
                        }
                    }

                    SendCommand("CDwi_Finalize\n");
                    ComRetry.Invoke(() => session.CurrentDocument.Application.ZoomExtents());

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    CloseFailedDrawing(drawingDocument);
                    return false;
                }



                try
                {
                    try
                    {
                        if (ApplicationRuntime.ExtensaoGeral == "DWG")
                            ComRetry.Invoke(() => session.CurrentDocument.Save());

                        else
                        {
                            SendCommand("SaveDXF\n");
                        }

                    }
                    catch (Exception)
                    {
                        ApplicationRuntime.ControladorT2 = false;
                        try
                        {
                            System.Windows.MessageBox.Show(
                                        Localization.MessageCouldNotSaveFile,
                                        Localization.TitleWarningNoExclamation,
                                        System.Windows.MessageBoxButton.OK,
                                        System.Windows.MessageBoxImage.Warning);
                        }
                        finally
                        {
                            ApplicationRuntime.ControladorT2 = true;
                        }

                        CloseFailedDrawing(drawingDocument);
                        return false;
                    }

                    if (!parametros.closedesenhos)
                    {
                        int cont = ComRetry.Invoke(() => session.Application.Documents.Count);

                        ComRetry.Invoke(() => session.CurrentDocument.Close());

                        Stopwatch waitClose = Stopwatch.StartNew();
                        while (waitClose.ElapsedMilliseconds < 30000)
                        {
                            if (ComRetry.Invoke(() => session.Application.Documents.Count) < cont)
                                break;

                            Thread.Sleep(50);
                        }

                        session.OpenedDocuments.RemoveAt(session.OpenedDocuments.Count - 1);
                    }
                }
                catch (Exception e)
                {
                    System.Windows.MessageBox.Show(
                                     e.Message,
                                     Localization.TitleWarningNoExclamation,
                                     System.Windows.MessageBoxButton.OK,
                                     System.Windows.MessageBoxImage.Warning);
                    CloseFailedDrawing(drawingDocument);
                    return false;
                }

                return true;
            }
            else
            {
                System.Windows.MessageBox.Show(Localization.FormatDrawingDoesNotExist(file),
                                   Localization.TitleAttentionPlain,
                                   System.Windows.MessageBoxButton.OK,
                                   System.Windows.MessageBoxImage.Exclamation);
                return false;
            }
        }

        private static void CloseFailedDrawing(ACAD.AcadDocument drawingDocument)
        {
            if (parametros == null || parametros.closedesenhos)
                return;

            ACAD.AcadDocument documentToClose = drawingDocument ?? session.CurrentDocument;
            if (documentToClose == null)
                return;

            try
            {
                ComRetry.Invoke(() => documentToClose.Close(false));
                session.OpenedDocuments.Remove(documentToClose);

                if (ReferenceEquals(session.CurrentDocument, documentToClose))
                    session.CurrentDocument = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
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

                SendCommand("CDwi_GetBlocks\n");

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

            if (session.CurrentDocument == null)
                return false;

            try
            {
                object ptMin = ComRetry.Invoke(() => session.CurrentDocument.GetVariable("EXTMIN"));
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

            if (session.CurrentDocument == null || string.IsNullOrWhiteSpace(layerName))
                return false;

            bool found = false;

            try
            {
                foreach (object item in session.CurrentDocument.ModelSpace)
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







