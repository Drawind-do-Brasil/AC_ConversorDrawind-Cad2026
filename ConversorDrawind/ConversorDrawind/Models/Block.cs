using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ConversorDrawind
{
    public class Block
    {
        public string blockName;
        public string blockNameRelacao;
        public List<TagBlock> listTags = new List<TagBlock>();
        public Color cor = Color.Black;
        public void ResetTagReference()
        {
            foreach (TagBlock item in listTags)
            {
                item.indiceRelacao = -1;
                item.isSociate = false;
            }
        }
        public Block DeepCopy()
        {
            Block other = (Block)this.MemberwiseClone();

            other.listTags = new List<TagBlock>();
            foreach (TagBlock item in this.listTags)
            {
                other.listTags.Add(item.DeepCopy());
            }
            return other;
        }
    }

    class BlockClassComparer : IEqualityComparer<Block>
    {
        public BlockClassComparer(Func<Block, Block, bool> equalityComparer, Func<Block, int> getHashCode)
        {
            EqualityComparer = equalityComparer;
            HashCodeGenerator = getHashCode;
        }

        Func<Block, Block, bool> EqualityComparer;
        Func<Block, int> HashCodeGenerator;

        public bool Equals(Block x, Block y)
        {
            return EqualityComparer(x, y);
        }

        public int GetHashCode(Block obj)
        {
            return HashCodeGenerator(obj);
        }
    }
}

