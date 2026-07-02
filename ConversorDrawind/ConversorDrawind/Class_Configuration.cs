using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;

namespace ConversorDrawind
{
    public class Class_Configuration
    {
        /// <summary>
        /// INT CONVERT DIMENSION
        /// </summary>
       // public string INTFILTERCOTALayerName = "Dimension";
        public double INTFactorLengthChar = 1.5;
        public double INTDIMTextOffset = 0.6100;
        public bool DMBlock = true;
        /// <summary>
        /// EXT CONVERT DIMENSION
        /// </summary>
        public string EXTDIMStyleName = "COTAS";
        public double EXTDIMScale = 1; //Escala da dimensão
        public int EXTDIMPrecision = 0; //Precisão da dimensão 
        public int EXTDIMAngularPrecision = 1; //Precisão da dimensão algular
        public int EXTDIMUnit = 2;//Unidade da dimensão 
        public int EXTDIMAngularUnit = 2;//Unidade da dimensão algular
        public string EXTDIMSeta = "Oblique";
        public string EXTDIMSeta1 = "Oblique";
        public string EXTDIMSeta2 = "Oblique";
        public double EXTDIMSizeSeta = 1.25;
        public int EXTDIMTad = 1; //Posição do texto na dimensão verticalmente
        public bool EXTDIMDimensionPosition = false; //Posição do texto em relação a dimensão
        public bool EXTDIMTextForced = true; //Forçar o texto a permanecer alinhado com a dimensão, caso o espaço seja curto.
        public bool EXTDIMLineForced = true;
        public string EXTDIMColorText = "GREEN";
        public string EXTDIMColorLine = "RED";
        public double EXTDIMOffsetLineFromRefPoint = 0;
        public int EXTDIMTextMove = 2;
        public bool EXTDIMOutsideAlign = false;
        public string EXTDIMlayer = "0";
        public string EXTTEXTStyleName = "TEXTO";
        
        public bool EXTDIMGERALHabilit = true;
        public double EXTDIMDIMEX = 1.25;
        public string EXTDIMBaseLayer = "DIMENSION";
        public string LayerTeklaString = "DRAWING SHEET";
        public string LayerBlockAttribute = "OTHER OBJECT TYPE";
       // public double EXTTEXTObliqueAngle = 0;
        /// <summary>
        /// EXT SCALE
        /// </summary>
        public bool EXTSCALEManual = true;
        public Class_PointEspecial EXTSCALEMp1 = new Class_PointEspecial(0, 0, 0);
        public Class_PointEspecial EXTSCALEMp2 = new Class_PointEspecial(0, 0, 0);
        public Class_PointEspecial EXTSCALEAp1 = new Class_PointEspecial(0, 0, 0);
        public Class_PointEspecial EXTSCALEAp2 = new Class_PointEspecial(0, 0, 0);
        public string EXTSCALELayer = "0";
        public double EXTSCALETextSize = 2.5;

        /// <summary>
        /// EXT CONFIGURATION
        /// </summary> 
        public string EXTCONFComments = "";
        public bool EXTCONFIsConvertDimension = true;
        public bool EXTCONFIsConvertLayer = true;
        public bool EXTCONFIsExchangeFormat = false;
        public bool EXTCONFIsExchangeLM = false;
        public bool EXTCONFIsPutOnTheScaleDrawing = false;
        public bool EXTCONFIsExecuteLISP = false;
        public bool EXTCONFIsExecuteDLL = false;
        public bool EXTCONFIsDeleteTeklaStructures = true;
        public string EXTCONFIsFirstRum = "LISP";
        public bool EXTCONFIsPurge = true;
        /// <summary>
        /// LINE CONFIGURATION
        /// </summary> 
        public double EXTLINELtscale = 10;

        /// <summary>
        /// ROGRAM CONFIGURATION
        /// </summary> 
        static private string PROGRAMDirectoryTemp = Path.GetTempPath() + "ConversorDrawindTemp\\";



        public string PROGRAMDbLin;
        public bool PROGRAMMessage = true;

        public bool EXTDIMCorrigeSeta = false;
        public string EXTDIMCorrigeSetaTipoSeta = "Oblique";
        public double EXTDIMCorrigeSetaFactor = 7.23;

        public string PROGRAMblockFormatoCaminho = " ";

        public int EXTCONFOrigem = 0;
        public bool EXTCONFInventorExplode = false;
        public string EXTCONFCaminhoBlocoInv = "";


        public bool ExplodeBlocks = false;

