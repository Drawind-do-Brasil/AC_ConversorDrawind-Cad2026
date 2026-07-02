using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConversorDrawindDLL
{

    public class TagBlockClass
    {
        public bool verifiqued = false;
        public double widthfactor = 1;
        public string tag;
        public bool modify = false;
        public PointEspecial2 p1 = new PointEspecial2();
        public PointEspecial2 p2 = new PointEspecial2();
        public Filter filtro;
        public string text = "";
        public int indiceRelacao = -1;
        public bool isSociate = false;
        public TagBlockClass()
        {
            filtro = new Filter();
            filtro.SetConjunto3();
        }

        public static List<TagBlockClass> GetTagBlock(Point3d p)
        {
            List<TagBlockClass> list = new List<TagBlockClass>();
            foreach (BlockClass tag in Arranjos.ListBlocks)
            {
                list.AddRange(tag.listTags.Where(a => !a.verifiqued && ConvertBlocks.CheckPoint(p,
                   ConvertBlocks.GetPTReal(new Point3d(a.p1.X, a.p1.Y, a.p1.Z)), ConvertBlocks.GetPTReal(new Point3d(a.p2.X, a.p2.Y, a.p2.Z)))).ToList());

            }

            return list;

        }

        public void SetConjunto(string conjunto)
        {
            string[] conj = conjunto.Split('@');
            tag = conj[0];
            modify = Convert.ToBoolean(conj[1]);
            string[] pts = conj[2].Split(';');
            string[] pts1 = pts[0].Split(',');
            string[] pts2 = pts[1].Split(',');
            p1 = new PointEspecial2(Convert.ToDouble(pts1[0].ReplaceComma()), Convert.ToDouble(pts1[1].ReplaceComma()), Convert.ToDouble(pts1[2].ReplaceComma()));
            p2 = new PointEspecial2(Convert.ToDouble(pts2[0].ReplaceComma()), Convert.ToDouble(pts2[1].ReplaceComma()), Convert.ToDouble(pts2[2].ReplaceComma()));
            string[] subconj = conj[3].Split(';');
            filtro.layerBase = subconj[0];
            filtro.SetConjunto(subconj[1]);
            widthfactor = Convert.ToDouble(conj[4].ReplaceComma());
        }

    
    }
}
