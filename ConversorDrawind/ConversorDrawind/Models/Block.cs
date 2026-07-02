using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ConversorDrawind
{
    public class Class_BlockClass
    {
        public string blockName;
        public string blockNameRelacao;
        public List<Class_TagBlockClass> listTags = new List<Class_TagBlockClass>();
        public Color cor = Color.Black;
        public void ResetTagReference()
        {
            foreach (Class_TagBlockClass item in listTags)
            {
                item.indiceRelacao = -1;
                item.isSociate = false;
            }
        }
        public Class_BlockClass DeepCopy()
        {
            Class_BlockClass other = (Class_BlockClass)this.MemberwiseClone();

            other.listTags = new List<Class_TagBlockClass>();
            foreach (Class_TagBlockClass item in this.listTags)
            {
                other.listTags.Add(item.DeepCopy());
            }
            return other;
        }
    }

    class BlockClassComparer : IEqualityComparer<Class_BlockClass>
    {
        public BlockClassComparer(Func<Class_BlockClass, Class_BlockClass, bool> equalityComparer, Func<Class_BlockClass, int> getHashCode)
        {
            EqualityComparer = equalityComparer;
            HashCodeGenerator = getHashCode;
        }

        Func<Class_BlockClass, Class_BlockClass, bool> EqualityComparer;
        Func<Class_BlockClass, int> HashCodeGenerator;

        public bool Equals(Class_BlockClass x, Class_BlockClass y)
        {
            return EqualityComparer(x, y);
        }

        public int GetHashCode(Class_BlockClass obj)
        {
            return HashCodeGenerator(obj);
        }
    }
}