        public Class_Configuration()
        { 
            var t = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            PROGRAMDbLin = Path.Combine(t, @"Autodesk\AutoCAD 2026\R25.1\enu\support\") + @"acad.lin";
        }

        public string GetPROGRAMDirectoryTemp()
        {
            return PROGRAMDirectoryTemp;
        }

        public bool CheckFileTemplateExist(string file, StatusConversorItem statusConversorItem)
        {
                 string arquivo = AppDomain.CurrentDomain.BaseDirectory +
                                        statusConversorItem.Pasta + "\\";

            if (!Directory.Exists(arquivo))
                Directory.CreateDirectory(arquivo);
            arquivo += file + ".Templates";

            return File.Exists(arquivo);
        }

        public bool CheckFileTxmlExist(string file, StatusConversorItem statusConversorItem)
        {
            string arquivo = AppDomain.CurrentDomain.BaseDirectory +
                                        statusConversorItem.Pasta + "\\";

            if (!Directory.Exists(arquivo))
                Directory.CreateDirectory(arquivo);
            arquivo += file + ".txml";

            return File.Exists(arquivo);
        }


        public void Load(string file, Class_Arranjos arranjos, List<Class_BlockClass> blocks, List<Class_BlockClass> blocosi, List<Class_BlockClass> blocoso, StatusConversorItem statusConversorItem)
        {
            List<string> listLineBase = new List<string>();
            string type = "Coments:";
            string line = string.Empty;
            string location = AppDomain.CurrentDomain.BaseDirectory +
                                                        statusConversorItem.Pasta + "\\" +
                                                         file +
                                                        ".Template";
            StreamReader streamReader = new StreamReader(location, Encoding.UTF8, true);

            EXTCONFComments = string.Empty;
            while (!streamReader.EndOfStream && type == "Coments:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "Coments:")
                    EXTCONFComments += treatment.Last() + "\n";
            }

            EXTCONFComments = EXTCONFComments.Remove(EXTCONFComments.Length - 1, 1);
            EXTCONFIsConvertDimension = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            EXTCONFIsConvertLayer = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            EXTCONFIsExchangeFormat = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            EXTCONFIsExchangeLM = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            EXTCONFIsPutOnTheScaleDrawing = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            EXTCONFIsExecuteLISP = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            EXTCONFIsExecuteDLL = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            EXTCONFIsFirstRum = streamReader.ReadLine().Remove(0, 12);
            EXTCONFIsPurge = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 12));
            streamReader.ReadLine();

            arranjos.allBaseLayer.Clear();
            type = "BaseLayer:";
            while (!streamReader.EndOfStream && type == "BaseLayer:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "BaseLayer:")
                    arranjos.allBaseLayer.Add(treatment.Last());
            }

            arranjos.allLineType1.Clear();
            type = "BaseLineType:";
            while (!streamReader.EndOfStream && type == "BaseLineType:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "BaseLineType:")
                    arranjos.allLineType1.Add(treatment.Last());
            }

            if (arranjos.allLineType1.Count == 0)
            {
                arranjos.allLineType1.AddRange(arranjos.lineType1);
            }

            arranjos.allNewLayerComposition.Clear();
            type = "NewLayer:";
            while (!streamReader.EndOfStream && type == "NewLayer:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "NewLayer:")
                    arranjos.allNewLayerComposition.Add(treatment.Last());
            }

            arranjos.allNewLayer.Clear();
            for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
            {
                arranjos.allNewLayer.Add(arranjos.allNewLayerComposition[i].Split(':').First());
            }

            arranjos.conversor.Clear();
            type = "Converter:";
            while (!streamReader.EndOfStream && type == "Converter:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "Converter:")
                    arranjos.conversor.Add(treatment.Last());
            }


