using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FORMS = System.Windows.Forms;
using ACAD = Autodesk.AutoCAD.Interop;
using ACCOMMON = Autodesk.AutoCAD.Interop.Common;
using System.Threading;
using System.Windows.Forms;

namespace ConversorDrawind
{
    public class Class_GetInfo
    {
        private static ACAD.AcadApplication acadApplication = null;
        private static ACAD.AcadDocument acadDocument = null;
        private string status = "ERROR";



        public void UpdateStatus()
        {
            try
            {
                using (Class_MessageFilter.ScopedRegistration())
                {
                    string name = acadDocument.Name;
                }

            }
            catch (Exception)
            {
                status = "ERROR";
            }
        }
       


        public Class_GetInfo(string fileArq)
        {
            if (fileArq != "")
            {
                try 
                {
                    using (Class_MessageFilter.ScopedRegistration())
                    {

                        try
                        {
                            acadDocument = acadApplication.Documents.Item(0);
                         
                        }
                        catch (Exception)
                        {
                            acadApplication = new ACAD.AcadApplication();

                            acadApplication.WindowState = ACCOMMON.AcWindowState.acMax;



                        }

                        acadDocument = acadApplication.Documents.Open(fileArq, false);


                    }

                    LoadFiles.LoadFile(Class_DrawingProcess.DLLPath1, acadDocument);
                    status = "OK";
                }
                catch (Exception)
                {
                    FORMS.MessageBox.Show(new FORMS.Form() { TopMost = true },
                                     "Não fopossível abrir o desenho selecionado!",
                                     "Error",
                                     FORMS.MessageBoxButtons.OK,
                                     FORMS.MessageBoxIcon.Warning,
                                     FORMS.MessageBoxDefaultButton.Button1);
                    status = "ERROR";
                }
            }
        }



        public string Status()
        {
            return status;
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private IntPtr _hWnd;

        public void Get2Point(ref Class_PointEspecial p1, ref Class_PointEspecial p2)
        {
            try
            {
                using (Class_MessageFilter.ScopedRegistration())
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
                        p1 = new Class_PointEspecial(Math.Round(Convert.ToDouble(myPointS[0].Replace('.', ',')), 2),
                                            Math.Round(Convert.ToDouble(myPointS[1].Replace('.', ',')), 2),
                                            Math.Round(Convert.ToDouble(myPointS[2].Replace('.', ',')), 2));
                        string[] myPoint2S = sr.ReadLine().Split(';');
                        p2 = new Class_PointEspecial(Math.Round(Convert.ToDouble(myPoint2S[0].Replace('.', ',')), 2),
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




        public List<Class_BlockClass> GetListBlocks()
        {
            List<Class_BlockClass> myListBlock = new List<Class_BlockClass>();
            try
            {
                using (Class_MessageFilter.ScopedRegistration())
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
                            Class_BlockClass blockClass = new Class_BlockClass();
                            if (temp.Substring(0, 3) == "***")
                            {
                                if (temp.Length > 3)
                                    blockClass.blockName = temp.Substring(3);
                            }
                            temp = sr.ReadLine();

                            while (temp.Substring(0, 3) != "***")
                            {
                                Class_TagBlockClass tagTemp = new Class_TagBlockClass();
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
                            Class_BlockClass blockClass = new Class_BlockClass();
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
