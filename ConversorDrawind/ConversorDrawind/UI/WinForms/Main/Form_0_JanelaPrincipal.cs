using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;


namespace ConversorDrawind
{
    public partial class Form_0_JanelaPrincipal : UserControl
    {
        public static string extensaoGeral = "DWG";
        public static string LOGdirConvertidos = Path.Combine(Path.GetTempPath(), "ConversorDrawindTemp");
        public static string LOGarqConvertidos = Path.Combine(LOGdirConvertidos, "lastconvertedfiles.log");
        private ContextMenuStrip lBDesenhosContextMenu = new ContextMenuStrip();
        private Configuration configuration = new Configuration();
        private Arranjos arranjos = new Arranjos();
        private string previousIndex;
        private bool NotLoop = true;
        private GetInfo myDrawing = null;
        private GetInfo myDrawingBlock = null;
        private List<Block> listBlocks = new List<Block>();
        private List<Block> listBlocksInv = new List<Block>();
        private List<Block> listBlocksOrig = new List<Block>();

        public bool EXTDIMCorrigeSeta = false;
        public string EXTDIMCorrigeSetaTipoSeta = "Oblique";
        public double EXTDIMCorrigeSetaFactor = 7.23;

        public Form_0_JanelaPrincipal()
        {


            Form_1_Senha s = new Form_1_Senha();
            bool IsValid = s.CheckSerial();
            if (!IsValid)
                s.ShowDialog();
            if (IsValid)
            {
               

                InitializeComponent();
                StatusConversor.ValueMember = "Nome";
                StatusConversor.Items.Add(new StatusConversorItem("Obras Ativas", "TemplatesAtivos"));
                StatusConversor.Items.Add(new StatusConversorItem("Obras Inativas", "TemplatesInativos"));
                StatusConversor.SelectedIndex = 0;

                Configuration.SaveConfigDLL();
                Show();
                Activate();
                CarregarTemplates();
                /*if (File.Exists(LOGarqConvertidos))
                    ButtonRestaurar.Enabled = true;
                else*/
                    ButtonRestaurar.Enabled = false;
                previousIndex = cBListaDeConversores.Text;
                CarregarTemplatesCBListaDeConversores();
                lBDesenhos.ContextMenuStrip = lBDesenhosContextMenu;
                lBDesenhosContextMenu.Opening += new CancelEventHandler(lBDesenhosContextMenu_Opening);
                lBDesenhos.ContextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(listboxContextMenu_Click);

                CarregarTemplateDeConfiguracaoPTela();
                gBScaleManual.Enabled = true;
                gBScaleAuto.Enabled = false;
                CarregarComentarioNoTemplate();
                loadDimensionTab();

                AbilitarDesabilitarInicial();
                loadAtributarTab();
                LoadExplodeLayers();
                typeconversion();
                if (radioButton1.Checked)
                    tabControl1.SelectTab(0);
                else
                    tabControl1.SelectTab(1);
                try
                {
                    StatusConversor.SelectedIndex = ConversorDrawind.Properties.Settings.Default.StatusConversor;
                    if (lBConversores.Items.Contains(ConversorDrawind.Properties.Settings.Default.ConversorAtual))
                        lBConversores.Text = ConversorDrawind.Properties.Settings.Default.ConversorAtual;
                    cBManterArquivosAbertos.Checked = ConversorDrawind.Properties.Settings.Default.OpeACAD;
                    extensao.Text = ConversorDrawind.Properties.Settings.Default.Extensao;
                    if (cBListaDeConversores.Items.Contains(ConversorDrawind.Properties.Settings.Default.ConversorAtualConfig))
                    {
                        cBListaDeConversores.Text = ConversorDrawind.Properties.Settings.Default.ConversorAtualConfig;
                    }

                }
                catch (Exception)
                {

                }


            }
        }

        public void Activate()
        {
            FindForm()?.Activate();
        }

        public new void SetTopLevel(bool value)
        {
        }

        public static void CloseAllInstance()
        {
            Process[] aCAD = Process.GetProcessesByName("acad");

            foreach (Process aCADPro in aCAD)
            {
                aCADPro.CloseMainWindow();
            }
        }

