using ConversorDrawind;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ACAD = Autodesk.AutoCAD.Interop;
using ACCOMMON = Autodesk.AutoCAD.Interop.Common;

namespace ConversorDrawind
{
    public class GetInfo
    {
        private static ACAD.AcadApplication acadApplication = null;
        private static ACAD.AcadDocument acadDocument = null;
        private string status = "ERROR";

        public void UpdateStatus()
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    string name = acadDocument.Name;
                }
            }
            catch (Exception)
            {
                status = "ERROR";
            }
        }

        public GetInfo(string fileArq)
        {
            if (!string.IsNullOrWhiteSpace(fileArq))
            {
                try
                {
                    using (MessageFilter.ScopedRegistration())
                    {
                        EnsureAcadApplication();
                        acadDocument = ComRetry.Invoke(() => acadApplication.Documents.Open(fileArq, false), 120, 100);
                    }

                    LoadFiles.LoadFile(DrawingProcess.DLLPath1, acadDocument);
                    status = "OK";
                }
                catch (Exception)
                {
                    System.Windows.MessageBox.Show(
                        Localization.MessageCouldNotOpenSelectedDrawing,
                        Localization.TitleWarningNoExclamation,
                        System.Windows.MessageBoxButton.OK,
                        System.Windows.MessageBoxImage.Warning);
                    status = "ERROR";
                }
            }
        }

        private static void EnsureAcadApplication()
        {
            if (acadApplication != null)
            {
                try
                {
                    ComRetry.Invoke(() => acadApplication.Documents.Count, 120, 100);
                    return;
                }
                catch (Exception)
                {
                    acadApplication = null;
                }
            }

            acadApplication = ComRetry.Invoke(() => new ACAD.AcadApplication(), 120, 100);
            ComRetry.Invoke(() => acadApplication.WindowState = ACCOMMON.AcWindowState.acMax, 120, 100);
        }

        public string Status()
        {
            return status;
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private IntPtr _hWnd;

        public void Get2Point(ref PointEspecial p1, ref PointEspecial p2)
        {
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    _hWnd = (IntPtr)acadApplication.HWND;
                    SetForegroundWindow(_hWnd);
                    acadApplication.WindowState = ACCOMMON.AcWindowState.acMax;

                    LoadFiles.SendCommand("DRAWINDCAD_Get2Point\n", acadDocument);

                    string arq = Path.GetTempPath();
                    if (!Directory.Exists(arq))
                        Directory.CreateDirectory(arq);
                    arq = Path.Combine(arq, "ConvertTo.Point2Info");

                    if (File.Exists(arq))
                    {
                        StreamReader sr = new StreamReader(arq);
                        string[] myPointS = sr.ReadLine().Split(';');
                        p1 = new PointEspecial(Math.Round(Convert.ToDouble(myPointS[0].Replace('.', ',')), 2),
                                            Math.Round(Convert.ToDouble(myPointS[1].Replace('.', ',')), 2),
                                            Math.Round(Convert.ToDouble(myPointS[2].Replace('.', ',')), 2));
                        string[] myPoint2S = sr.ReadLine().Split(';');
                        p2 = new PointEspecial(Math.Round(Convert.ToDouble(myPoint2S[0].Replace('.', ',')), 2),
                                            Math.Round(Convert.ToDouble(myPoint2S[1].Replace('.', ',')), 2),
                                            Math.Round(Convert.ToDouble(myPoint2S[2].Replace('.', ',')), 2));

                        sr.Close();
                        File.Delete(arq);
                    }
                }
            }
            catch (Exception)
            {
                status = "ERROR";
            }
        }

        public List<Block> GetListBlocks()
        {
            List<Block> myListBlock = new List<Block>();
            try
            {
                using (MessageFilter.ScopedRegistration())
                {
                    LoadFiles.SendCommand("DRAWINDCAD_GetBlocks\n", acadDocument);

                    if (!Directory.Exists(Path.GetTempPath() + "ConversorDrawindTemp"))
                        Directory.CreateDirectory(Path.GetTempPath() + "ConversorDrawindTemp");
                    string arq = Path.GetTempPath() + "ConversorDrawindTemp\\TempImporBlocks.Temp";
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
                                    break;
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
            }
            catch (Exception)
            {
                status = "ERROR";
            }
            return myListBlock;
        }

        public void CloseDrawing()
        {
            try
            {
                acadDocument = acadApplication.Documents.Item(1);
                acadDocument.Close(false);
            }
            catch (Exception)
            {
            }
        }

        public void Dispose()
        {
            try
            {
                acadDocument = acadApplication.Documents.Item(1);
                acadDocument.Close(false);
                acadDocument = acadApplication.Documents.Item(0);
                acadDocument.Close(false);
                acadApplication.Quit();
            }
            catch (Exception)
            {
            }
        }
    }
}