            EXTDIMGERALHabilit = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 15));
            EXTDIMlayer = streamReader.ReadLine().Remove(0, 15);
            EXTDIMColorLine = streamReader.ReadLine().Remove(0, 15);
            EXTDIMColorText = streamReader.ReadLine().Remove(0, 15);
            EXTDIMStyleName = streamReader.ReadLine().Remove(0, 15);
            EXTTEXTStyleName = streamReader.ReadLine().Remove(0, 15);
            EXTDIMSeta = streamReader.ReadLine().Remove(0, 15);
            
            EXTDIMScale = Convert.ToDouble(streamReader.ReadLine().Remove(0, 15).Replace('.', ','));
            EXTDIMPrecision = Convert.ToInt32(streamReader.ReadLine().Remove(0, 15));
            EXTDIMAngularPrecision = Convert.ToInt32(streamReader.ReadLine().Remove(0, 15));
            EXTDIMUnit = Convert.ToInt32(streamReader.ReadLine().Remove(0, 15));
            EXTDIMAngularUnit = Convert.ToInt32(streamReader.ReadLine().Remove(0, 15));
            EXTDIMSizeSeta = Convert.ToDouble(streamReader.ReadLine().Remove(0, 15).Replace('.', ','));
            EXTDIMOffsetLineFromRefPoint = Convert.ToDouble(streamReader.ReadLine().Remove(0, 15).Replace('.', ','));
            EXTDIMOutsideAlign = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 15));
            EXTDIMTad = Convert.ToInt32(streamReader.ReadLine().Remove(0, 15));
            EXTDIMDimensionPosition = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 15));
            EXTDIMTextForced = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 15));
            EXTDIMLineForced = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 15));
            EXTDIMDIMEX = Convert.ToDouble(streamReader.ReadLine().Remove(0, 15).Replace('.', ','));
            EXTDIMBaseLayer = streamReader.ReadLine().Remove(0, 15);

            streamReader.ReadLine();
            EXTLINELtscale = Convert.ToDouble(streamReader.ReadLine().Remove(0, 10));

            streamReader.ReadLine();
            PROGRAMMessage = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 13));


            streamReader.ReadLine();
            EXTSCALEManual = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 11));
            string[] mp1 = streamReader.ReadLine().Remove(0, 11).Split(';');
            EXTSCALEMp1 = new Class_PointEspecial(Convert.ToDouble(mp1[0].Replace('.', ',')),
                                     Convert.ToDouble(mp1[1].Replace('.', ',')),
                                     Convert.ToDouble(mp1[2].Replace('.', ',')));
            string[] mp2 = streamReader.ReadLine().Remove(0, 11).Split(';');
            EXTSCALEMp2 = new Class_PointEspecial(Convert.ToDouble(mp2[0].Replace('.', ',')),
                                     Convert.ToDouble(mp2[1].Replace('.', ',')),
                                     Convert.ToDouble(mp2[2].Replace('.', ',')));
            string[] mp3 = streamReader.ReadLine().Remove(0, 11).Split(';');
            EXTSCALEAp1 = new Class_PointEspecial(Convert.ToDouble(mp3[0].Replace('.', ',')),
                                     Convert.ToDouble(mp3[1].Replace('.', ',')),
                                     Convert.ToDouble(mp3[2].Replace('.', ',')));
            string[] mp4 = streamReader.ReadLine().Remove(0, 11).Split(';');
            EXTSCALEAp2 = new Class_PointEspecial(Convert.ToDouble(mp4[0].Replace('.', ',')),
                                     Convert.ToDouble(mp4[1].Replace('.', ',')),
                                     Convert.ToDouble(mp4[2].Replace('.', ',')));
            EXTSCALELayer = streamReader.ReadLine().Remove(0, 11);
            EXTSCALETextSize = Convert.ToDouble(streamReader.ReadLine().Remove(0, 11).Replace('.', ','));

            streamReader.ReadLine();
            EXTCONFIsDeleteTeklaStructures = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 23));

            streamReader.ReadLine();
            PROGRAMblockFormatoCaminho = streamReader.ReadLine().Remove(0, 11);

            streamReader.ReadLine();
            arranjos.layerRemove.Clear();
            type = "BlockLayerRemove:";
            while (!streamReader.EndOfStream && type == "BlockLayerRemove:")
            {
                Class_Filter f = new Class_Filter(arranjos);
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "BlockLayerRemove:")
                {
                    string[] st = treatment.Last().Split(';');
                    f.layerBase = st[0];
                    f.SetConjunto(st[1]);
                    arranjos.layerRemove.Add(f);
                }
            }

            arranjos.listLISPCommand.Clear();
            type = "DLLCommand:";
            while (!streamReader.EndOfStream && type == "DLLCommand:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "DLLCommand:")
                    arranjos.listLISPCommand.Add(line.Substring(12));
            }


            type = "LISPCommand:";
            while (!streamReader.EndOfStream && type == "LISPCommand:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "LISPCommand:")
                    arranjos.listLISPCommand.Add(line.Substring(13));
            }

            blocks.Clear();
            blocosi.Clear();
            blocoso.Clear();
            type = "BlockName:";
            while (!streamReader.EndOfStream && type == "BlockName:")
            {
                line = streamReader.ReadLine();
                string[] treatment = line.Split('$');
                type = treatment.First();
                if (type == "BlockName:")
                {
                    Class_BlockClass blockClass = new Class_BlockClass();
                    if (line.Length > 11)
                        blockClass.blockName = line.Substring(11);

                    type = "BlockTag:";
                    while (!streamReader.EndOfStream && type == "BlockTag:")
                    {
                        line = streamReader.ReadLine();
                        string[] treatment2 = line.Split('$');
                        type = treatment2.First();

                        if (type == "BlockTag:")
                        {
                            Class_TagBlockClass tagTemp = new Class_TagBlockClass();
                            tagTemp.SetConjunto(line.Substring(10));
                            blockClass.listTags.Add(tagTemp);
                        }
                        else
                        {
                            type = "BlockName:";
                        }

                    }
                    blocks.Add(blockClass);
                }
            }
            type = "ConfDimension:";
            string teste = streamReader.ReadLine().Remove(0, 15);
            EXTDIMCorrigeSeta = Convert.ToBoolean(teste);
            EXTDIMCorrigeSetaTipoSeta = streamReader.ReadLine().Remove(0, 15);
            EXTDIMCorrigeSetaFactor = Convert.ToDouble(streamReader.ReadLine().Remove(0, 15).Replace('.', ','));
            //EXTTEXTObliqueAngle = Convert.ToDouble(streamReader.ReadLine().Remove(0, 15).Replace('.', ','));
            try
            {
                arranjos.allExplodeLayers.Clear();
                streamReader.ReadLine();
                EXTCONFInventorExplode = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 11));
                EXTCONFOrigem = Convert.ToInt32(streamReader.ReadLine().Remove(0, 11));
           
                string[] allexplodelayers = streamReader.ReadLine().Remove(0, 11).Split(';');
                arranjos.allExplodeLayers.AddRange(allexplodelayers);
                EXTCONFCaminhoBlocoInv = streamReader.ReadLine().Remove(0, 11);



                type = "BlockName:";
                streamReader.ReadLine();
                while (!streamReader.EndOfStream && type == "BlockName:")
                {
                    line = streamReader.ReadLine();
                    string[] treatment = line.Split('$');
                    type = treatment.First();
                    if (type == "BlockName:")
                    {
                        Class_BlockClass blockClass = new Class_BlockClass();

                        if (line.Length > 11)
                        {
                            string[] linesplit = line.Substring(11).Split(';');
                            blockClass.blockName = linesplit[0];
                            blockClass.blockNameRelacao = linesplit[1];
                            blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));
                        }

                        type = "BlockTag:";
                        while (!streamReader.EndOfStream && type == "BlockTag:")
                        {
                            line = streamReader.ReadLine();
                            string[] treatment2 = line.Split('$');
                            type = treatment2.First();

                            if (type == "BlockTag:")
                            {
                                Class_TagBlockClass tagTemp = new Class_TagBlockClass();
                                tagTemp.SetConjunto(line.Substring(10));
                                string[] linetemp = line.Substring(10).Split('@');
                                tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                                tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                                blockClass.listTags.Add(tagTemp);
                            }
                            else
                            {
                                type = "BlockName:";
                            }

                        }
                        blocosi.Add(blockClass);
                    }
                }
                streamReader.ReadLine();
                type = "BlockName:";
                while (!streamReader.EndOfStream && type == "BlockName:")
                {
                    line = streamReader.ReadLine();
                    string[] treatment = line.Split('$');
                    type = treatment.First();
                    if (type == "BlockName:")
                    {
                        Class_BlockClass blockClass = new Class_BlockClass();

                        if (line.Length > 11)
                        {
                            string[] linesplit = line.Substring(11).Split(';');
                            blockClass.blockName = linesplit[0];
                            blockClass.blockNameRelacao = linesplit[1];
                            blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));
                        }

                        type = "BlockTag:";
                        while (!streamReader.EndOfStream && type == "BlockTag:")
                        {
                            line = streamReader.ReadLine();
                            string[] treatment2 = line.Split('$');
                            type = treatment2.First();

                            if (type == "BlockTag:")
                            {
                                Class_TagBlockClass tagTemp = new Class_TagBlockClass();
                                tagTemp.SetConjunto(line.Substring(10));
                                string[] linetemp = line.Substring(10).Split('@');
                                tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                                tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                                blockClass.listTags.Add(tagTemp);
                            }
                            else
                            {
                                type = "BlockName:";
                            }

                        }
                        blocoso.Add(blockClass);
                    }
                }
            }
            catch (Exception)
            {

            }
            streamReader.ReadLine();
            bool erroLoad = false;
            try
            {
                ExplodeBlocks = Convert.ToBoolean(streamReader.ReadLine().Remove(0, 11));
                LayerTeklaString = streamReader.ReadLine().Remove(0, 11);
                LayerBlockAttribute = streamReader.ReadLine().Remove(0, 11);
            }
            catch (Exception)
            {

                erroLoad = true;
              
            }
            streamReader.Close();
            if (erroLoad)
            {
                string space = "*************************************************************";
                StreamWriter sw = File.AppendText(location);
                sw.WriteLine(space);
                sw.WriteLine("ConfExtra:$" + ExplodeBlocks);
                sw.WriteLine("ConfExtra:$" + LayerTeklaString);
                sw.WriteLine("ConfExtra:$" + LayerBlockAttribute);
                sw.Close();
            }
        }

        public void LoadXML(string file, Class_Arranjos arranjos, List<Class_BlockClass> blocks, List<Class_BlockClass> blocosi, List<Class_BlockClass> blocoso , StatusConversorItem statusConversorItem)
        {
            arranjos.allBaseLayer.Clear();
            arranjos.allLineType1.Clear();
            arranjos.allNewLayerComposition.Clear();
            arranjos.allNewLayer.Clear();
            arranjos.conversor.Clear();
            arranjos.layerRemove.Clear();
            arranjos.listLISPCommand.Clear();
            blocks.Clear();
            blocosi.Clear();
            blocoso.Clear();
            arranjos.allExplodeLayers.Clear();


            string arquivo = AppDomain.CurrentDomain.BaseDirectory +
                                            statusConversorItem.Pasta +  "\\";

            if (!Directory.Exists(arquivo))
                Directory.CreateDirectory(arquivo);
            arquivo += file + ".txml";

            XElement importXML = XElement.Load(arquivo);

            EXTCONFComments = importXML.Element("COMMENTS").Attribute("TEXT").Value;


            EXTCONFOrigem = Convert.ToInt32(importXML.Element("BASIC_CONFIG").Attribute("TEKLAORCAD").Value);
            EXTCONFIsConvertDimension = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("CONVERT_DIMENSIONS").Value);
            EXTCONFIsConvertLayer =Convert.ToBoolean( importXML.Element("BASIC_CONFIG").Attribute("CONVERT_LAYERS").Value);
            EXTCONFIsExchangeFormat = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("EXCHANGE_FORMAT").Value);
            EXTCONFIsPutOnTheScaleDrawing = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("SCALE").Value);
            EXTCONFIsExecuteLISP = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("LISPORDLL").Value);
            EXTCONFIsPurge = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("PURGE").Value);
            PROGRAMMessage = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("MESSAGE").Value);
            EXTCONFIsDeleteTeklaStructures = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("DELETE_TEKLA_STRUCTURES").Value);
            ExplodeBlocks = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("EXPLOD_BLOCKS").Value);
            LayerTeklaString = importXML.Element("BASIC_CONFIG").Attribute("LAYER_TEKLA_STRING").Value;
            LayerBlockAttribute = importXML.Element("BASIC_CONFIG").Attribute("LAYER_BLOCK_ATTRIBUTE").Value;
            EXTCONFInventorExplode = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("CAD_EXPLODE").Value);


            EXTDIMGERALHabilit = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_GERAL_HABILIT").Value);
            EXTDIMlayer = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_LAYER").Value;
            EXTDIMColorLine = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_LINE_COLOR").Value;
            EXTDIMColorText = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_TEXT_COLOR").Value;
            EXTDIMStyleName = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_STYLE").Value;
            EXTDIMSeta = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_TYPE").Value;
            EXTDIMScale = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_SCALE").Value.Replace('.', ','));
            EXTDIMPrecision = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_PRECISION").Value);
            EXTDIMAngularPrecision = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ANGULAR_PRECISION").Value);
            EXTDIMUnit = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_UNIT").Value);
            EXTDIMAngularUnit = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ANGULAR_UNIT").Value);
            EXTDIMSizeSeta = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_SIZE").Value.Replace('.', ','));
            EXTDIMOffsetLineFromRefPoint = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_OFFSET").Value.Replace('.', ','));
            EXTDIMOutsideAlign = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_OUTSIDE_ALING").Value);
            EXTDIMTad = Convert.ToInt32(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_TAD").Value);
            EXTDIMDimensionPosition = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_POSITION").Value);
            EXTDIMTextForced = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_TEXT_FORCED").Value);
            EXTDIMLineForced = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_LINE_FORCED").Value);
            EXTDIMDIMEX = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_DIMEX").Value.Replace('.', ','));
            EXTDIMBaseLayer = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_BASE_LAYER").Value;
            EXTDIMCorrigeSeta = Convert.ToBoolean(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_FIX").Value);
            EXTDIMCorrigeSetaTipoSeta = importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_FIX_TYPE").Value;
            EXTDIMCorrigeSetaFactor = Convert.ToDouble(importXML.Element("DIMENSION_CONFIG").Attribute("DIM_ARROW_FACTOR").Value.Replace('.', ','));
            EXTTEXTStyleName = importXML.Element("TEXT_CONFIG").Attribute("TEXT_STYPE").Value;
           
            //EXTTEXTObliqueAngle = Convert.ToDouble(importXML.Element("TEXT_CONFIG").Attribute("TEXT_OBLIQUE_ANGLE").Value.Replace('.', ','));


            EXTSCALEManual = Convert.ToBoolean(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MODE").Value);
            EXTSCALEMp1.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P1_X").Value.Replace('.', ','));
            EXTSCALEMp1.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P1_Y").Value.Replace('.', ','));
            EXTSCALEMp1.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P1_Z").Value.Replace('.', ','));
            EXTSCALEMp2.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P2_X").Value.Replace('.', ','));
            EXTSCALEMp2.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P2_Y").Value.Replace('.', ','));
            EXTSCALEMp2.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_MANUAL_P2_Z").Value.Replace('.', ','));
            EXTSCALEAp1.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P1_X").Value.Replace('.', ','));
            EXTSCALEAp1.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P1_Y").Value.Replace('.', ','));
            EXTSCALEAp1.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P1_Z").Value.Replace('.', ','));
            EXTSCALEAp2.X = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P2_X").Value.Replace('.', ','));
            EXTSCALEAp2.Y = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P2_Y").Value.Replace('.', ','));
            EXTSCALEAp2.Z = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_AUTO_P2_Z").Value.Replace('.', ','));
            EXTSCALELayer = importXML.Element("SCALE_CONFIG").Attribute("SCALE_LAYER").Value;
            EXTSCALETextSize = Convert.ToDouble(importXML.Element("SCALE_CONFIG").Attribute("SCALE_TEXT_SIZE").Value.Replace('.', ','));



            EXTLINELtscale = Convert.ToDouble(importXML.Element("BASIC_LAYERS").Attribute("LTSCALE").Value.Replace('.', ','));
            foreach (var item in importXML.Element("BASIC_LAYERS").Elements("BASE_LAYER"))
            {
                arranjos.allBaseLayer.Add(item.Value);
            }
            foreach (var item in importXML.Element("BASIC_LINES").Elements("BASE_LINE"))
            {
                arranjos.allLineType1.Add(item.Value);
            }

            foreach (var item in importXML.Element("NEW_LAYERS").Elements("NEW_LAYER"))
            {
                arranjos.allNewLayerComposition.Add(item.Value);
            }

            try
            {
                arranjos.allTextSyles.Clear();
                foreach (var item in importXML.Element("NEW_TEXTSTYLES").Elements("TEXT_STYLE"))
                {
                    arranjos.allTextSyles.Add(item.Value);
                }
            }
            catch (Exception)
            {
                try
                {
                    string font = importXML.Element("TEXT_CONFIG").Attribute("TEXT_FONTE").Value;
                    string size = importXML.Element("TEXT_CONFIG").Attribute("TEXT_TAMANHO").Value;
                    string factor = importXML.Element("TEXT_CONFIG").Attribute("TEXT_LARGURA").Value;
                    string italic = importXML.Element("TEXT_CONFIG").Attribute("TEXT_ITALICO").Value;
                    string negrito = importXML.Element("TEXT_CONFIG").Attribute("TEXT_NEGRITO").Value;
                    string angle = importXML.Element("TEXT_CONFIG").Attribute("TEXT_OBLIQUE_ANGLE").Value;

                    arranjos.allTextSyles.Add(EXTTEXTStyleName + ":" +
                        font + ":" +
                        italic + ":" +
                        negrito + ":" +
                        size + ":" +
                        factor + ":" +
                        angle );
                }
                catch (Exception)
                {
                    arranjos.allTextSyles.Add(Class_Arranjos.defaultTextStyle);
                }
        
            }

            for (int i = 0; i < arranjos.allNewLayerComposition.Count; i++)
            {
                arranjos.allNewLayer.Add(arranjos.allNewLayerComposition[i].Split(':').First());
            }

         

            string line;
            foreach (var item in importXML.Element("REMOVE_LAYERS").Elements("REMOVE_LAYER"))
            {
                Class_Filter f = new Class_Filter(arranjos);
                line = item.Value;
                string[] treatment = line.Split('$');
                string[] st = treatment.Last().Split(';');
                f.layerBase = st[0];
                f.SetConjunto(st[1]);
                arranjos.layerRemove.Add(f);
            }

            foreach (var item in importXML.Element("CONVERTERS").Elements("CONVERTER"))
            {
                arranjos.conversor.Add(item.Value);
            }
            foreach (var item in importXML.Element("DLL_OR_LIST_COMMANDS").Elements("COMMAND"))
            {
                arranjos.listLISPCommand.Add(item.Value);
            }

            PROGRAMblockFormatoCaminho = importXML.Element("BLOCK_CONFIG").Attribute("DIRECTORY_TEKLA_CONVERSION").Value;
            EXTCONFCaminhoBlocoInv = importXML.Element("BLOCK_CONFIG").Attribute("DIRECTORY_CAD_CONVERSION").Value;


            string[] allexplodelayers = importXML.Element("BLOCK_CONFIG").Attribute("LAYER_EXPLODE").Value.Split(';');
            arranjos.allExplodeLayers.AddRange(allexplodelayers);

            foreach (var item in importXML.Element("BLOCK_CONFIG").Elements("BLOCK_ATT"))
            {
                Class_BlockClass blockClass = new Class_BlockClass();
                blockClass.blockName = item.Attribute("NOME").Value;
                foreach (var tag in item.Elements("TAG"))
                {
                    Class_TagBlockClass tagTemp = new Class_TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    blockClass.listTags.Add(tagTemp);
                }
                blocks.Add(blockClass);
            }
            foreach (var item in importXML.Element("BLOCK_CONFIG").Elements("BLOCK_ATT_CAD"))
            {
                Class_BlockClass blockClass = new Class_BlockClass();

                string[] linesplit = item.Attribute("NOME").Value.Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements("TAG"))
                {
                    Class_TagBlockClass tagTemp = new Class_TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                blocosi.Add(blockClass);
            }
            foreach (var item in importXML.Element("BLOCK_CONFIG").Elements("BLOCK_ATT_ORIG"))
            {
                Class_BlockClass blockClass = new Class_BlockClass();

                string[] linesplit = item.Attribute("NOME").Value.Split(';');
                blockClass.blockName = linesplit[0];
                blockClass.blockNameRelacao = linesplit[1];
                blockClass.cor = Color.FromArgb(Convert.ToInt32(linesplit[2]));

                foreach (var tag in item.Elements("TAG"))
                {
                    Class_TagBlockClass tagTemp = new Class_TagBlockClass();
                    tagTemp.SetConjunto(tag.Value);
                    string[] linetemp = tag.Value.Split('@');
                    tagTemp.indiceRelacao = Convert.ToInt32(linetemp[linetemp.Count() - 2]);
                    tagTemp.isSociate = Convert.ToBoolean(linetemp[linetemp.Count() - 1]);
                    blockClass.listTags.Add(tagTemp);
                }
                blocoso.Add(blockClass);
            }

            try
            {
                DMBlock = Convert.ToBoolean(importXML.Element("BASIC_CONFIG").Attribute("DMBLOCK").Value);
            }
            catch (Exception)
            {

            }
           
        }
        
        public void SaveXML(string file, Class_Arranjos arranjos, List<Class_BlockClass> blocks, List<Class_BlockClass> blocosi, List<Class_BlockClass> blocoso, StatusConversorItem statusConversorItem)
        {
            string arquivo = AppDomain.CurrentDomain.BaseDirectory +
                                           statusConversorItem.Pasta + "\\";

            if (!Directory.Exists(arquivo))
                Directory.CreateDirectory(arquivo);
            arquivo += file + ".txml";

            if (File.Exists(arquivo))
                File.Delete(arquivo);
            var escritor = new XmlTextWriter(arquivo, Encoding.UTF8) { Formatting = Formatting.Indented };
            escritor.WriteStartDocument();
            escritor.WriteStartElement("CONVERSOR");
            escritor.WriteEndElement();
            escritor.WriteEndDocument();
            escritor.Close();

            XElement xml = XElement.Load(arquivo);
            //COMENTARIO
            {
                XElement x = new XElement("COMMENTS");
                x.Add(new XAttribute("TEXT", EXTCONFComments));
                xml.Add(x);
            }

            //CONFIGURACAO BASICAS
            {
                XElement x = new XElement("BASIC_CONFIG");
                x.Add(new XAttribute("TEKLAORCAD", EXTCONFOrigem));
                x.Add(new XAttribute("CONVERT_DIMENSIONS", EXTCONFIsConvertDimension));
                x.Add(new XAttribute("CONVERT_LAYERS", EXTCONFIsConvertLayer));
                x.Add(new XAttribute("EXCHANGE_FORMAT", EXTCONFIsExchangeFormat));
                x.Add(new XAttribute("SCALE", EXTCONFIsPutOnTheScaleDrawing));
                x.Add(new XAttribute("LISPORDLL", EXTCONFIsExecuteLISP));
                x.Add(new XAttribute("PURGE", EXTCONFIsPurge));
                x.Add(new XAttribute("MESSAGE", PROGRAMMessage));
                x.Add(new XAttribute("DELETE_TEKLA_STRUCTURES", EXTCONFIsDeleteTeklaStructures));
                x.Add(new XAttribute("EXPLOD_BLOCKS", ExplodeBlocks));
                x.Add(new XAttribute("LAYER_TEKLA_STRING", LayerTeklaString));
                x.Add(new XAttribute("LAYER_BLOCK_ATTRIBUTE", LayerBlockAttribute));
                x.Add(new XAttribute("CAD_EXPLODE", EXTCONFInventorExplode));
                x.Add(new XAttribute("DMBLOCK", DMBlock));
                xml.Add(x);
            }
            {
                XElement x = new XElement("DIMENSION_CONFIG");
                x.Add(new XAttribute("DIM_GERAL_HABILIT", EXTDIMGERALHabilit));
                x.Add(new XAttribute("DIM_LAYER", EXTDIMlayer));
                x.Add(new XAttribute("DIM_LINE_COLOR", EXTDIMColorLine));
                x.Add(new XAttribute("DIM_TEXT_COLOR", EXTDIMColorText));
                x.Add(new XAttribute("DIM_STYLE", EXTDIMStyleName));
                x.Add(new XAttribute("DIM_ARROW_TYPE", EXTDIMSeta));
                x.Add(new XAttribute("DIM_SCALE", EXTDIMScale));
                x.Add(new XAttribute("DIM_PRECISION", EXTDIMPrecision));
                x.Add(new XAttribute("DIM_ANGULAR_PRECISION", EXTDIMAngularPrecision));
                x.Add(new XAttribute("DIM_UNIT", EXTDIMUnit));
                x.Add(new XAttribute("DIM_ANGULAR_UNIT", EXTDIMAngularUnit));
                x.Add(new XAttribute("DIM_ARROW_SIZE", EXTDIMSizeSeta));
                x.Add(new XAttribute("DIM_OFFSET", EXTDIMOffsetLineFromRefPoint));
                x.Add(new XAttribute("DIM_OUTSIDE_ALING", EXTDIMOutsideAlign));
                x.Add(new XAttribute("DIM_TAD", EXTDIMTad));
                x.Add(new XAttribute("DIM_POSITION", EXTDIMDimensionPosition));
                x.Add(new XAttribute("DIM_TEXT_FORCED", EXTDIMTextForced));
                x.Add(new XAttribute("DIM_LINE_FORCED", EXTDIMLineForced));
                x.Add(new XAttribute("DIM_DIMEX", EXTDIMDIMEX));
                x.Add(new XAttribute("DIM_BASE_LAYER", EXTDIMBaseLayer));
                x.Add(new XAttribute("DIM_ARROW_FIX", EXTDIMCorrigeSeta));
                x.Add(new XAttribute("DIM_ARROW_FIX_TYPE", EXTDIMCorrigeSetaTipoSeta));
                x.Add(new XAttribute("DIM_ARROW_FACTOR", EXTDIMCorrigeSetaFactor));
                xml.Add(x);
            }
            {
                XElement x = new XElement("TEXT_CONFIG");
                x.Add(new XAttribute("TEXT_STYPE", EXTTEXTStyleName));
                //x.Add(new XAttribute("TEXT_OBLIQUE_ANGLE", EXTTEXTObliqueAngle));
                xml.Add(x);

            }
            {
                XElement x = new XElement("SCALE_CONFIG");
                x.Add(new XAttribute("SCALE_MODE", EXTSCALEManual));
                x.Add(new XAttribute("SCALE_MANUAL_P1_X", EXTSCALEMp1.X));
                x.Add(new XAttribute("SCALE_MANUAL_P1_Y", EXTSCALEMp1.Y));
                x.Add(new XAttribute("SCALE_MANUAL_P1_Z", EXTSCALEMp1.Z));
                x.Add(new XAttribute("SCALE_MANUAL_P2_X", EXTSCALEMp2.X));
                x.Add(new XAttribute("SCALE_MANUAL_P2_Y", EXTSCALEMp2.Y));
                x.Add(new XAttribute("SCALE_MANUAL_P2_Z", EXTSCALEMp2.Z));
                x.Add(new XAttribute("SCALE_AUTO_P1_X", EXTSCALEAp1.X));
                x.Add(new XAttribute("SCALE_AUTO_P1_Y", EXTSCALEAp1.Y));
                x.Add(new XAttribute("SCALE_AUTO_P1_Z", EXTSCALEAp1.Z));
                x.Add(new XAttribute("SCALE_AUTO_P2_X", EXTSCALEAp2.X));
                x.Add(new XAttribute("SCALE_AUTO_P2_Y", EXTSCALEAp2.Y));
                x.Add(new XAttribute("SCALE_AUTO_P2_Z", EXTSCALEAp2.Z));
                x.Add(new XAttribute("SCALE_LAYER", EXTSCALELayer));
                x.Add(new XAttribute("SCALE_TEXT_SIZE", EXTSCALETextSize));
                xml.Add(x);

            }

            {
                XElement x = new XElement("BASIC_LAYERS");
                x.Add(new XAttribute("LTSCALE", EXTLINELtscale));

                foreach (var item in arranjos.allBaseLayer)
                {
                    x.Add(new XElement("BASE_LAYER", item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement("BASIC_LINES");
                foreach (var item in arranjos.allLineType1)
                {
                    x.Add(new XElement("BASE_LINE", item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement("NEW_TEXTSTYLES");
                foreach (var item in arranjos.allTextSyles)
                {
                    x.Add(new XElement("TEXT_STYLE", item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement("NEW_LAYERS");
                if (arranjos.allNewLayerComposition.Count == 0)
                {
                    x.Add(new XElement("NEW_LAYER", "0:WHITE:CONTINUOUS"));
                }

                foreach (var item in arranjos.allNewLayerComposition)
                {
                    x.Add(new XElement("NEW_LAYER", item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement("REMOVE_LAYERS");
                foreach (var item in arranjos.layerRemove)
                {
                    x.Add(new XElement("REMOVE_LAYER", item.layerBase + ";" + item.GetConjunto()));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement("CONVERTERS");
                foreach (var item in arranjos.conversor)
                {
                    x.Add(new XElement("CONVERTER", item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement("DLL_OR_LIST_COMMANDS");

                foreach (var item in arranjos.listLISPCommand)
                {
                    x.Add(new XElement("COMMAND", item));
                }
                xml.Add(x);
            }

            {
                XElement x = new XElement("BLOCK_CONFIG");
                x.Add(new XAttribute("DIRECTORY_TEKLA_CONVERSION", PROGRAMblockFormatoCaminho));

                string removelayer = "";
                foreach (var item in arranjos.allExplodeLayers)
                {
                    removelayer = removelayer + item + ";";
                }
                removelayer = removelayer.Trim(';');

                x.Add(new XAttribute("DIRECTORY_CAD_CONVERSION", EXTCONFCaminhoBlocoInv));
                x.Add(new XAttribute("LAYER_EXPLODE", removelayer));
                foreach (Class_BlockClass item in blocks)
                {
                    XElement x1 = new XElement("BLOCK_ATT");
                    x1.Add(new XAttribute("NOME", item.blockName));

                    foreach (Class_TagBlockClass tag in item.listTags)
                    {
                        x1.Add(new XElement("TAG", tag.GetConjuntoString()));
                    }
                    x.Add(x1);
                }

                foreach (Class_BlockClass item in blocosi)
                {
                    XElement x1 = new XElement("BLOCK_ATT_CAD");
                    x1.Add(new XAttribute("NOME", item.blockName + ";" + item.blockNameRelacao + ";" + item.cor.ToArgb()));

                    foreach (Class_TagBlockClass tag in item.listTags)
                    {
                        x1.Add(new XElement("TAG", tag.GetConjuntoString() + "@" + tag.indiceRelacao + "@" + tag.isSociate));
                    }
                    x.Add(x1);
                }

                foreach (Class_BlockClass item in blocoso)
                {
                    XElement x1 = new XElement("BLOCK_ATT_ORIG");
                    x1.Add(new XAttribute("NOME", item.blockName + ";" + item.blockNameRelacao + ";" + item.cor.ToArgb()));

                    foreach (Class_TagBlockClass tag in item.listTags)
                    {
                        x1.Add(new XElement("TAG", tag.GetConjuntoString() + "@" + tag.indiceRelacao + "@" + tag.isSociate));

                    }
                    x.Add(x1);
                }
                xml.Add(x);
            }
            xml.Save(arquivo);
        }


        public static string LoadConfigDLL()
        {
            string arquivo = Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config");
            if (!File.Exists(arquivo))
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg");

            XElement importXML = XElement.Load(arquivo);


            return importXML.Element("configurations").Attribute("BlocoDM").Value;
        }

        public static void SaveConfigDLL()
        {
            string arquivo = Path.Combine(Path.GetTempPath(), "ConversorDrawind.dll.config");

            if (File.Exists(arquivo))
                return;

            var escritor = new XmlTextWriter(arquivo, Encoding.UTF8) { Formatting = Formatting.Indented };
            escritor.WriteStartDocument();
            escritor.WriteStartElement("configuration");
            escritor.WriteEndElement();
            escritor.WriteEndDocument();
            escritor.Close();

            XElement xml = XElement.Load(arquivo);

            XElement configurations = new XElement("configurations");
            configurations.Add(new XAttribute("BlocoDM", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Quadro_DrawindDM.dwg")));
            xml.Add(configurations);
            
            xml.Save(arquivo);
        }
    }
}