        private void CarregarTemplates()
        {
            lBConversores.Items.Clear();
            try
            {
                foreach (string converterName in LoadConverterList((StatusConversorItem)StatusConversor.SelectedItem))
                    lBConversores.Items.Add(converterName);
                if (lBConversores.Items.Count > 0)
                {
                    lBConversores.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {

            }
        }

        private void CarregarComentarioNoTemplate()
        {
            if (lBConversores.Items.Count > 0 && lBConversores.SelectedItem != null)
            {
                rTBComentarios.Clear();
                string itemSelecionado = lBConversores.SelectedItem.ToString();
                string nomeTemplade = ConverterFileService.GetTxmlPath(itemSelecionado, (StatusConversorItem)StatusConversor.SelectedItem);
                Configuration CT = new Configuration();

              
                if (File.Exists(nomeTemplade))
                {
                    CT.LoadXML(itemSelecionado, new Arranjos(), new List<Block>(), new List<Block>(), new List<Block>(), (StatusConversorItem)StatusConversor.SelectedItem);
                    rTBComentarios.AppendText(CT.EXTCONFComments);
                }

                else
                {
                    CarregarTemplates();
                    CarregarTemplatesCBListaDeConversores();
                    CarregarComentarioNoTemplate();
                    MessageBox.Show("O template selecionado foi excluÃ­do ou renomeado, a lista de templates foi atualizada.",
                 "Error",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning,
                 MessageBoxDefaultButton.Button1);
                }
            }
           
        }

        private void CarregarTemplatesCBListaDeConversores()
        {
            cBListaDeConversores.Items.Clear();
            try
            {
                foreach (string converterName in LoadConverterList((StatusConversorItem)StatusConversor.Items[0]))
                    cBListaDeConversores.Items.Add(converterName);
            }
            catch (Exception)
            {

            }
        }

        private List<string> LoadConverterList(StatusConversorItem statusConversorItem)
        {
            return ConverterFileService.ListConverterNames(statusConversorItem);
        }

        private void CarregarTemplateDeConfiguracaoPTela()
        {
            LoadConfigurationToControls();
        }

        private void LoadConfigurationToControls()
        {
            rTBAddComentarios.Text = configuration.EXTCONFComments;
            cBConverterCotas.Checked = configuration.EXTCONFIsConvertDimension;
            cBConverterLayer.Checked = configuration.EXTCONFIsConvertLayer;
            cBAtributarFormato.Checked = configuration.EXTCONFIsExchangeFormat;
            cBEscalaDesenho.Checked = configuration.EXTCONFIsPutOnTheScaleDrawing;
            cBExLISP.Checked = configuration.EXTCONFIsExecuteLISP;
            //cBExDLL.Checked = configuration.EXTCONFIsExecuteDLL;
            //cBFirstRum.Text = configuration.EXTCONFIsFirstRum;
            cBMenssage.Checked = configuration.PROGRAMMessage;

            dGVLayer.Rows.Clear();

            for (int i = 0; i < arranjos.conversor.Count; i++)
            {
                string[] temp = arranjos.conversor[i].Split(';');
                dGVLayer.Rows.Add(temp[0], temp[1], temp[2]);

            }
            
            //CCCB_HabilitAdvanced.Checked = configuration.EXTDIMGERALHabilit;
            if (!CCCB_Layer.Items.Contains(configuration.EXTDIMlayer))
                CCCB_Layer.Items.Add(configuration.EXTDIMlayer);
            CCCB_Layer.Text = configuration.EXTDIMlayer;
            if (!CCCB_ColorLine.Items.Contains(configuration.EXTDIMColorLine))
                CCCB_ColorLine.Items.Add(configuration.EXTDIMColorLine);
            CCCB_ColorLine.Text = configuration.EXTDIMColorLine;
            if (!CCCB_ColorText.Items.Contains(configuration.EXTDIMColorText))
                CCCB_ColorText.Items.Add(configuration.EXTDIMColorText);
            CCCB_ColorText.Text = configuration.EXTDIMColorText;
            CCTB_DimStyle.Text = configuration.EXTDIMStyleName;

            CCTB_TypeArrow.Text = configuration.EXTDIMSeta;
            CCTB_DimScale.Text = Convert.ToString(configuration.EXTDIMScale);
            CCTB_DimLinearPrecision.Text = Convert.ToString(configuration.EXTDIMPrecision);
            CCTB_DimAngularPresicion.Text = Convert.ToString(configuration.EXTDIMAngularPrecision);
            CCTB_DimLinearFormatUnit.Text = Convert.ToString(configuration.EXTDIMUnit);
            CCTB_DimAngularFormatUnit.Text = Convert.ToString(configuration.EXTDIMAngularUnit);
            CCTB_DimArrowSize.Text = Convert.ToString(configuration.EXTDIMSizeSeta);
            CCTB_DimOffset.Text = Convert.ToString(configuration.EXTDIMOffsetLineFromRefPoint);
            CCTB_DimOutsideAling.Text = Convert.ToString(configuration.EXTDIMOutsideAlign);
            CCTB_TextPlacementVertical.Text = Convert.ToString(configuration.EXTDIMTad);
            CCTB_TextAlignment.Text = Convert.ToString(configuration.EXTDIMDimensionPosition);
            CCTB_TextInside.Text = Convert.ToString(configuration.EXTDIMTextForced);
            CCTB_LineForced.Text = Convert.ToString(configuration.EXTDIMLineForced);
            CCTB_LineExt.Text = Convert.ToString(configuration.EXTDIMDIMEX);
            TB_Ltscale.Text = Convert.ToString(configuration.EXTLINELtscale);
            if (!CCTB_LayerBase.Text.Contains(configuration.EXTDIMBaseLayer))
                CCTB_LayerBase.Items.Add(configuration.EXTDIMBaseLayer);
            CCTB_LayerBase.Text = configuration.EXTDIMBaseLayer;
            cBPurge.Checked = configuration.EXTCONFIsPurge;

            p1x.Text = Convert.ToString(configuration.EXTSCALEMp1.X);
            p1y.Text = Convert.ToString(configuration.EXTSCALEMp1.Y);
            p1z.Text = Convert.ToString(configuration.EXTSCALEMp1.Z);

            p2x.Text = Convert.ToString(configuration.EXTSCALEMp2.X);
            p2y.Text = Convert.ToString(configuration.EXTSCALEMp2.Y);
            p2z.Text = Convert.ToString(configuration.EXTSCALEMp2.Z);

            ap1x.Text = Convert.ToString(configuration.EXTSCALEAp1.X);
            ap1y.Text = Convert.ToString(configuration.EXTSCALEAp1.Y);
            ap1z.Text = Convert.ToString(configuration.EXTSCALEAp1.Z);

            ap2x.Text = Convert.ToString(configuration.EXTSCALEAp2.X);
            ap2y.Text = Convert.ToString(configuration.EXTSCALEAp2.Y);
            ap2z.Text = Convert.ToString(configuration.EXTSCALEAp2.Z);

            if (configuration.EXTSCALEManual)
                rBScaleManual.Checked = true;
            else
                rBScaleAuto.Checked = true;
            if (!zoomLayerFilter.Text.Contains(configuration.EXTSCALELayer))
                zoomLayerFilter.Items.Add(configuration.EXTSCALELayer);
            zoomLayerFilter.Text = configuration.EXTSCALELayer;
            zoomTextSize.Text = Convert.ToString(configuration.EXTSCALETextSize);

            cBDeleteTekla.Checked = configuration.EXTCONFIsDeleteTeklaStructures;
            tBDiretorioFormatoAtributado.Text = configuration.PROGRAMblockFormatoCaminho;
            listBoxBlock.Items.Clear();
            foreach (Block item in listBlocks)
            {
                listBoxBlock.Items.Add(item.blockName);
            }
            dataGridViewLayer.Rows.Clear();
            for (int i = 0; i < arranjos.layerRemove.Count; i++)
            {
                dataGridViewLayer.Rows.Add(arranjos.layerRemove[i].layerBase,arranjos.layerRemove[i].GetConjunto() );
            }

            lBLISP.Items.Clear();
            foreach (string item in arranjos.listLISPCommand)
            {
                lBLISP.Items.Add(item); 
            }

            lBDLL.Items.Clear();
            foreach (string item in arranjos.listDLLCommand)
            {
                lBDLL.Items.Add(item);
            }

            EXTDIMCorrigeSeta = configuration.EXTDIMCorrigeSeta;
            EXTDIMCorrigeSetaTipoSeta = configuration.EXTDIMCorrigeSetaTipoSeta;
            EXTDIMCorrigeSetaFactor = configuration.EXTDIMCorrigeSetaFactor;
            cBExplodir.Checked = configuration.EXTCONFInventorExplode;
            cBDMblock.Checked = configuration.DMBlock;

            if (configuration.EXTCONFOrigem == 0)
                radioButton1.Checked = true;
            else
                radioButton2.Checked = true;
            typeconversion();

            LoadExplodeLayers();

            EXPL_ListaExplodeLayers.Items.Clear();
            foreach (string item in arranjos.allExplodeLayers)
            {
                EXPL_ListaExplodeLayers.Items.Add(item);
            }
            if (radioButton1.Checked)
                tabControl1.SelectTab(0);
            else
                tabControl1.SelectTab(1);



            LBNEW_BlocosInventor.Items.Clear();
            foreach (Block item in listBlocksInv)
            {
                LBNEW_BlocosInventor.Items.Add(item.blockName);
            }

            LBNEW_BlocosOriginais.Items.Clear();
            foreach (Block item in listBlocksOrig)
            {
                LBNEW_BlocosOriginais.Items.Add(item.blockName);
            }

            LBNEW_Relacoes.Items.Clear();
            foreach (Block item in listBlocksOrig)
            {
               if( item.blockNameRelacao != "")
               {
                   LBNEW_Relacoes.Items.Add(item.blockNameRelacao + "    = >    " + item.blockName);
               }
            }
            TBNEW_BlocosOriginais.Text = configuration.EXTCONFCaminhoBlocoInv;

            ExplodirBlocos.Checked = configuration.ExplodeBlocks;
            LayerTextoTekla.Text = configuration.LayerTeklaString;
            LayerBlocosAtt.Text = configuration.LayerBlockAttribute;

            LoadEstiloTexto();

        }

        private void LoadEstiloTexto()
        {
            EstiloTexto.Items.Clear();
            if (arranjos.allTextSyles.Count() == 0)
                arranjos.allTextSyles.Add(Arranjos.defaultTextStyle);
            EstiloTexto.Items.AddRange(arranjos.allTextSyles.Select(a => a.Split(':').First()).ToArray());

            if (EstiloTexto.Items.Cast<string>().Where(a => a.ToUpper() == configuration.EXTTEXTStyleName.ToUpper()).Count() > 0)
                EstiloTexto.Text = configuration.EXTTEXTStyleName;
            else
                EstiloTexto.Text = EstiloTexto.Items.Cast<string>().First();
        }

        private void CarregarTemplateDeTelaPConviguracao()
        {
            ReadConfigurationFromControls();
        }

        private void ReadConfigurationFromControls()
        {
            configuration.DMBlock = cBDMblock.Checked;
            configuration.EXTCONFComments = rTBAddComentarios.Text;
            configuration.EXTCONFIsConvertDimension = cBConverterCotas.Checked;
            configuration.EXTCONFIsConvertLayer = cBConverterLayer.Checked;
            configuration.EXTCONFIsExchangeFormat = cBAtributarFormato.Checked;
            configuration.EXTCONFIsPutOnTheScaleDrawing = cBEscalaDesenho.Checked;
            configuration.EXTCONFIsExecuteLISP = cBExLISP.Checked;
            //configuration.EXTCONFIsExecuteDLL = cBExDLL.Checked;
            //configuration.EXTCONFIsFirstRum = cBFirstRum.Text;
            configuration.PROGRAMMessage = cBMenssage.Checked;

            //configuration.EXTDIMGERALHabilit = true;//CCCB_HabilitAdvanced.Checked;
            configuration.EXTDIMlayer = CCCB_Layer.Text;
            configuration.EXTDIMColorLine = CCCB_ColorLine.Text;
            configuration.EXTDIMColorText = CCCB_ColorText.Text;
            configuration.EXTDIMStyleName = CCTB_DimStyle.Text;
            configuration.EXTTEXTStyleName = EstiloTexto.Text;
            configuration.EXTDIMSeta = CCTB_TypeArrow.Text;
            configuration.EXTDIMScale = NumericTextParser.ToDouble(CCTB_DimScale.Text);
            configuration.EXTDIMPrecision = Convert.ToInt32(CCTB_DimLinearPrecision.Text);
            configuration.EXTDIMAngularPrecision = Convert.ToInt32(CCTB_DimAngularPresicion.Text);
            configuration.EXTDIMUnit = Convert.ToInt32(CCTB_DimLinearFormatUnit.Text);
            configuration.EXTDIMAngularUnit = Convert.ToInt32(CCTB_DimAngularFormatUnit.Text);
            configuration.EXTDIMSizeSeta = NumericTextParser.ToDouble(CCTB_DimArrowSize.Text);
            configuration.EXTDIMOffsetLineFromRefPoint = NumericTextParser.ToDouble(CCTB_DimOffset.Text);
            configuration.EXTDIMOutsideAlign = Convert.ToBoolean(CCTB_DimOutsideAling.Text);
            configuration.EXTDIMTad = Convert.ToInt32(CCTB_TextPlacementVertical.Text);
            configuration.EXTDIMDimensionPosition = Convert.ToBoolean(CCTB_TextAlignment.Text);
            configuration.EXTDIMTextForced = Convert.ToBoolean(CCTB_TextInside.Text);
            configuration.EXTDIMLineForced = Convert.ToBoolean(CCTB_LineForced.Text);
            configuration.EXTDIMDIMEX = NumericTextParser.ToDouble(CCTB_LineExt.Text);
            configuration.EXTLINELtscale = NumericTextParser.ToDouble(TB_Ltscale.Text);
            configuration.EXTDIMBaseLayer = CCTB_LayerBase.Text;
            configuration.EXTCONFIsPurge = cBPurge.Checked;

            configuration.EXTSCALEMp1.X = NumericTextParser.ToDouble(p1x.Text);
            configuration.EXTSCALEMp1.Y = NumericTextParser.ToDouble(p1y.Text);
            configuration.EXTSCALEMp1.Z = NumericTextParser.ToDouble(p1z.Text);

            configuration.EXTSCALEMp2.X = NumericTextParser.ToDouble(p2x.Text);
            configuration.EXTSCALEMp2.Y = NumericTextParser.ToDouble(p2y.Text);
            configuration.EXTSCALEMp2.Z = NumericTextParser.ToDouble(p2z.Text);

            configuration.EXTSCALEAp1.X = NumericTextParser.ToDouble(ap1x.Text);
            configuration.EXTSCALEAp1.Y = NumericTextParser.ToDouble(ap1y.Text);
            configuration.EXTSCALEAp1.Z = NumericTextParser.ToDouble(ap1z.Text);

            configuration.EXTSCALEAp2.X = NumericTextParser.ToDouble(ap2x.Text);
            configuration.EXTSCALEAp2.Y = NumericTextParser.ToDouble(ap2y.Text);
            configuration.EXTSCALEAp2.Z = NumericTextParser.ToDouble(ap2z.Text);

            configuration.EXTSCALEManual = rBScaleManual.Checked;
            configuration.EXTSCALELayer = zoomLayerFilter.Text;
            configuration.EXTSCALETextSize = NumericTextParser.ToDouble(zoomTextSize.Text);

            configuration.EXTCONFIsDeleteTeklaStructures = cBDeleteTekla.Checked;
            configuration.PROGRAMblockFormatoCaminho = tBDiretorioFormatoAtributado.Text;

            arranjos.layerRemove.Clear();
            for (int i = 0; i < dataGridViewLayer.Rows.Count; i++)
            {
                Filter f = new Filter(arranjos);
                f.layerBase = dataGridViewLayer.Rows[i].Cells[0].Value.ToString();
                f.SetConjunto(dataGridViewLayer.Rows[i].Cells[1].Value.ToString());
                arranjos.layerRemove.Add(f);
            }

            arranjos.listLISPCommand.Clear();
            foreach (string item in lBLISP.Items)
            {
                arranjos.listLISPCommand.Add(item);
            }

            arranjos.listDLLCommand.Clear();
            foreach (string item in lBDLL.Items)
            {
                arranjos.listDLLCommand.Add(item);
            }

            configuration.EXTDIMCorrigeSeta = EXTDIMCorrigeSeta;
            configuration.EXTDIMCorrigeSetaTipoSeta = EXTDIMCorrigeSetaTipoSeta;
            configuration.EXTDIMCorrigeSetaFactor = EXTDIMCorrigeSetaFactor;

            configuration.EXTCONFInventorExplode = cBExplodir.Checked;
            if (radioButton1.Checked)
                configuration.EXTCONFOrigem = 0;
            else
                configuration.EXTCONFOrigem = 1;
            arranjos.allExplodeLayers.Clear();
            foreach (string item in EXPL_ListaExplodeLayers.Items)
            {
                arranjos.allExplodeLayers.Add(item);
            }

            configuration.EXTCONFCaminhoBlocoInv = TBNEW_BlocosOriginais.Text;
            configuration.ExplodeBlocks = ExplodirBlocos.Checked;
            configuration.LayerTeklaString = LayerTextoTekla.Text;
            configuration.LayerBlockAttribute = LayerBlocosAtt.Text;
        }

        private ConversionPreflightResult ValidateConversionConfiguration(Configuration configurationToValidate)
        {
            return ConversionPreflightValidator.ValidateFormatPath(configurationToValidate);
        }

        private void lBDesenhosContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (lBDesenhos.SelectedIndex != -1)
            {
                lBDesenhosContextMenu.Items.Clear();
                lBDesenhosContextMenu.Items.Add(string.Format("Remover"));
            }
        }

        public static double[] GetPoint(double[] ponto)
        {
            return new double[]{Math.Round(ponto[0], 1), Math.Round(ponto[1], 1), Math.Round(ponto[2], 1)};
        }



        private void bConverter_Click(object sender, EventArgs e)
        {
            extensaoGeral = extensao.Text;
            try
            {
                string nomeTemplate = ConverterFileService.GetTxmlPath(lBConversores.Text, (StatusConversorItem)StatusConversor.SelectedItem);
                if (File.Exists(nomeTemplate))
                {
                    if (lBDesenhos.Items.Count != 0)
                    {
                        if (lBConversores.SelectedIndex != -1)
                        {
                            Param1 p1 = new Param1();
                            List<string> temp = new List<string>();
                            foreach (string item in lBDesenhos.Items)
                            {
                                temp.Add(item);
                            }
                            Configuration conf = new Configuration();

                            Arranjos arr = new Arranjos();
                            ConverterFileService.LoadConverter(conf, lBConversores.Text, arr, new List<Block>(), new List<Block>(), new List<Block>(), (StatusConversorItem)StatusConversor.SelectedItem);
                            p1.desenhosName = temp.ToArray();
                            p1.conversorName = lBConversores.Text;
                            p1.closedesenhos = cBManterArquivosAbertos.Checked;
                            p1.configuration = conf;
                            p1.arranjos = arr;
                            p1.StatusConversorItem = (StatusConversorItem)StatusConversor.SelectedItem;
                            ConversionPreflightResult preflightResult = ValidateConversionConfiguration(p1.configuration);
                            if (!preflightResult.CanConvert)
                            {
                                MessageBox.Show(new Form() { TopMost = true }, "NÃ£o Ã© possÃ­vel prosseguir com a conversÃ£o porque nÃ£o foi possÃ­vel localizar o formato para substituiÃ§Ã£o.\nCaminho do formado: " + preflightResult.MissingFormatPath,
                                             "Error",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Warning,
                                             MessageBoxDefaultButton.Button1);
                                MessageBox.Show("          A conversÃ£o falhou!          ",
                                         "InformaÃ§Ã£o",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Warning,
                                          MessageBoxDefaultButton.Button1);
                                return;
                            }
                            try
                            {
                                Form_2_Processo progresso = new Form_2_Processo(p1);
                                progresso.ShowDialog();
                            }
                            catch (Exception e2)
                            {
                                Form_0_JanelaPrincipal.ControladorT = false;
                                MessageBox.Show(new Form() { TopMost = true }, "A conversÃ£o nÃ£o pode ser feita em todos os desenhos devido ao seguinte erro: \n" + e2.Message,
                                                 "Error",
                                                 MessageBoxButtons.OK,
                                                 MessageBoxIcon.Warning,
                                                 MessageBoxDefaultButton.Button1);
                            }


                            this.SetTopLevel(true);
                            this.Activate();
                            if (Form_2_Processo.IsCanceled)
                                MessageBox.Show("       ConversÃ£o cancelada pelo usuÃ¡rio!       ",
                                     "InformaÃ§Ã£o",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information,
                                      MessageBoxDefaultButton.Button1);
                            else
                            {
                                Form_2_ProcessoEnd fpe = new Form_2_ProcessoEnd();
                                fpe.ShowDialog();
                            }

                        }
                        else
                        {
                            MessageBox.Show("NÃ£o existe nenhum conversor selecionado.",
                                             "Error",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Warning,
                                             MessageBoxDefaultButton.Button1);
                        }
                    }
                    else
                    {
                        MessageBox.Show("NÃ£o existe desenhos na lista para serem convertidos.",
                                         "Error",
                                         MessageBoxButtons.OK,
                                         MessageBoxIcon.Warning,
                                         MessageBoxDefaultButton.Button1);
                    }
                }
                else
                {
                    CarregarTemplates();
                    CarregarTemplatesCBListaDeConversores();
                    CarregarComentarioNoTemplate();
                    MessageBox.Show("O template selecionado foi excluÃ­do ou renomeado, a lista de templates foi atualizada.",
                 "Error",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Warning,
                 MessageBoxDefaultButton.Button1);
                }

            }
            catch (Exception)
            {
            }
            finally
            {
                if (File.Exists(LOGarqConvertidos))
                    ButtonRestaurar.Enabled = true;
                else
                    ButtonRestaurar.Enabled = false;
            }
        }

        public static volatile bool controladorT = true;

        public static bool ControladorT
        {
            get { return controladorT; }
            set { controladorT = value; }
        }

        public static volatile bool controladorT2 = true;

        public static bool ControladorT2
        {
            get { return controladorT2; }
            set { controladorT2 = value; }
        }

        private static void ThreadMethod2()
        {
            controladorT = true;
            controladorT2 = true;
            Form_1_Informacao u = new Form_1_Informacao();
            u.Show();
            while (controladorT)
            {
                if (ControladorT2 == true && u.TopLevel == false)
                    u.SetTopLevelInfUser(true);
                else if (ControladorT2 == false && u.TopLevel == true)
                    u.SetTopLevelInfUser(false);

                u.AtualizarStatus("Convertendo");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Convertendo.");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Convertendo..");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Convertendo...");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Convertendo....");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Convertendo.....");
                u.Update();
                Thread.Sleep(50);
            }
            u.Close();
            u.Dispose();
        }

