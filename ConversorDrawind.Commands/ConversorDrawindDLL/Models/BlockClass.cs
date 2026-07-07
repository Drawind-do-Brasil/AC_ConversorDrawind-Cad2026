using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ConversorDrawindDLL
{
    public class BlockClass
    {
        public string blockName;
        public string blockNameRelacao;
        public List<TagBlockClass> listTags = new List<TagBlockClass>();
        public Color cor = Color.Black;

        public BlockClass()
        {
        }

        public BlockClass( string name)
        {
            blockName = name;
        }
        public double GetWFTag(string tag)
        {
            foreach (TagBlockClass item in listTags)
            {
                if (item.tag == tag)
                    return item.widthfactor;
            }
            return 1;
        }
    }
}
