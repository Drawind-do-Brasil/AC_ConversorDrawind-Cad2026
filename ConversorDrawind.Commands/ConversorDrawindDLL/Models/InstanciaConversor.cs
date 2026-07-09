using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawind.Commands
{
    public class InstanciaConversor
    {
        static public List<InstanciaConversor> ConversorInstancias = new List<InstanciaConversor>();
        static double GetObliqueByTextStyle(string style)
        {
            return TextStyleResolver.ResolveOblique(RuntimeConfigurationState.TextStyles, style);
        }
        static public void ConvertInstance(Entity entity)
        {
            if (string.Equals(entity.Id.ObjectClass.DxfName, "DIMENSION", StringComparison.OrdinalIgnoreCase))
                return;

            InstanciaConversor conversor =
                ConversorInstancias.Where(p => string.Equals(p.BaseLayerName, entity.Layer, StringComparison.OrdinalIgnoreCase) &&
                                     (string.Equals(p.BaseObjectType, "ALL", StringComparison.OrdinalIgnoreCase) || string.Equals(p.BaseObjectType, entity.Id.ObjectClass.DxfName, StringComparison.OrdinalIgnoreCase)) &&
                                     (p.BaseColor == null || p.BaseColor.ColorIndex == entity.Color.ColorIndex) &&
                                     (string.Equals(p.BaseLineTypeString, "ALL", StringComparison.OrdinalIgnoreCase) || string.Equals(p.BaseLineTypeString, entity.Linetype, StringComparison.OrdinalIgnoreCase)) &&
                                     (entity.GetType() != typeof(DBText) || (entity.GetType() == typeof(DBText) &&
                                                                             (String.IsNullOrEmpty(p.BaseConteudo) || p.BaseConteudo == ((DBText)entity).TextString) &&
                                                                             (p.BaseAlturaTexto == 0 || Math.Round(((DBText)entity).Height, p.BaseAlturaTextoArredondamento) == p.BaseAlturaTexto))) &&
                                     (entity.GetType() != typeof(Line) || (entity.GetType() == typeof(Line) && (p.BaseOrientacaoLinha == "ALL" || ConvertLayer.WhatIsTheOrientation(((Line)entity).StartPoint, ((Line)entity).EndPoint, p.BaseOrientacaoLinha))))).FirstOrDefault();


            if (conversor == null)
                return;

            if (string.Equals(entity.Id.ObjectClass.DxfName, "INSERT", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(conversor.BaseObjectType, "INSERT", StringComparison.OrdinalIgnoreCase))
                return;

            try
            {
                entity.UpgradeOpen();
                entity.Layer = conversor.NewLayerName;
                entity.Color = conversor.NewColor;
                entity.LinetypeId = ConvertLayer.LoadLinetype(conversor.NewLineTypeString);

                if (entity.GetType() == typeof(DBText))
                {
                    DBText text = (DBText)entity;
                    if (conversor.NewAltura > 0)
                        text.Height = conversor.NewAltura;
                    if (conversor.NewLargura > 0)
                        text.WidthFactor = conversor.NewLargura;
                    text.TextStyleId = ConvertLayer.GetTextSyleByName(conversor.TextSyle);
                    text.Oblique = GetObliqueByTextStyle(conversor.TextSyle);
                }
                entity.DowngradeOpen();
            }
            catch (Exception e)
            {
                ConversionLog.Write(LogContext.ConverterInstancia, e.Message + "| Falha ao converter o item: " + entity.Id.ObjectClass.DxfName.ToUpper() + " " + conversor.BaseLayerName + " " + conversor.BaseColorString + " " + conversor.BaseLineTypeString);
            }
        }

        string baseLayerName;
        string baseObjectType;
        Color baseColor;
        string baseColorString;
        string baseLineTypeString;
        string baseConteudo;
        double baseAlturaTexto;
        int baseAlturaTextoArredondamento;
        string baseOrientacaoLinha;
        string newLayerName;
        Color newColor;
        string newLineTypeString;
        double newAltura;
        double newLargura;
        string stringClass;
        string textSyle;

        public string BaseLayerName
        {
            get { return baseLayerName; }
        }
        public string BaseObjectType
        {
            get { return baseObjectType; }
        }
        public Color BaseColor
        {
            get { return baseColor; }
        }
        public string BaseConteudo
        {
            get { return baseConteudo; }
        }
        public double BaseAlturaTexto
        {
            get { return baseAlturaTexto; }
        }
        public int BaseAlturaTextoArredondamento
        {
            get { return baseAlturaTextoArredondamento; }
        }
        public string BaseOrientacaoLinha
        {
            get { return baseOrientacaoLinha; }
        }
        public string NewLayerName
        {
            get { return newLayerName; }
        }
        public Color NewColor
        {
            get { return newColor; }
        }
        public double NewAltura
        {
            get { return newAltura; }
        }
        public double NewLargura
        {
            get { return newLargura; }
        }
        public string BaseColorString
        {
            get { return baseColorString; }
        }
        public string NewLineTypeString
        {
            get { return newLineTypeString; }
        }
        public string BaseLineTypeString
        {
            get { return baseLineTypeString; }
        }

        public string TextSyle { get => textSyle; }

        public override string ToString()
        {
            return stringClass;
        }

        public InstanciaConversor(string line)
        {
            stringClass = line;
            string[] split1 = line.Split(';');
            string[] split2 = split1[1].Split(':');
            string[] split3 = split1[2].Split(':');
            baseLayerName = split1[0].ToUpper();
            baseObjectType = split2[0].ToUpper();
            baseColor = ConvertLayer.GetColorForName(split2[1]);
            baseColorString = split2[1];

            baseLineTypeString = split2[2].ToUpper();
            baseConteudo = split2[3];
            baseAlturaTexto = 0;
            double.TryParse(split2[4].ReplaceComma(), out baseAlturaTexto);
            baseAlturaTextoArredondamento = split2[4].ReplaceComma().Split(',').Last().Length;
            baseOrientacaoLinha = split2[5].ToUpper();
            newLayerName = split3[0];
            newColor = ConvertLayer.GetColorForName(split3[1]);
            newLineTypeString = split3[2];
            newAltura = 0;
            double.TryParse(split3[3].ReplaceComma(), out newAltura);
            newLargura = 0;
            double.TryParse(split3[4].ReplaceComma(), out newLargura);

            if (split3.Count() >= 6)
                textSyle = split3[5];
            else
                textSyle = RuntimeConfigurationState.TextStyles.First().Split(':').First();


        }
    }
}