        public static void StopStatusThread(Thread thread)
        {
            controladorT = false;
            if (thread != null && thread.IsAlive)
                thread.Join(TimeSpan.FromSeconds(2));
        }

        public static void ThreadMethodAnalisando()
        {
            controladorT = true;
            Form_1_Informacao u = new Form_1_Informacao();
            u.Show();
            while (controladorT)
            {
                u.AtualizarStatus("Analisando");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Analisando.");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Analisando..");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Analisando...");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Analisando....");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Analisando.....");
                u.Update();
                Thread.Sleep(50);
            }
            u.Close();
            u.Dispose();
        }

        public static void ThreadMethodAbrindoCad()
        {
            controladorT = true;
            Form_1_Informacao u = new Form_1_Informacao();
            u.Show();
            while (controladorT)
            {
                u.AtualizarStatus("Processando");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Processando.");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Processando..");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Processando...");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Processando....");
                u.Update();
                Thread.Sleep(50);
                u.AtualizarStatus("Processando.....");
                u.Update();
                Thread.Sleep(50);
            }
            u.Close();
            u.Dispose();
        }
        
        private void listboxContextMenu_Click(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Remover")
                if (lBDesenhos.SelectedIndex != -1)
                {
                    lBDesenhos.Items.Remove(lBDesenhos.SelectedItem.ToString());
                }
        }

        private void lBConversores_Click(object sender, EventArgs e)
        {
            CarregarComentarioNoTemplate();
        }

        private void bAdicionarArquivos_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Drawing (*.dwg)|*.dwg|Drawing (*.dxf)|*.dxf";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string[] files = openFileDialog.FileNames;
                    foreach (string item in files)
                    {
                        lBDesenhos.Items.Remove(item);
                        lBDesenhos.Items.Add(item);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void bLimparLista_Click(object sender, EventArgs e)
        {
            lBDesenhos.Items.Clear();
        }

        private void salar3()
        {
            Regex regex = new Regex(@"^[a-z|0-9|_|\-Ã£ÃµÃ¢ÃªÃ®Ã´Ã»Ã¡Ã©Ã­Ã³ÃºÃ Ã¨Ã¬Ã²Ã¹Ã§& ]*$", RegexOptions.IgnoreCase);
            String s = tbListaDeConversores.Text;
            Match match = regex.Match(s);
            string t = s.Trim();

            if (match.Success == true && !string.IsNullOrWhiteSpace(s) && s == t && !s.Contains('|'))
            {
                bool isSave = true;
                if (cBListaDeConversores.Text.ToUpper() != tbListaDeConversores.Text.ToUpper())
                {
                    string filetemp = ConverterFileService.GetTxmlPath(tbListaDeConversores.Text, (StatusConversorItem)StatusConversor.Items[0]);
                    if (File.Exists(filetemp))
                    {
                        if (MessageBox.Show("JÃ¡ existe um arquivo com esse nome." +
                       "\nDeseja substituir?",
                       "AtenÃ§Ã£o",
                       MessageBoxButtons.YesNo,
                       MessageBoxIcon.Exclamation,
                       MessageBoxDefaultButton.Button2) == DialogResult.No)
                        {
                            isSave = false;
                        }
                    }
                }

                if (isSave)
                {
                    this.arranjos.conversor.Clear();
                    for (int i = 0; i < dGVLayer.Rows.Count; i++)
                    {
                        this.arranjos.conversor.Add(dGVLayer.Rows[i].Cells[0].Value.ToString() + ";" +
                                                    dGVLayer.Rows[i].Cells[1].Value.ToString() + ";" +
                                                    dGVLayer.Rows[i].Cells[2].Value.ToString());
                    }

                    CarregarTemplateDeTelaPConviguracao();
                    SaveSelectedConverter(tbListaDeConversores.Text.ToUpper());

                    CarregarTemplates();
                    CarregarTemplatesCBListaDeConversores();
                    cBListaDeConversores.Text = tbListaDeConversores.Text;
                }
            }
            else
            {
                MessageBox.Show("Nome invÃ¡lido!",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1);
            }
        }

        private void bSalvar3_Click(object sender, EventArgs e)
        {
            int indexTab1 = -1;
            int indexTab2 = -1;
            if(dGVLayer.CurrentRow != null)
                indexTab1 = dGVLayer.CurrentRow.Index;
            indexTab2 = lBConversores.SelectedIndex;
            salar3();
            if (indexTab1 != -1)
                dGVLayer.CurrentCell = dGVLayer.Rows[indexTab1].Cells[0];
                 //tabela1BindingSource.Position = indexTab1; *****xxxxx*****
            if (indexTab2 != -1)
                lBConversores.SelectedIndex = indexTab2;
        }

        private void SaveSelectedConverter(string converterName)
        {
            ConverterFileService.SaveConverter(configuration, converterName, this.arranjos, this.listBlocks, this.listBlocksInv, this.listBlocksOrig, (StatusConversorItem)StatusConversor.Items[0]);
        }

        private void cBListaDeConversores_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBListaDeConversores.SelectedItem != null)
            {
                if (NotLoop)
                {
                    if (IsModify())
                    {
                        
                        if (MessageBox.Show("Houve alteraÃ§Ãµes no templade." +
                                  "\nDeseja realmente selecionar outro templade?",
                                  "AtenÃ§Ã£o",
                                  MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Exclamation,
                                  MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                        {
                            tbListaDeConversores.Text = cBListaDeConversores.SelectedItem.ToString();
                            previousIndex = cBListaDeConversores.Text;
                            ConverterFileService.LoadConverter(configuration, cBListaDeConversores.SelectedItem.ToString(), this.arranjos, this.listBlocks, this.listBlocksInv, this.listBlocksOrig, (StatusConversorItem)StatusConversor.Items[0]);
                            CarregarTemplateDeConfiguracaoPTela();
                            loadDimensionTab();
                            loadScaleTab();
                            loadAtributarTab();

                        }
                        else
                        {
                            NotLoop = false;
                            tbListaDeConversores.Text = previousIndex;
                            cBListaDeConversores.Text = (previousIndex == "") ? null : previousIndex;
                        }
                    }
                    else
                    {
                        tbListaDeConversores.Text = cBListaDeConversores.SelectedItem.ToString();
                        previousIndex = cBListaDeConversores.Text;
                        ConverterFileService.LoadConverter(configuration, cBListaDeConversores.SelectedItem.ToString(), this.arranjos, this.listBlocks, this.listBlocksInv, this.listBlocksOrig, (StatusConversorItem)StatusConversor.Items[0]);
                        CarregarTemplateDeConfiguracaoPTela();
                        loadDimensionTab();
                        loadScaleTab();
                        loadAtributarTab();
      
                    }
     
                }
                else
                {
                    NotLoop = true;
                }
            }
        }

        private void JanelaPrincipal_Load(object sender, EventArgs e)
        {
            dGVLayer.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dGVLayer.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dGVLayer.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewLayer.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridViewLayer.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

        
        }

        private void bIncL_Click(object sender, EventArgs e)
        {
            if (dGVLayer.Rows.Count == 0)
            {
                Filter filtro = new Filter(this.arranjos);
                filtro.SetConjunto();
                NewLayer novoLayer = new NewLayer(this.arranjos);
                novoLayer.SetConjunto();
                dGVLayer.Rows.Add("", filtro.GetConjunto(), novoLayer.GetConjunto());
                dGVLayer.CurrentCell = dGVLayer.Rows[dGVLayer.Rows.Count - 1].Cells[0];
            }
            else if (dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[0].Value.ToString() != "")
            {
                dGVLayer.Rows.Insert(dGVLayer.CurrentRow.Index, dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[0].Value.ToString(),
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[1].Value.ToString(),
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[2].Value.ToString());
                    dGVLayer.CurrentCell = dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[0];
            }

          
            //tabela1BindingSource.MoveLast(); *****xxxxx*****
        }

        private void bExcL_Click(object sender, EventArgs e)
        {
            if (dGVLayer.SelectedRows.Count > 0 && dGVLayer.CurrentRow.Index != -1)
            {
                if (MessageBox.Show("Deseja realmente excluir todas as linhas selecionadas?",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    for (int i = dGVLayer.SelectedRows.Count-1; i > -1 ; i--)
                    {
                        dGVLayer.Rows.RemoveAt(dGVLayer.SelectedRows[i].Index);
                    }
                }
            }
        }

        private void dGVLayer_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dataGridView = sender as DataGridView;
            System.Windows.Forms.DataGridView.HitTestInfo hitTestInfo = dataGridView.HitTest(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                dGVLayer_CellDoubleClick(sender, hitTestInfo.RowIndex, hitTestInfo.ColumnIndex);

            }
        }

        private void dGVLayer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dGVLayer_CellDoubleClick(sender, e.RowIndex, e.ColumnIndex);
        }

        private void dGVLayer_CellDoubleClick(object sender, int rowIndex, int columIndex)
        {
            if (columIndex != -1 && rowIndex != -1)
            {
               
                    if (columIndex == 0)
                    {
                        string valor = string.Empty;
                        if (dGVLayer.Rows.Count != 0)
                            valor = dGVLayer.SelectedRows[0].Cells[0].Value.ToString();
                        Form_4_LayersLayerBase myLayer = new Form_4_LayersLayerBase(valor, arranjos);
                        myLayer.ShowDialog();
                        foreach (DataGridViewRow row in dGVLayer.SelectedRows)
                        {
                            row.Cells[0].Value = myLayer.layerBase;
                            myLayer.Dispose();
                        }
                    }
                    if (columIndex == 1)
                    {
                        Form_4_LayersFilter myFilter = new Form_4_LayersFilter(dGVLayer.SelectedRows[0].Cells[1].Value.ToString(), arranjos);
                        myFilter.ShowDialog();
                        foreach (DataGridViewRow row in dGVLayer.SelectedRows)
                        {
                            row.Cells[1].Value = myFilter.filtro.GetConjunto();
                            myFilter.Dispose();
                        }
                    }
                    if (columIndex == 2)
                    {
                        Form_4_LayersNewLayer myNewLayer = new Form_4_LayersNewLayer(dGVLayer.SelectedRows[0].Cells[2].Value.ToString(), arranjos);
                        myNewLayer.ShowDialog();
                        foreach (DataGridViewRow row in dGVLayer.SelectedRows)
                        {
                            row.Cells[2].Value = myNewLayer.novoLayer.GetConjunto();
                            myNewLayer.Dispose();
                        }
                    }
                
            }
        }

        private void bCarregarLC_Click(object sender, EventArgs e)
        {
            Form_3_ConfigurarLayers newLayerConfiguration = new Form_3_ConfigurarLayers(this.arranjos);
            newLayerConfiguration.OpenAcadLoadLayerExterno();
            newLayerConfiguration.ShowDialog();
            this.Activate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form_3_ConfigurarLayers newLayerConfiguration = new Form_3_ConfigurarLayers(this.arranjos);
            newLayerConfiguration.ShowDialog();
        }

        private void bUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (dGVLayer.CurrentRow.Index > 0)
                {
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Selected = false;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Selected = true;

                    string t1 = dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Cells[0].Value.ToString();
                    string t2 = dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Cells[1].Value.ToString();
                    string t3 = dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Cells[2].Value.ToString();

                    dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Cells[0].Value = dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[0].Value;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Cells[1].Value = dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[1].Value;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Cells[2].Value = dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[2].Value;

                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[0].Value = t1;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[1].Value = t2;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[2].Value = t3;

                    dGVLayer.CurrentCell = dGVLayer.Rows[dGVLayer.CurrentRow.Index - 1].Cells[0];
                    //tabela1BindingSource.MovePrevious(); *****xxxxx*****
                }
            }
            catch (Exception)
            {

            }
        }

        private void bDow_Click(object sender, EventArgs e)
        {
            try
            {
                if (dGVLayer.CurrentRow.Index < dGVLayer.Rows.Count - 1)
                {
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Selected = false;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Selected = true;

                    string t1 = dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Cells[0].Value.ToString();
                    string t2 = dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Cells[1].Value.ToString();
                    string t3 = dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Cells[2].Value.ToString();

                    dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Cells[0].Value = dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[0].Value;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Cells[1].Value = dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[1].Value;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Cells[2].Value = dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[2].Value;

                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[0].Value = t1;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[1].Value = t2;
                    dGVLayer.Rows[dGVLayer.CurrentRow.Index].Cells[2].Value = t3;

                    dGVLayer.CurrentCell = dGVLayer.Rows[dGVLayer.CurrentRow.Index + 1].Cells[0];
                    //tabela1BindingSource.MoveNext(); *****xxxxx*****
                }
            }
            catch (Exception)
            {

            }
        }

        private void tCPrincipal_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 1)
            {
                configuration.PROGRAMDbLin = UserSettingsService.EnsureAndReadProgramDbLin(configuration.PROGRAMDbLin);

                if (!File.Exists(configuration.PROGRAMDbLin))
                {

                    Form_1_CaminhoLin caminhoLin = new Form_1_CaminhoLin(configuration.PROGRAMDbLin);
                    caminhoLin.ShowDialog();

                    if (!File.Exists(caminhoLin.file))
                        tCPrincipal.SelectTab(0);
                    else
                    {
                        configuration.PROGRAMDbLin = caminhoLin.file;
                        caminhoLin.Dispose();
                        UserSettingsService.SaveProgramDbLin(configuration.PROGRAMDbLin);
                    }
                }
            }
        }

        private void bNovo_Click(object sender, EventArgs e)
        {
            if (IsModify())
            {
                if (MessageBox.Show("Houve alteraÃ§Ãµes no templade." +
                          "\nDeseja realmente criar um novo templade?",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {

                    configuration = new Configuration();
                    arranjos = new Arranjos();
                    CarregarTemplateDeConfiguracaoPTela();

                    tbListaDeConversores.Text = "";
                    cBListaDeConversores.Text = "";
                    previousIndex = "";
                    listBlocks.Clear();
                    listBlocksInv.Clear();
                    listBlocksOrig.Clear();
                    listBoxBlock.Items.Clear();
                    loadDimensionTab();
                    loadScaleTab();
                    loadAtributarTab();
                }
            }
            else
            {
                configuration = new Configuration();
                arranjos = new Arranjos();
                CarregarTemplateDeConfiguracaoPTela();
                tbListaDeConversores.Text = "";
                cBListaDeConversores.Text = "";
                previousIndex = "";
                listBlocks.Clear();
                listBlocksInv.Clear();
                listBlocksOrig.Clear();
                listBoxBlock.Items.Clear();
                loadDimensionTab();
                loadScaleTab();
                loadAtributarTab();
            }
        }

        private bool IsModify()
        {


            bool isModify = false;
            this.arranjos.conversor.Clear();
            for (int i = 0; i < dGVLayer.Rows.Count; i++)
            {
                this.arranjos.conversor.Add(dGVLayer.Rows[i].Cells[0].Value.ToString() + ";" +
                                            dGVLayer.Rows[i].Cells[1].Value.ToString() + ";" +
                                            dGVLayer.Rows[i].Cells[2].Value.ToString());
            }
            CarregarTemplateDeTelaPConviguracao();

            string file1 = ConverterFileService.GetTxmlPath("TEMPORARYFile1NFJDWI00012", (StatusConversorItem)StatusConversor.Items[0]);
            string filexml = ConverterFileService.GetTxmlPath(tbListaDeConversores.Text, (StatusConversorItem)StatusConversor.Items[0]);

            configuration.SaveXML("TEMPORARYFile1NFJDWI00012", arranjos, this.listBlocks, this.listBlocksInv, this.listBlocksOrig, (StatusConversorItem)StatusConversor.Items[0]);

            if (File.Exists(filexml))
            {
                StreamReader sr1 = new StreamReader(file1, Encoding.UTF8, true);
                StreamReader sr2 = new StreamReader(filexml, Encoding.UTF8, true);
                if (sr1.ReadToEnd() != sr2.ReadToEnd())
                    isModify = true;
                else
                    isModify = false;
                sr1.Close();
                sr2.Close();
            }

            else
            {
                Configuration tempconf = new Configuration();
                Arranjos temparray = new Arranjos();
                tempconf.SaveXML("TEMPORARYFile2NFJDWI00012", temparray, this.listBlocks, this.listBlocksInv, this.listBlocksOrig, (StatusConversorItem)StatusConversor.Items[0]);
                string file3 = ConverterFileService.GetTxmlPath("TEMPORARYFile2NFJDWI00012", (StatusConversorItem)StatusConversor.Items[0]);

                StreamReader sr1 = new StreamReader(file1, Encoding.UTF8, true);
                StreamReader sr2 = new StreamReader(file3, Encoding.UTF8, true);
                if (sr1.ReadToEnd() != sr2.ReadToEnd())
                    isModify = true;
                else
                    isModify = false;
                sr1.Close();
                sr2.Close();
                File.Delete(file3);
            }
            File.Delete(file1);
            return isModify;
        }

        private void JanelaPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsModify())
            {
                if (MessageBox.Show("Houve alteraÃ§Ãµes no templade." +
                          "\nDeseja realmente sair sem salvar as alteraÃ§Ãµes?",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button2) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
            if (e.Cancel == false)
            {
                myDrawing?.Dispose();
                myDrawingBlock?.Dispose();
            }
        }


        private void CCTB_DimScale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (CCTB_DimScale.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void CCTB_DimArrowSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            } 
            else if (e.KeyChar == ',')
            {
                if (CCTB_DimArrowSize.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void CCTB_DimOffset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (CCTB_DimOffset.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void loadDimensionTab()
        {
            string textoCCCB_Layer = CCCB_Layer.Text;
            CCCB_Layer.Items.Clear();
            CCCB_Layer.Items.AddRange(arranjos.allNewLayer.ToArray());
            if (!CCCB_Layer.Items.Contains(textoCCCB_Layer))
                CCCB_Layer.Items.Add(textoCCCB_Layer);
            CCCB_Layer.Text = textoCCCB_Layer;
            string textoCCCB_ColorLine = CCCB_ColorLine.Text;
            CCCB_ColorLine.Items.Clear();
            CCCB_ColorLine.Items.AddRange(arranjos.allcolor.ToArray());
            CCCB_ColorLine.Items.RemoveAt(0);
            if (!CCCB_ColorLine.Items.Contains(textoCCCB_ColorLine))
                CCCB_ColorLine.Items.Add(textoCCCB_ColorLine);
            CCCB_ColorLine.Text = textoCCCB_ColorLine;
            string textoCCCB_ColorText = CCCB_ColorText.Text;
            CCCB_ColorText.Items.Clear();
            CCCB_ColorText.Items.AddRange(arranjos.allcolor.ToArray());
            CCCB_ColorText.Items.RemoveAt(0);
            if (!CCCB_ColorText.Items.Contains(textoCCCB_ColorText))
                CCCB_ColorText.Items.Add(textoCCCB_ColorText);
            CCCB_ColorText.Text = textoCCCB_ColorText;
            CarregarComboBoxTableConfiguracaoDimensoes_LayerASerConvertido();
            CarregarComboBoxTableConfiguracaoDRAWINGSHEET();
            CarregarComboBoxTableConfiguracaoOTHEROBJECTTYPE();

        }

        private void loadScaleTab()
        {
            string text = zoomLayerFilter.Text;
            zoomLayerFilter.Items.Clear();
            List<string> layers = new List<string>();
            layers.AddRange(arranjos.allNewLayer.ToArray());
            layers.AddRange(arranjos.allBaseLayer.ToArray());
            zoomLayerFilter.Items.AddRange(layers.Distinct().ToArray());
            if (!zoomLayerFilter.Items.Contains(text))
                zoomLayerFilter.Items.Add(text);
            zoomLayerFilter.Text = text;
        }

        private void loadAtributarTab()
        {
            comboBoxLayer.Items.Clear();
            List<string> layers = new List<string>();
            layers.AddRange(arranjos.allNewLayer.ToArray());
            layers.AddRange(arranjos.allBaseLayer.ToArray());
            comboBoxLayer.Items.AddRange(layers.Distinct().ToArray());
            try
            {
                comboBoxLayer.Text = arranjos.allNewLayer.First();
            }catch (Exception){}
        }

        private void TP_Conf_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPageIndex == 2)
            {
                loadDimensionTab();
            }
            else if (e.TabPageIndex == 3)
            {
                loadScaleTab();
            }
            else if (e.TabPageIndex == 4)
            {
                loadAtributarTab();
            }
        }

        private void CCCB_Layer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void CCCB_ColorLine_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void CCCB_ColorText_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void CCTB_TypeArrow_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void CCTB_Fonts_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }



        private void TB_Ltscale_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (TB_Ltscale.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void CCTB_LineExt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (CCTB_LineExt.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void rBScaleManual_CheckedChanged(object sender, EventArgs e)
        {
            if (rBScaleManual.Checked)
            {
                gBScaleManual.Enabled = true;
                gBScaleAuto.Enabled = false;
            }
        }

        private void rBScaleAuto_CheckedChanged(object sender, EventArgs e)
        {
            if(rBScaleAuto.Checked)
            {
                gBScaleManual.Enabled = false;
                gBScaleAuto.Enabled = true;
            }
        }

        private void p1x_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (p1x.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void p1y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (p1y.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void p1z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (p1z.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void p2x_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (p2x.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void p2y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (p2y.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void p2z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (p2z.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void ap1x_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ap1x.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void ap1y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ap1y.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void ap1z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ap1z.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void ap2x_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ap2x.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void ap2y_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ap2y.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void ap2z_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (ap2z.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private void zoomLayerFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void zoomTextSize_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != (char)8 && e.KeyChar != ',')
            {
                e.Handled = true;
            }
            else if (e.KeyChar == ',')
            {
                if (zoomTextSize.Text.Contains(','))
                    e.Handled = true;
            }
        }

        private bool IsContinueOp1 = true;

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private void bSelect_Click(object sender, EventArgs e)
        { 
            CarregarMyDrawing();
            if (IsContinueOp1)
            {  
                if (myDrawing != null)
                {
                    PointEspecial p1 = new PointEspecial();
                    PointEspecial p2 = new PointEspecial();
                    myDrawing.Get2Point(ref p1, ref p2);

                    ap1x.Text = Convert.ToString(p1.X);
                    ap1y.Text = Convert.ToString(p1.Y);
                    ap1z.Text = Convert.ToString(p1.Z);

                    ap2x.Text = Convert.ToString(p2.X);
                    ap2y.Text = Convert.ToString(p2.Y);
                    ap2z.Text = Convert.ToString(p2.Z);

                    p1x.Text = Convert.ToString(p1.X);
                    p1y.Text = Convert.ToString(p1.Y);
                    p1z.Text = Convert.ToString(p1.Z);

                    p2x.Text = Convert.ToString(p2.X);
                    p2y.Text = Convert.ToString(p2.Y);
                    p2z.Text = Convert.ToString(p2.Z);
                }
                System.Threading.Thread.Sleep(5);
                SetForegroundWindow(this.Handle);
                this.Activate();
            }

        }

        private void bSelect2_Click(object sender, EventArgs e)
        {
            CarregarMyDrawing();
            if (IsContinueOp1)
            {
                if (myDrawing != null)
                {
                    PointEspecial p1 = new PointEspecial();
                    PointEspecial p2 = new PointEspecial();
                    myDrawing.Get2Point(ref p1, ref p2);

                    ap1x.Text = Convert.ToString(p1.X);
                    ap1y.Text = Convert.ToString(p1.Y);
                    ap1z.Text = Convert.ToString(p1.Z);

                    ap2x.Text = Convert.ToString(p2.X);
                    ap2y.Text = Convert.ToString(p2.Y);
                    ap2z.Text = Convert.ToString(p2.Z);

                    p1x.Text = Convert.ToString(p1.X);
                    p1y.Text = Convert.ToString(p1.Y);
                    p1z.Text = Convert.ToString(p1.Z);

                    p2x.Text = Convert.ToString(p2.X);
                    p2y.Text = Convert.ToString(p2.Y);
                    p2z.Text = Convert.ToString(p2.Z);
                }
                System.Threading.Thread.Sleep(5);
                SetForegroundWindow(this.Handle);
                this.Activate();
            }
        }

        private void CarregarMyDrawing()
        {
            if (myDrawing != null)
                myDrawing.UpdateStatus();
            if (myDrawing == null || myDrawing.Status() == "ERROR")
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAbrindoCad));
                        newThread2.SetApartmentState(ApartmentState.STA);
                        newThread2.Start();
                        myDrawing = new GetInfo(openFileDialog.FileName);
                        StopStatusThread(newThread2);
                        IsContinueOp1 = true;
                    }
                    openFileDialog.Dispose();
                }
                else
                    IsContinueOp1 = false;
            }
            else
                IsContinueOp1 = true;
        }

        private void CarregarMyDrawingBlock()
        {
            if (myDrawingBlock != null)
                myDrawingBlock.UpdateStatus();
            if (myDrawingBlock == null || myDrawingBlock.Status() == "ERROR")
            {
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAbrindoCad));
                        newThread2.SetApartmentState(ApartmentState.STA);
                        newThread2.Start();
                        tBDiretorioFormatoAtributado.Text = openFileDialog.FileName;
                        myDrawingBlock = new GetInfo(openFileDialog.FileName);
                        StopStatusThread(newThread2);
                        IsContinueOp1 = true;
                    }
                    openFileDialog.Dispose();
                }
                else
                    IsContinueOp1 = false;
            }
            else
            {
                myDrawingBlock.CloseDrawing();
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(openFileDialog.FileName))
                    {
                        Thread newThread2 = new Thread(new ThreadStart(Form_0_JanelaPrincipal.ThreadMethodAbrindoCad));
                        newThread2.SetApartmentState(ApartmentState.STA);
                        newThread2.Start();
                        tBDiretorioFormatoAtributado.Text = openFileDialog.FileName;
                        myDrawingBlock = new GetInfo(openFileDialog.FileName);
                        StopStatusThread(newThread2);
                        IsContinueOp1 = true;
                    }
                    openFileDialog.Dispose();
                }
                else
                    IsContinueOp1 = false;
            }
        }

        private void bPesquisar3_Click(object sender, EventArgs e)
        {
            if (listBlocks.Count <= 0)
            {
                CarregarBlocos();
                this.SetTopLevel(true);
                this.BringToFront();
                this.Activate();
            }
            else
            {
                string status = Form_5_ChangeFormat.Show("Deseja atualizar os blocos?\nEssa operaÃ§Ã£o anularÃ¡ qualquer modificaÃ§Ã£o feita sobre eles.",
                                                  "AtenÃ§Ã£o");
                if (status == "2")
                {
                    CarregarBlocos();
                    this.SetTopLevel(true);
                    this.BringToFront();
                    this.Activate();
                }
                else if (status == "1")
                {
                    openFileDialog.ShowDialog();
                    if (File.Exists(openFileDialog.FileName))
                    {
                        tBDiretorioFormatoAtributado.Text = openFileDialog.FileName;
                    }
                    openFileDialog.Dispose();
                }
            }
        }

        private void CarregarBlocos()
        {
            try
            {
                CarregarMyDrawingBlock();
                if (myDrawingBlock != null)
                {
                    listBlocks = myDrawingBlock.GetListBlocks();
                }
                listBlocks = listBlocks.Distinct(new BlockClassComparer((x, y) => x.blockName == y.blockName, f => f.blockName.GetHashCode())).ToList();
                listBoxBlock.Items.Clear();
                foreach (Block item in listBlocks)
                {
                    listBoxBlock.Items.Add(item.blockName);
                }
            }
            catch (Exception)
            {
            }
        }

        private void comboBoxLayer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void bAdd_Click(object sender, EventArgs e)
        {
            Filter filtro = new Filter(this.arranjos);
            filtro.SetConjunto2();




            foreach (var item in comboBoxLayer.SelectedItems)
            {
                //if (!EXPL_ListaExplodeLayers.Items.Contains(item))
                if (dataGridViewLayer.Rows
            .Cast<DataGridViewRow>()
            .Where(r => r.Cells["layerDataGridViewTextBoxColumn"].Value.ToString().Equals(item)).Count() == 0)
                    dataGridViewLayer.Rows.Add(item, filtro.GetConjunto());
            }
        }

        private void bRemove_Click(object sender, EventArgs e)
        {
            if (dataGridViewLayer.SelectedRows.Count > 0 && dataGridViewLayer.CurrentRow.Index != -1)
            {
                for (int i = dataGridViewLayer.SelectedRows.Count - 1; i > -1; i--)
                {
                    dataGridViewLayer.Rows.RemoveAt(dataGridViewLayer.SelectedRows[i].Index);
                }

            }
        }

        private void bModify_Click(object sender, EventArgs e)
        {
            if (listBoxBlock.SelectedIndex != -1)
            {
                this.SetTopLevel(false);
                bloEdit();
                this.SetTopLevel(true);
            }
        }

        private void listBoxBlock_DoubleClick(object sender, EventArgs e)
        {
            this.SetTopLevel(false);
            bloEdit();
            this.SetTopLevel(true);
        }

        private void bloEdit()
        {
            try
            {
                Form_5_AttFormat bC = new Form_5_AttFormat(listBlocks[listBoxBlock.SelectedIndex], arranjos, myDrawingBlock);
                bC.ShowDialog();
                myDrawingBlock = bC.myDrawingBlock;
                bC.Dispose();
            }
            catch (Exception)
            {

            }
        }

        private void dataGridViewLayer_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1)
            {
                Form_4_LayersFilter myFilter = new Form_4_LayersFilter(dataGridViewLayer.Rows[dataGridViewLayer.CurrentRow.Index].Cells[1].Value.ToString(), arranjos);
                myFilter.CarregarControlFilterCBLinhaTipo2(dataGridViewLayer.Rows[dataGridViewLayer.CurrentRow.Index].Cells[1].Value.ToString());
                myFilter.DisableOrientacao();
                myFilter.ShowDialog();
                dataGridViewLayer.Rows[e.RowIndex].Cells[1].Value = myFilter.filtro.GetConjunto();
                myFilter.Dispose();
            }
        }

        private void bRemoveBlocks_Click(object sender, EventArgs e)
        {
            try
            {
                listBlocks.RemoveAt(listBoxBlock.SelectedIndex);
                listBoxBlock.Items.RemoveAt(listBoxBlock.SelectedIndex);

            }
            catch (Exception)
            {

            }
        }

        private void lBLISP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lBLISP.SelectedIndex != -1)
            {
                string[] value = lBLISP.Items[lBLISP.SelectedIndex].ToString().Split('@');
                tBComandoLISP.Text = value[0];
                tBDiretorio1.Text = value[1];
                if (value.Count() == 3)
                    ExcultarFinalConversao.Checked = true;
                else
                    ExcultarFinalConversao.Checked = false;

            }
        }

        private void bPesquisar1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog oFD = new System.Windows.Forms.OpenFileDialog();
            oFD.Filter = "Autocad Apps File|*.arx;*.lsp;*.dvb;*.dbx;*.vlx;*.fas;*.dll";
            oFD.Multiselect = false;
            if (oFD.ShowDialog() == DialogResult.OK)
            {
                tBDiretorio1.Text = oFD.FileName;
            }
        }

        private void bExcluir1_Click(object sender, EventArgs e)
        {
            if (lBLISP.SelectedIndex != -1)
            {
                lBLISP.Items.RemoveAt(lBLISP.SelectedIndex);
            }
        }

        private void bUp1_Click(object sender, EventArgs e)
        {
            if (lBLISP.SelectedIndex > 0)
            {
                string t = lBLISP.Items[lBLISP.SelectedIndex].ToString();
                lBLISP.Items[lBLISP.SelectedIndex] = lBLISP.Items[lBLISP.SelectedIndex - 1];
                lBLISP.Items[lBLISP.SelectedIndex - 1] = t;
                lBLISP.SetSelected(lBLISP.SelectedIndex - 1, true);
            }
        }

        private void bDonw1_Click(object sender, EventArgs e)
        {
            if (lBLISP.SelectedIndex != -1 && lBLISP.SelectedIndex < lBLISP.Items.Count - 1)
            {
                string t = lBLISP.Items[lBLISP.SelectedIndex].ToString();
                lBLISP.Items[lBLISP.SelectedIndex] = lBLISP.Items[lBLISP.SelectedIndex + 1];
                lBLISP.Items[lBLISP.SelectedIndex + 1] = t;
                lBLISP.SetSelected(lBLISP.SelectedIndex + 1, true);
            }
        }

        private void lBDLL_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lBDLL.SelectedIndex != -1)
            {
                string[] value = lBDLL.Items[lBDLL.SelectedIndex].ToString().Split('@');
                tBComandoDLL.Text = value[0];
                tBDiretorio2.Text = value[1];
            }
        }

        private void bPesquisar2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.OpenFileDialog oFD = new System.Windows.Forms.OpenFileDialog();
            oFD.Filter = "DLL (*.dll)|*.dll";
            oFD.Multiselect = false;
            if (oFD.ShowDialog() == DialogResult.OK)
            {
                tBDiretorio2.Text = oFD.FileName;
            }
        }

        private void bSalvar2_Click(object sender, EventArgs e)
        {
            if (File.Exists(tBDiretorio1.Text) || String.IsNullOrWhiteSpace(tBDiretorio1.Text))
            {
                if (!String.IsNullOrWhiteSpace(tBComandoDLL.Text))
                {
                    bool adicionar = true;
                    for (int i = 0; i < lBDLL.Items.Count; i++)
                    {
                        if (tBComandoDLL.Text == lBDLL.Items[i].ToString().Split('@')[0])
                        {
                            lBDLL.Items[i] = tBComandoDLL.Text + "@" + tBDiretorio2.Text;
                            adicionar = false;
                            break;
                        }
                    }
                    if (adicionar)
                    {
                        lBDLL.Items.Add(tBComandoDLL.Text + "@" + tBDiretorio2.Text);
                    }
                }
                else
                {
                    MessageBox.Show("Comando invÃ¡lido",
                              "AtenÃ§Ã£o",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button1);
                }
            }

            else
            {
                MessageBox.Show("Arquivo invÃ¡lido",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1);
            }
        }

        private void bExcluir2_Click(object sender, EventArgs e)
        {
            if (lBDLL.SelectedIndex != -1)
            {
                lBDLL.Items.RemoveAt(lBDLL.SelectedIndex);
            }
        }

        private void bUp2_Click(object sender, EventArgs e)
        {
            if (lBDLL.SelectedIndex > 0)
            {
                string t = lBDLL.Items[lBDLL.SelectedIndex].ToString();
                lBDLL.Items[lBDLL.SelectedIndex] = lBDLL.Items[lBDLL.SelectedIndex - 1];
                lBDLL.Items[lBDLL.SelectedIndex - 1] = t;
                lBDLL.SetSelected(lBDLL.SelectedIndex - 1, true);
            }
        }

        private void bDonw2_Click(object sender, EventArgs e)
        {
            if (lBDLL.SelectedIndex != -1 && lBDLL.SelectedIndex < lBDLL.Items.Count - 1)
            {
                string t = lBDLL.Items[lBDLL.SelectedIndex].ToString();
                lBDLL.Items[lBDLL.SelectedIndex] = lBDLL.Items[lBDLL.SelectedIndex + 1];
                lBDLL.Items[lBDLL.SelectedIndex + 1] = t;
                lBDLL.SetSelected(lBDLL.SelectedIndex + 1, true);
            }
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void rTBComentarios_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void bImportar_Click(object sender, EventArgs e)
        {
            if (IsModify())
            {
                if (MessageBox.Show("Houve alteraÃ§Ãµes no templade." +
                          "\nDeseja realmente criar um novo templade?",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    Importar();
                }
            }
            else
            {
                Importar();
            }
            
        }

        private void Importar()
        {
            if (openFileDialogCC.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                configuration = new Configuration();
                arranjos = new Arranjos();

                StreamReader sr = new StreamReader(openFileDialogCC.FileName);
                string ponteiro = "";
                while (!sr.EndOfStream)
                {
                    string atual = sr.ReadLine();
                    if (atual == "")
                    {
                        ponteiro = "";
                        continue;
                    }
                    else if (atual == "-=[DESCRICAO]=-")
                    {
                        ponteiro = "-=[DESCRICAO]=-";
                        continue;
                    }
                    else if (atual == "-=[LAYERS]=-")
                    {
                        ponteiro = "-=[LAYERS]=-";
                        continue;
                    }
                    else if (atual == "-=[COTAS]=-")
                    {
                        ponteiro = "-=[COTAS]=-";
                        continue;
                    }
                    else if (atual == "-=[DONUTS]=-")
                    {
                        ponteiro = "-=[DONUTS]=-";
                        continue;
                    }
                    else if (atual == "-=[MAPAS]=-")
                    {
                        ponteiro = "-=[MAPAS]=-";
                        continue;
                    }
                    else if (atual == "-=[FIM]=-")
                    {
                        ponteiro = "-=[FIM]=-";
                        continue;
                    }

                    if (ponteiro == "-=[DESCRICAO]=-")
                    {
                        configuration.EXTCONFComments += atual + "\n";
                    }

                    else if (ponteiro == "-=[LAYERS]=-")
                    {
                        string[] t1 = atual.Split('=');
                        string[] t2 = t1.Last().Split(';');
                        string s1 = t1.First().TrimEnd();
                        string s2 = t2.First().TrimEnd();
                        string s3 = t2.Last().TrimEnd();
                        arranjos.allNewLayerComposition.Add(s1.TrimStart() + ":" +
                                                            s2.TrimStart() + ":" +
                                                            s3.TrimStart());
                        arranjos.allNewLayer.Remove(s1.TrimStart());
                        arranjos.allNewLayer.Add(s1.TrimStart());
                    }

                    else if (ponteiro == "-=[COTAS]=-")
                    {
                        string[] t1 = atual.Split('=');
                        if (t1.First() == "LAYER")
                            configuration.EXTDIMlayer = t1.Last();
                        else if (t1.First() == "COR LINHA")
                            configuration.EXTDIMColorLine = t1.Last();
                        else if (t1.First() == "COR TEXTO")
                            configuration.EXTDIMColorText = t1.Last();
                    }

                    else if (ponteiro == "-=[MAPAS]=-")
                    {
                        string[] t1 = atual.Split(';');
                        string[] t2 = t1.Last().Split('=');

                        string baseLayer = t1.First().TrimStart().TrimEnd();

                        string[] t3 = t2.First().TrimStart().TrimEnd().Split(':');
                        string filtroTipo = "ALL";
                        if (t3[0] == "X")
                            filtroTipo = "ALL";
                        else if (t3[0] == "T")
                            filtroTipo = "TEXT";
                        else if (t3[0] == "L")
                            filtroTipo = "LINE";
                        else if (t3[0] == "A")
                            filtroTipo = "ARC";
                        else if (t3[0] == "C")
                            filtroTipo = "CIRCLE";
                        else if (t3[0] == "S")
                            filtroTipo = "HATCH";
                        else if (t3[0] == "P")
                            filtroTipo = "LWPOLYLINE";
                        else if (t3[0] == "D")
                            filtroTipo = "DIMENSION";

                        string filtroCor = "ALL";
                        string filtroTipoLinha = "ALL";
                        string filtroConteudo = "";
                        string filtroAltura = "";
                        string filtroOrientacaoLinha = "ALL";
                        for (int i = 1; i < t3.Count(); i++)
                        {
                            if (t3[i].Substring(0, 1) == "C")
                                filtroCor = t3[i].Substring(1, t3[i].Length - 1);
                            else if (t3[i].Substring(0, 1) == "L")
                                filtroTipoLinha = t3[i].Substring(1, t3[i].Length - 1);
                            else if (t3[i].Substring(0, 1) == "S")
                                filtroConteudo = t3[i].Substring(1, t3[i].Length - 1);
                            else if (t3[i].Substring(0, 1) == "H")
                                filtroAltura = t3[i].Substring(1, t3[i].Length - 1).Replace('.', ',');
                        }

                        string[] t4 = t2.Last().Split(':');
                        string newLayer = t4[0].TrimEnd().TrimStart().TrimEnd();
                        string newCor = t4[1].TrimEnd();
                        if (newCor == "NIL")
                            newCor = "BYLAYER";
                        string newLineType = t4[2].TrimEnd();
                        if (newLineType == "NIL")
                            newLineType = "BYLAYER";
                        string newAltura = t4[3].TrimEnd().Replace('.', ',');
                        try
                        {
                            Convert.ToDouble(newAltura);
                        }
                        catch (Exception)
                        {
                            newAltura = "";
                        }
                        string newLargura = t4[4].TrimEnd().Replace('.', ',');
                        try
                        {
                            Convert.ToDouble(newLargura);
                        }
                        catch (Exception)
                        {
                            newLargura = "";
                        }
                        arranjos.conversor.Add(baseLayer + ";" +
                                               filtroTipo + ":" +
                                               filtroCor + ":" +
                                               filtroTipoLinha + ":" +
                                               filtroConteudo + ":" +
                                               filtroAltura + ":" +
                                               filtroOrientacaoLinha + ";" +
                                               newLayer + ":" +
                                               newCor + ":" +
                                               newLineType + ":" +
                                               newAltura + ":" +
                                               newLargura);

                    }
                }
                listBlocks.Clear();
                listBoxBlock.Items.Clear();
                CarregarTemplateDeConfiguracaoPTela();
                loadDimensionTab();
                loadScaleTab();
                loadAtributarTab();
                string name = openFileDialogCC.SafeFileName;
                name = name.Remove(name.Length - 5);
                tbListaDeConversores.Text = name;
                previousIndex = name;

                MessageBox.Show("ImportaÃ§Ã£o realizada com sucesso!",
                           "InformaÃ§Ã£o",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information,
                           MessageBoxDefaultButton.Button1);
            }
        }
        private void CarregarComboBoxTableConfiguracaoDimensoes_LayerASerConvertido()
        {
            string texto = CCTB_LayerBase.Text;
            CCTB_LayerBase.Items.Clear();
            List<string> lt = arranjos.allBaseLayer.GetRange(0, arranjos.allBaseLayer.Count);
            lt.Remove("DIMENSION");
            CCTB_LayerBase.Items.Add("DIMENSION");
            foreach (string item in lt)
            {
                CCTB_LayerBase.Items.Add(item);
            }
            if (!CCTB_LayerBase.Items.Contains(texto))
                CCTB_LayerBase.Items.Add(texto);
            CCTB_LayerBase.Text = texto;
   
       
        }

        private void CarregarComboBoxTableConfiguracaoDRAWINGSHEET()
        {
            string texto = LayerTextoTekla.Text;
            LayerTextoTekla.Items.Clear();
            List<string> lt = arranjos.allBaseLayer.GetRange(0, arranjos.allBaseLayer.Count);
            lt.Remove("DRAWING SHEET");
            LayerTextoTekla.Items.Add("DRAWING SHEET");
            foreach (string item in lt)
            {
                LayerTextoTekla.Items.Add(item);
            }
            if (lt.Contains(texto))
                LayerTextoTekla.Text = texto;
            else
                LayerTextoTekla.Text = "DRAWING SHEET";
        }

        private void CarregarComboBoxTableConfiguracaoOTHEROBJECTTYPE()
        {
            string texto = LayerBlocosAtt.Text;
            LayerBlocosAtt.Items.Clear();
            List<string> lt = arranjos.allBaseLayer.GetRange(0, arranjos.allBaseLayer.Count);
            lt.Remove("OTHER OBJECT TYPE");
            LayerBlocosAtt.Items.Add("OTHER OBJECT TYPE");
            foreach (string item in lt)
            {
                LayerBlocosAtt.Items.Add(item);
            }
            if (lt.Contains(texto))
                LayerBlocosAtt.Text = texto;
            else
                LayerBlocosAtt.Text = "OTHER OBJECT TYPE";
        }

        private void CCTB_LayerBase_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void CCB_OtherColorLine_Click(object sender, EventArgs e)
        {
            string color = string.Empty;
            Form_8_GenericNewColor newColor = new Form_8_GenericNewColor(CCCB_ColorLine.Text);
            newColor.ShowDialog();
            if (!arranjos.allcolor.Contains(newColor.colorClass))
            {
                arranjos.allcolor.Add(newColor.colorClass);
                CCCB_ColorLine.Items.Add(newColor.colorClass);
                CCCB_ColorText.Items.Add(newColor.colorClass);
            }
            CCCB_ColorLine.Text = newColor.colorClass;
            newColor.Dispose();
        }

        private void CCB_OtherColorText_Click(object sender, EventArgs e)
        {
            string color = string.Empty;
            Form_8_GenericNewColor newColor = new Form_8_GenericNewColor(CCCB_ColorText.Text);
            newColor.ShowDialog();

            if (!arranjos.allcolor.Contains(newColor.colorClass))
            {
                arranjos.allcolor.Add(newColor.colorClass);
                CCCB_ColorLine.Items.Add(newColor.colorClass);
                CCCB_ColorText.Items.Add(newColor.colorClass);
            }

            CCCB_ColorText.Items.Add(newColor.colorClass);

            newColor.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form_7_ConfAvaACADaDeCota confCota = new Form_7_ConfAvaACADaDeCota(EXTDIMCorrigeSeta, EXTDIMCorrigeSetaTipoSeta, EXTDIMCorrigeSetaFactor);
            confCota.ShowDialog();
            EXTDIMCorrigeSeta = confCota.EXTDIMCorrigeSeta;
            EXTDIMCorrigeSetaTipoSeta = confCota.EXTDIMCorrigeSetaTipoSeta;
            EXTDIMCorrigeSetaFactor = confCota.EXTDIMCorrigeSetaFactor;
        }

        private void cBConverterCotas_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Enabled = cBConverterCotas.Checked;
            CCGB_Advanced.Enabled = cBConverterCotas.Checked;
            groupBox6.Enabled = cBConverterCotas.Checked;
        }

        private void cBConverterLayer_CheckedChanged(object sender, EventArgs e)
        {
            tPConvLayer.Enabled = cBConverterLayer.Checked;
            groupBox1.Enabled = cBConverterLayer.Checked;
        }

        private void cBEscalaDesenho_CheckedChanged(object sender, EventArgs e)
        {
            tabPage3.Enabled = cBEscalaDesenho.Checked;
        }

        private void cBExLISP_CheckedChanged(object sender, EventArgs e)
        {
            tabPage1.Enabled = cBExLISP.Checked;
        }


        private void AbilitarDesabilitarInicial()
        {
            groupBox2.Enabled = cBConverterCotas.Checked;
            CCGB_Advanced.Enabled = cBConverterCotas.Checked;
            groupBox6.Enabled = cBConverterCotas.Checked;
            tPConvLayer.Enabled = cBConverterLayer.Checked;

            tabPage3.Enabled = cBEscalaDesenho.Checked;
            tabPage1.Enabled = cBExLISP.Checked;

            groupBox1.Enabled = cBConverterLayer.Checked;
            groupBox4.Enabled = cBAtributarFormato.Checked;
            bPesquisar3.Enabled = cBAtributarFormato.Checked;
            panel1.Enabled = cBAtributarFormato.Checked;
            if (radioButton1.Checked)
                tabControl1.SelectTab(0);
            else
                tabControl1.SelectTab(1);
        }

        private void cBAtributarFormato_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                groupBox4.Enabled = cBAtributarFormato.Checked;
                bPesquisar3.Enabled = cBAtributarFormato.Checked;
                panel1.Enabled = false;
            }
            else
            {
                groupBox4.Enabled = false;
                bPesquisar3.Enabled = false;
                panel1.Enabled = cBAtributarFormato.Checked;
            }

        }
   
        private void lBConversores_SelectedValueChanged(object sender, EventArgs e)
        {
            CarregarComentarioNoTemplate();
        
            Configuration conf = new Configuration();

            Arranjos arr = new Arranjos();
            ConverterFileService.LoadConverter(conf, lBConversores.Text, arr, new List<Block>(), new List<Block>(), new List<Block>(), (StatusConversorItem)StatusConversor.SelectedItem);

           
        }




        private void lBConversores_KeyPress(object sender, KeyPressEventArgs e)
        {
            string caracter = Convert.ToString(e.KeyChar).ToUpper();
            try
            {
                for (int i = 0; i < lBConversores.Items.Count; i++)
                {
                    if (lBConversores.Items[i].ToString().Substring(0, 1) == caracter)
                    {
                        lBConversores.SetSelected(i, false);
                        break;
                    }
                }  
            }
            catch (Exception)
            {
            }         
        }

        private void JanelaPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            ConversorDrawind.Properties.Settings.Default.Extensao = extensao.Text;
            ConversorDrawind.Properties.Settings.Default.ConversorAtual = lBConversores.SelectedItem?.ToString();
            ConversorDrawind.Properties.Settings.Default.StatusConversor = StatusConversor.SelectedIndex;
            ConversorDrawind.Properties.Settings.Default.OpeACAD = cBManterArquivosAbertos.Checked;
            ConversorDrawind.Properties.Settings.Default.ConversorAtualConfig = cBListaDeConversores.Text;
            ConversorDrawind.Properties.Settings.Default.Save();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            typeconversion();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            typeconversion();
        }

        private void typeconversion()
        {
            if (radioButton1.Checked)
            {
                tabControl1.SelectTab(0);
                cBExplodir.Enabled = false;
                cBConverterCotas.Enabled = true;
                cBEscalaDesenho.Enabled = true;
                cBDeleteTekla.Enabled = true;
                groupBox4.Enabled = cBAtributarFormato.Checked;
                bPesquisar3.Enabled = cBAtributarFormato.Checked;
                panel1.Enabled = false;

                CCGB_Advanced.Enabled = true;
                groupBox6.Enabled = true;
                CCTB_DimStyle.Enabled = true;
                CCTB_TypeArrow.Enabled = true;
                button2.Enabled = true;
                groupBox5.Enabled = false;
                ExplodirBlocos.Enabled = true;
            }
            else
            {
                tabControl1.SelectTab(1);
                cBExplodir.Enabled = true;
                cBConverterCotas.Enabled = false;
                cBEscalaDesenho.Enabled = true;
                cBDeleteTekla.Enabled = false;
                groupBox4.Enabled = false;
                bPesquisar3.Enabled = false;
                panel1.Enabled = cBAtributarFormato.Checked;
                ExplodirBlocos.Enabled = false;


                CCTB_DimStyle.Enabled = false;
                CCTB_TypeArrow.Enabled = false;
                button2.Enabled = false;
                CCGB_Advanced.Enabled = false;
                groupBox6.Enabled = false;
                groupBox5.Enabled = true;
            }
        }
        private void LoadExplodeLayers()
        
        {
            EXPL_ListaAllLayers.Items.Clear();
            EXPL_ListaAllLayers.Items.AddRange(arranjos.allBaseLayer.ToArray().Distinct().ToArray());
            try
            {
                EXPL_ListaAllLayers.Text = arranjos.allBaseLayer.ToArray().Distinct().ToArray().First();
            }
            catch (Exception)
            {

            }
   
        }

        private void EXPL_AddExplodeLayer_Click(object sender, EventArgs e)
        {

            foreach (var item in EXPL_ListaAllLayers.SelectedItems)
            {
                if (!EXPL_ListaExplodeLayers.Items.Contains(item))

                    EXPL_ListaExplodeLayers.Items.Add(item);
            }
             
            
        }

        private void EXPL_RemoveExplodeLayer_Click(object sender, EventArgs e)
        {
            List<string> temp = new List<string>();
            foreach (var item in EXPL_ListaExplodeLayers.SelectedItems)
            {
                temp.Add(item.ToString());
            }
            foreach (var item in temp)
            {
                EXPL_ListaExplodeLayers.Items.Remove(item);
            }
           
          
        }

        private void BNEW_BlocosInventor_Click(object sender, EventArgs e)
        {
            CarregarBlocos2();
            this.SetTopLevel(true);
            this.BringToFront();
            this.Activate();
            LBNEW_Relacoes.Items.Clear();
            foreach (Block item in listBlocksOrig)
            {
                item.cor = Color.Black;
                item.blockNameRelacao = "";
                item.ResetTagReference();
            }
            if (LBNEW_BlocosOriginais.Items.Count > 0)
            {
                LBNEW_BlocosOriginais.SelectedIndex = 0;
            }
            if (LBNEW_BlocosInventor.Items.Count > 0)
            {
                LBNEW_BlocosInventor.SelectedIndex =0;
            }
            listRInventor = new List<string>();
          listROriginais = new List<string>();
        }

        private void BNEW_BlocosOriginais_Click(object sender, EventArgs e)
        {
            CarregarBlocos3();
            this.SetTopLevel(true);
            this.BringToFront();
            this.Activate();
            LBNEW_Relacoes.Items.Clear();
            foreach (Block item in listBlocksInv)
            {
                item.cor = Color.Black;
                item.blockNameRelacao = "";
                item.ResetTagReference();
            }
            if (LBNEW_BlocosOriginais.Items.Count > 0)
            {
                LBNEW_BlocosOriginais.SelectedIndex = 0;
            }
            if (LBNEW_BlocosInventor.Items.Count > 0)
            {
                LBNEW_BlocosInventor.SelectedIndex = 0;
            }
            listRInventor = new List<string>();
            listROriginais = new List<string>();
        }

        private void CarregarBlocos2()
        {
            try
            {
                CarregarMyDrawingBlock();
                if (myDrawingBlock != null)
                {
                    listBlocksInv = myDrawingBlock.GetListBlocks();
                }
                listBlocksInv = listBlocksInv.Distinct(new BlockClassComparer((x, y) => x.blockName == y.blockName, f => f.blockName.GetHashCode())).ToList();
                LBNEW_BlocosInventor.Items.Clear();
                foreach (Block item in listBlocksInv)
                {
                    LBNEW_BlocosInventor.Items.Add(item.blockName);
                }
                using (MessageFilter.ScopedRegistration())
                {
                    myDrawingBlock.Dispose();
                }
            }
            catch (Exception)
            {
            }

        }

        private void CarregarBlocos3()
        {
            try
            {
                CarregarMyDrawingBlock();
                if (myDrawingBlock != null)
                {
                    listBlocksOrig = myDrawingBlock.GetListBlocks();
                }
                listBlocksOrig = listBlocksOrig.Distinct(new BlockClassComparer((x, y) => x.blockName == y.blockName, f => f.blockName.GetHashCode())).ToList();
                LBNEW_BlocosOriginais.Items.Clear();
                foreach (Block item in listBlocksOrig)
                {
                    LBNEW_BlocosOriginais.Items.Add(item.blockName);
                }
                TBNEW_BlocosOriginais.Text = openFileDialog.FileName;
                using (MessageFilter.ScopedRegistration())
                {
                    myDrawingBlock.Dispose();
                }
            }
            catch (Exception)
            {
            }
        }
        List<string> listRInventor = new List<string>();
        List<string> listROriginais = new List<string>();

        private void BNEW_Relacionar_Click(object sender, EventArgs e)
        {
            if (LBNEW_BlocosOriginais.SelectedIndex != -1 && LBNEW_BlocosInventor.SelectedIndex != -1)
            {
                LBNEW_Relacoes.Items.Add(LBNEW_BlocosInventor.Items[LBNEW_BlocosInventor.SelectedIndex] + "    = >    " + LBNEW_BlocosOriginais.Items[LBNEW_BlocosOriginais.SelectedIndex]);
                listROriginais.Add(LBNEW_BlocosOriginais.Items[LBNEW_BlocosOriginais.SelectedIndex].ToString());
                listRInventor.Add(LBNEW_BlocosInventor.Items[LBNEW_BlocosInventor.SelectedIndex].ToString());
                listBlocksOrig[LBNEW_BlocosOriginais.SelectedIndex].cor = Color.LightGray;
                listBlocksInv[LBNEW_BlocosInventor.SelectedIndex].cor = Color.LightGray;

                LBNEW_BlocosInventor_SelectedValueChanged(null, null);
                foreach (Block item in listBlocksOrig)
                {
                    if (item.blockName == LBNEW_BlocosOriginais.Items[LBNEW_BlocosOriginais.SelectedIndex].ToString())
                    {
                        item.blockNameRelacao = LBNEW_BlocosInventor.Items[LBNEW_BlocosInventor.SelectedIndex].ToString();
                        break;
                    }
                }
            }
        }

        private void BNEW_Remover_Click(object sender, EventArgs e)
        {
            if (LBNEW_Relacoes.SelectedIndex != -1)
            {
                string[] relacoesSplit = LBNEW_Relacoes.Items[LBNEW_Relacoes.SelectedIndex].ToString().Replace("    = >    ", ";").Split(';');
                listRInventor.Remove(relacoesSplit.First());
                listROriginais.Remove(relacoesSplit.Last());
                LBNEW_Relacoes.Items.RemoveAt(LBNEW_Relacoes.SelectedIndex);
                LBNEW_BlocosInventor_SelectedValueChanged(null, null);
                int i = 0;
                foreach (Block item in listBlocksInv)
                {
                    if (item.blockName == relacoesSplit.First())
                    {
                        item.ResetTagReference();
                        item.cor = Color.Black;
                        LBNEW_BlocosInventor.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
                 i = 0;
                foreach (Block item in listBlocksOrig)
                {
                    if (item.blockName == relacoesSplit.Last())
                    {
                        item.blockNameRelacao = "";
                        item.ResetTagReference();
                        item.cor = Color.Black;
                        LBNEW_BlocosOriginais.SelectedIndex = i;
                        break;
                    }
                    i++;
                }
            }
        }

        private void BNEW_Parametros_Click(object sender, EventArgs e)
        {
            if (LBNEW_Relacoes.SelectedIndex != -1)
            {
                string[] relacoesSplit = LBNEW_Relacoes.Items[LBNEW_Relacoes.SelectedIndex].ToString().Replace("    = >    ", ";").Split(';');
                int indOriginal = 0;
                int indInventor = 0;
                for (int i = 0; i < listBlocksInv.Count; i++)
                {
                     if (listBlocksInv[i].blockName == relacoesSplit.First())
                    {
                        indInventor = i;
                        break;
                    }
                }
                for (int i = 0; i < listBlocksOrig.Count; i++)
                {
                    if (listBlocksOrig[i].blockName == relacoesSplit.Last())
                    {
                        indOriginal = i;
                        break;
                    }
                }

                Form_6_AttFormatInventor fAttFI = new Form_6_AttFormatInventor(listBlocksInv[indInventor].DeepCopy(), listBlocksOrig[indOriginal].DeepCopy());

                fAttFI.ShowDialog();
                listBlocksInv[indInventor] = fAttFI.Inventor;
                listBlocksOrig[indOriginal] = fAttFI.Original;
                fAttFI.Dispose();
            }
        }

        private void LBNEW_BlocosInventor_SelectedValueChanged(object sender, EventArgs e)
        {
            if ((LBNEW_BlocosInventor.SelectedIndex != -1 && listRInventor.Contains(LBNEW_BlocosInventor.Items[LBNEW_BlocosInventor.SelectedIndex].ToString())) ||
                (LBNEW_BlocosOriginais.SelectedIndex != -1 && listROriginais.Contains(LBNEW_BlocosOriginais.Items[LBNEW_BlocosOriginais.SelectedIndex].ToString())))
                BNEW_Relacionar.Enabled = false;

            else
                BNEW_Relacionar.Enabled = true;
        }

        private void LBNEW_Relacoes_DoubleClick(object sender, EventArgs e)
        {
            this.SetTopLevel(false);
            BNEW_Parametros_Click(null, null);
            this.SetTopLevel(true);
        }

        private void LBNEW_BlocosInventor_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0 && e.Index < listBlocksInv.Count)
            {
                e.DrawBackground();
                string n = ((ListBox)sender).Items[e.Index].ToString();
                Font f = ((ListBox)sender).Font;
                Color c = listBlocksInv[e.Index].cor;
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, b, rect.X, rect.Top);
                e.DrawFocusRectangle();

            }
        }

        private void LBNEW_BlocosOriginais_DrawItem(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle rect = e.Bounds;
            if (e.Index >= 0 && e.Index < listBlocksOrig.Count)
            {
                e.DrawBackground();
                string n = ((ListBox)sender).Items[e.Index].ToString();
                Font f = ((ListBox)sender).Font;
                Color c = listBlocksOrig[e.Index].cor;
                Brush b = new SolidBrush(c);
                g.DrawString(n, f, b, rect.X, rect.Top);
                e.DrawFocusRectangle();

            }
        }

        private void bSalvar1_Click(object sender, EventArgs e)
        {
            if (File.Exists(tBDiretorio1.Text) || String.IsNullOrWhiteSpace(tBDiretorio1.Text))
            {
                if (!String.IsNullOrWhiteSpace(tBComandoLISP.Text))
                {
                    if (ExcultarFinalConversao.Checked)
                        lBLISP.Items.Add(tBComandoLISP.Text + "@" + tBDiretorio1.Text + "@" + "True");
                    else
                        lBLISP.Items.Add(tBComandoLISP.Text + "@" + tBDiretorio1.Text);
                    lBLISP.SelectedIndex = lBLISP.Items.Count - 1;
                }

                else
                {
                    MessageBox.Show("Comando invÃ¡lido",
                              "AtenÃ§Ã£o",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button1);
                }
            }

            else
            {
                MessageBox.Show("Arquivo invÃ¡lido",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (File.Exists(tBDiretorio1.Text) || String.IsNullOrWhiteSpace(tBDiretorio1.Text))
            {
                if (!String.IsNullOrWhiteSpace(tBComandoLISP.Text))
                {


                    if (lBLISP.SelectedIndex >= 0)
                    {
                        

                        if (ExcultarFinalConversao.Checked)
                            lBLISP.Items[lBLISP.SelectedIndex] = (tBComandoLISP.Text + "@" + tBDiretorio1.Text + "@" + "True");
                        else
                            lBLISP.Items[lBLISP.SelectedIndex] = tBComandoLISP.Text + "@" + tBDiretorio1.Text;
                    }

                }

                else
                {
                    MessageBox.Show("Comando invÃ¡lido",
                              "AtenÃ§Ã£o",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Exclamation,
                              MessageBoxDefaultButton.Button1);
                }
            }

            else
            {
                MessageBox.Show("Arquivo invÃ¡lido",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1);
            }
        }

        private void lBDesenhos_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void lBDesenhos_DragDrop(object sender, DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int i;
            for (i = 0; i < s.Length; i++)
            {
                string ex = Path.GetExtension(s[i]).ToUpper() ;
                if (ex == ".DWG" || ex == ".DXF")
                {
                    lBDesenhos.Items.Remove(s[i]);
                    lBDesenhos.Items.Add(s[i]);
                }
                
            }
              

        }



        private void ButtonRestaurar_Click(object sender, EventArgs e)
        {
            List<string> arquivos = new List<string>();
            bool retorno = true;
            if (!File.Exists(LOGarqConvertidos))
            {
                ButtonRestaurar.Enabled = false;
                return;
            }
            StreamReader sr = new StreamReader(LOGarqConvertidos);
            string temp;
            string file;
            while (!sr.EndOfStream)
            {
                temp = sr.ReadLine();
                file = temp + ".bak";
                try
                {
                    if (File.Exists(file))
                    {
                        if (File.Exists(temp))
                            File.Delete(temp);
                        File.Move(file, temp);
                    }

                }
                catch (Exception)
                {
                    arquivos.Add(temp);
                    retorno = false;
                }
            }
            sr.Close();
            File.Delete(LOGarqConvertidos);
            if (!retorno)
            {
                Form_1_Relatorio log = new Form_1_Relatorio(arquivos, "NÃ£o foi possÃ­vel restaurar todos os arquivos!");
                MessageBox.Show("NÃ£o foi possÃ­vel restaurar todos os arquivos",
                          "AtenÃ§Ã£o",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation,
                          MessageBoxDefaultButton.Button1);
            }
            ButtonRestaurar.Enabled = false;
        }

        private void openFileDialogCC_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void ConfigurarEstilosTexto_Click(object sender, EventArgs e)
        {
            Form_3_ConfigurarTextStyle newTSConfiguration = new Form_3_ConfigurarTextStyle(this.arranjos);
            newTSConfiguration.ShowDialog();
            LoadEstiloTexto();
        }

        private void StatusConversor_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarTemplates();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }

    public class Param1
    {
        public string conversorName;
        public string[] desenhosName;
        public bool closedesenhos;
        public Configuration configuration;
        public Arranjos arranjos;
        public StatusConversorItem StatusConversorItem;
    }
}



