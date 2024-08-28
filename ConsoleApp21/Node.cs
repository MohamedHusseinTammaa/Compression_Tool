using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding
{
    public class Node
    {
        public char chr { get; set; }
        public int frq { get; set; }
        public Node left { get; set; }
        public Node right { get; set; }
        public Node(char chr, int frq)
        {
            left = right = null;
            this.frq = frq;
            this.chr = chr;
        }
    }
}
