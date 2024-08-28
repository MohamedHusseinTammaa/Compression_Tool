using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp21
{
    internal class Program
    {
        class Node
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
        static void encode(string text)
        {
            // Create a min heap using list
            var minheap = new List<Node>();
            // Frequency dictionary to store the frequency of each character
            var frequencyDict = new Dictionary<char, int>();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (frequencyDict.ContainsKey(c))
                    frequencyDict[c]++;
                else
                    frequencyDict[c] = 1;
            }

            // Adding nodes to the minheap
            foreach (var i in frequencyDict)
            {
                minheap.Add(new Node(i.Key, i.Value));
            }

            // Create the Huffman Tree
            while (minheap.Count > 1)
            {
                minheap.Sort((x,y) => x.frq.CompareTo(y.frq));
                Node minNode1 = minheap[0];
                Node minNode2 = minheap[1];
                minheap.RemoveRange(0,2);
                Node top = new Node('$', minNode1.frq + minNode2.frq)
                {
                    left = minNode1,
                    right = minNode2
                };
                minheap.Add(top);
            }
            Dictionary<char, string> dic = new Dictionary<char,string>();
            addCodeToDic(minheap[0], "" , dic);
            foreach (var i in dic)
            {
                Console.WriteLine(i.Key+" "+i.Value);
            }
            convertToBinary(text, dic);

        }
        static void convertToBinary(string fileContent , Dictionary<char,string> dic)
        {
            FileStream fs = new FileStream("E:\\Git projects\\huffman\\compressed.txt", FileMode.Create ,FileAccess.ReadWrite);
            
            BinaryWriter bw = new BinaryWriter(fs);
            string encodedText="";
            
            for (int i = 0; i < fileContent.Length; i++) 
                encodedText +=dic[fileContent[i]];
            byte[] bytes = convertTobytes(encodedText);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        } 
        static byte [] convertTobytes (string bits)
        {
            int numberOfBytes = (bits.Length + 7) / 8;
            Console.WriteLine(numberOfBytes);
            byte[] bytes = new byte[numberOfBytes];
            for(int i = 0;i < bits.Length; i++)
            {
                if (bits[i] == '1')
                {
                    bytes[i / 8] |= (byte)(1 << (7 - i % 8));
                }
            }
            return bytes; 
        }
        static void addCodeToDic(Node root, string s , Dictionary<char,string> dic)
        {
            if (root == null) return;
            if (root.chr != '$') dic.Add(root.chr,s);
            addCodeToDic(root.left, s + '0', dic);
            addCodeToDic(root.right, s + '1', dic);
        }
        

        static void decode(string text)
        {
            // Decode implementation here (if needed)
        }

        static void Main(string[] args)
        {
            string filePath = "E:\\Git projects\\huffman\\file.txt";
            StreamReader sr = new StreamReader(filePath);
            string fileContent = sr.ReadToEnd();
            Console.WriteLine("ENTER 'c' IF YOU WANT TO COMPRESS THE FILE");
            Console.WriteLine("ENTER 'd' IF YOU WANT TO EXTRACT THE FILE");

            Char c = Char.Parse(Console.ReadLine());
            Console.WriteLine(fileContent);
            if (c == 'c') {
                encode(fileContent); 
            }
            else if (c == 'd') decode(fileContent);
        }
    }
}
