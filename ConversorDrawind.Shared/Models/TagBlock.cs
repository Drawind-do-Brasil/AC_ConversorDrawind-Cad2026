using System;
using System.Xml.Serialization;

namespace ConversorDrawind
{
    public class TagBlock
    {
        public string widthfactor = "1";
        public string tag;
        public bool modify = false;
        public Point p1 = new Point();
        public Point p2 = new Point();
        public Filter filtro;
        public int indiceRelacao = -1;
        public bool isSociate = false;

        [XmlIgnore]
        public bool verifiqued = false;

        [XmlIgnore]
        public string text = string.Empty;

        [XmlIgnore]
        public double WidthFactorValue
        {
            get { return string.IsNullOrWhiteSpace(widthfactor) ? 1 : NumericTextParser.ToDouble(widthfactor); }
            set { widthfactor = value.ToString(System.Globalization.CultureInfo.InvariantCulture); }
        }

        public TagBlock()
        {
            filtro = new Filter(new Configuration().Catalogs);
            filtro.SetConjunto3();
        }

        public TagBlock DeepCopy()
        {
            TagBlock other = (TagBlock)this.MemberwiseClone();
            other.p1 = new Point(this.p1);
            other.p2 = new Point(this.p2);
            other.filtro = new Filter(this.filtro);
            other.verifiqued = this.verifiqued;
            other.text = this.text;
            return other;
        }

        public void SetConjunto(string conjunto)
        {
            string[] conj = conjunto.Split('@');
            tag = conj[0];
            modify = Convert.ToBoolean(conj[1]);
            string[] pts = conj[2].Split(';');
            string[] pts1 = pts[0].Split(',');
            string[] pts2 = pts[1].Split(',');
            p1 = new Point(NumericTextParser.ToDouble(pts1[0]), NumericTextParser.ToDouble(pts1[1]), NumericTextParser.ToDouble(pts1[2]));
            p2 = new Point(NumericTextParser.ToDouble(pts2[0]), NumericTextParser.ToDouble(pts2[1]), NumericTextParser.ToDouble(pts2[2]));
            string[] subconj = conj[3].Split(';');
            filtro.layerBase = subconj[0];
            filtro.SetConjunto(subconj[1]);
            widthfactor = conj[4];
        }

        public string GetConjuntoString()
        {
            return tag + "@" +
                   modify + "@" +
                   p1.X.ToString().Replace(',', '.') + "," +
                   p1.Y.ToString().Replace(',', '.') + "," +
                   p1.Z.ToString().Replace(',', '.') + ";" +
                   p2.X.ToString().Replace(',', '.') + "," +
                   p2.Y.ToString().Replace(',', '.') + "," +
                   p2.Z.ToString().Replace(',', '.') + "@" +
                   filtro.layerBase + ";" +
                   filtro.GetConjunto() + "@" +
                   widthfactor;
        }
    }
}




