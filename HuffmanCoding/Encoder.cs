using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding
{
    internal class Encoder
    {
        public static void encode(string filePath )
        {
            var fi1 = new FileInfo(filePath);
            long oraginalSize = fi1.Length; // to get the size of the file

            FileStream fso = new FileStream(filePath,FileMode.Open);
            StreamReader sr = new StreamReader(fso);
            string text = sr.ReadToEnd(); // to read all content of the file
            sr.Close();
            
            // to create the encoded file path 
            string directoryPath = Path.GetDirectoryName(filePath);
            string newFileName = "Encoded_"+Path.GetFileName(filePath);
            string encodedFilePath = Path.Combine(directoryPath,newFileName); 
            
            FileStream fsc = new FileStream(encodedFilePath, FileMode.Create, FileAccess.ReadWrite);
            
            // Create a min heap using list
            var minheap = new List<Node>();
            // Frequency dictionary to store the frequency of each character
            var frequencyDict = new Dictionary<char, int>();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (!frequencyDict.ContainsKey(c)) frequencyDict.Add(c,1);
                else frequencyDict[c]++;

            }

            // Adding nodes to the minheap
            foreach (var i in frequencyDict)
            {
                minheap.Add(new Node(i.Key, i.Value));
            }

            // Create the Huffman Tree
            while (minheap.Count > 1)
            {
                minheap.Sort((x, y) => x.frq.CompareTo(y.frq));
                Node minNode1 = minheap[0];
                Node minNode2 = minheap[1];
                minheap.RemoveRange(0, 2);
                Node top = new Node('\0', minNode1.frq + minNode2.frq)
                {
                    left = minNode1,
                    right = minNode2
                };
                minheap.Add(top);
            }
            Dictionary<char, string> dic = new Dictionary<char, string>();
            addCodeToDic(minheap.Min(), "", dic);
            printCodeTable(dic, fsc);
            writeAsBinary(text, dic, fsc);
            FileInfo f1 = new FileInfo(encodedFilePath);
            long encodedFileSize =f1.Length;
            Console.WriteLine($"The Ratio of comperssion : {(oraginalSize/1.0/encodedFileSize/1.0)}");
            Console.WriteLine("COMPRESSED SUCCESSFULLY =)");

        }
        static void writeAsBinary(string fileContent, Dictionary<char, string> dic, FileStream fs)
        {
            BinaryWriter bw = new BinaryWriter(fs);
            fs.Seek(0, SeekOrigin.End); // to make sure that the curser at the end of the file

            string encodedText = "";
            // to convert all file content to encoded form
            for (int i = 0; i < fileContent.Length; i++)  
                encodedText += dic[fileContent[i]];

            int codeSize = encodedText.Length; 
            // to convert content bits to an array of bytes
            byte[] bytes = convertTobytes(encodedText);
           
            // to write the padding size
            bw.Write((byte)(bytes.Length * 8 - codeSize)); 
            
            // to write the encoded bytes
            bw.Write(bytes); 

            bw.Flush(); //  to clear any buffered data in the writer
            fs.Close();
        }
        static byte[] convertTobytes(string bits)
        {
            // Calculate the number of bytes needed to store the bit string.
            // The length of the bit string is divided by 8 because each byte contains 8 bits.
            // We add 7 before dividing to account for any remaining bits that would require an extra byte.
            int numberOfBytes = (bits.Length + 7) / 8;

            // Create an array of bytes to store the final encoded data.
            byte[] bytes = new byte[numberOfBytes];

            // Loop through each bit in the string.
            for (int i = 0; i < bits.Length; i++)
            {
                // If the current bit is '1', set the corresponding bit in the byte array.
                // We determine which byte (index i / 8) and which bit within that byte (position 7 - (i % 8)) to set.
                if (bits[i] == '1')
                {
                    // use bitwise OR to shift the bit to the right posistion.
                    bytes[i / 8] |= (byte)(1 << (7 - i % 8));
                }
            }

            return bytes;
        }

        static void printCodeTable(Dictionary<char, string> dic, FileStream fs)
        {
            BinaryWriter bw = new BinaryWriter(fs);

            int charTableSize = dic.Count();
            bw.Write(charTableSize); // to write the value if char table size

            // loop to each char and its code in the table to write it into the file
            foreach (char c in dic.Keys)
            {
                int sizeOfCode = dic[c].Length;
                // write the char in form of ([size of huffman code][char][code as bytes])

                bw.Write(sizeOfCode);
                bw.Write(c);
                bw.Write(Encoding.UTF8.GetBytes(dic[c]));
                
            }

            bw.Flush(); // to clear any buffered data 
            fs.Flush();
        }

        static void addCodeToDic(Node root, string s, Dictionary<char, string> dic)
        {
            if (root == null) return;
            
            if (!dic.ContainsKey(root.chr))dic.Add(root.chr, s); 

            addCodeToDic(root.left, s + '0', dic);
            addCodeToDic(root.right, s + '1', dic);
        }

    }
}
