using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HuffmanCoding
{
    internal class Encoder
    {
        public static void Encode(string filePath)
        {
            var fi1 = new FileInfo(filePath);
            long originalSize = fi1.Length; // Get the size of the file

            string text;
            using (FileStream fso = new FileStream(filePath, FileMode.Open))
            using (StreamReader sr = new StreamReader(fso))
            {
                text = sr.ReadToEnd(); // Read all content of the file
            }

            // Create the encoded file path
            string directoryPath = Path.GetDirectoryName(filePath);
            string newFileName = "Encoded_" + Path.GetFileName(filePath);
            string encodedFilePath = Path.Combine(directoryPath, newFileName);

            using (FileStream fsc = new FileStream(encodedFilePath, FileMode.Create, FileAccess.ReadWrite))
            {
                // Create a min heap using list
                var minHeap = new List<Node>();
                var frequencyDict = new Dictionary<char, int>();

                // Build the frequency dictionary
                foreach (char c in text)
                {
                    if (frequencyDict.ContainsKey(c))
                    {
                        frequencyDict[c]++;
                    }
                    else
                    {
                        frequencyDict[c] = 1;
                    }
                }

                // Create nodes and add them to the min heap
                foreach (var entry in frequencyDict)
                {
                    minHeap.Add(new Node(entry.Key, entry.Value));
                }

                // Build the Huffman Tree
                while (minHeap.Count > 1)
                {
                    minHeap.Sort((x, y) => x.frq.CompareTo(y.frq));
                    Node minNode1 = minHeap[0];
                    Node minNode2 = minHeap[1];
                    minHeap.RemoveRange(0, 2);

                    Node top = new Node('\0', minNode1.frq + minNode2.frq)
                    {
                        left = minNode1,
                        right = minNode2
                    };
                    minHeap.Add(top);
                }

                var dic = new Dictionary<char, string>();
                AddCodeToDic(minHeap[0], "", dic);
                PrintCodeTable(dic, fsc);
                WriteAsBinary(text, dic, fsc);

                FileInfo f1 = new FileInfo(encodedFilePath);
                long encodedFileSize = f1.Length;
                Console.WriteLine($"The Ratio of compression: {(originalSize / 1.0 / encodedFileSize / 1.0):F2}");
                Console.WriteLine("COMPRESSED SUCCESSFULLY =)");
            }
        }

        private static void WriteAsBinary(string fileContent, Dictionary<char, string> dic, FileStream fs)
        {
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                fs.Seek(0, SeekOrigin.End); // Move cursor to the end of the file

                string encodedText = string.Concat(fileContent.Select(c => dic[c]));
                int codeSize = encodedText.Length;

                // Convert content bits to an array of bytes
                byte[] bytes = ConvertToBytes(encodedText);

                // Write the padding size
                bw.Write((byte)(bytes.Length * 8 - codeSize));

                // Write the encoded bytes
                bw.Write(bytes);

                bw.Flush(); // Clear any buffered data
            }
        }

        private static byte[] ConvertToBytes(string bits)
        {
            int numberOfBytes = (bits.Length + 7) / 8;
            byte[] bytes = new byte[numberOfBytes];

            for (int i = 0; i < bits.Length; i++)
            {
                if (bits[i] == '1')
                {
                    bytes[i / 8] |= (byte)(1 << (7 - i % 8));
                }
            }

            return bytes;
        }

        private static void PrintCodeTable(Dictionary<char, string> dic, FileStream fs)
        {
            using (BinaryWriter bw = new BinaryWriter(fs, Encoding.UTF8))
            {
                int charTableSize = dic.Count;
                bw.Write(charTableSize); // Write the size of the Huffman table

                // Write each char and its code to the file
                foreach (var kvp in dic)
                {
                    string code = kvp.Value;
                    bw.Write(code.Length);
                    bw.Write(kvp.Key);
                    bw.Write(Encoding.UTF8.GetBytes(code));
                }

                bw.Flush(); // Clear any buffered data
                fs.Flush();
            }
        }

        private static void AddCodeToDic(Node root, string s, Dictionary<char, string> dic)
        {
            if (root == null) return;

            if (!dic.ContainsKey(root.chr))
            {
                dic.Add(root.chr, s);
            }

            AddCodeToDic(root.left, s + '0', dic);
            AddCodeToDic(root.right, s + '1', dic);
        }
    }
}
