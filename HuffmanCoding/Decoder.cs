using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanCoding
{
    internal class Decoder
    {
        public static void decode(string filePath)
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            
            string directoryPath = Path.GetDirectoryName(filePath);
            string newFileName = "Decoded_" + Path.GetFileName(filePath);
            string encodedFilePath = Path.Combine(directoryPath, newFileName);
           
            FileStream fse = new FileStream(encodedFilePath, FileMode.Create, FileAccess.Write);
           
            var dic = new Dictionary<string, char>();
            string content;
            readFile(fs, dic, out content);
            byte [] decodedContent= Encoding.UTF8.GetBytes( writeCodeAsChar(content, dic));
            BinaryWriter bw = new BinaryWriter(fse);
            bw.Write(decodedContent);

            bw.Flush();
            fs.Close();
            fse.Close();

        }
        static void readFile(FileStream fs, Dictionary<string, char> dic, out string content)
        {
            fs.Seek(0, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(fs);

            int charTableSize = br.ReadInt32(); // Read the size of the Huffman table

            // Read each character and its corresponding Huffman code from the file
            for (int i = 0; i < charTableSize; i++)
            {
                int sizeOfCode = br.ReadInt32();
                char c = br.ReadChar();
                byte[] codeBytes = br.ReadBytes(sizeOfCode);
                string code = Encoding.UTF8.GetString(codeBytes);

                dic.Add(code, c); // Add the code and character to the dictionary 
            }

            int padding = br.ReadByte(); // Read the padding size

            // Read the rest of the file as the encoded binary data
            List<byte> bytes = new List<byte>();
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                bytes.Add(br.ReadByte());
            }

            // Convert the byte array to a binary string
            content = BytesToBinaryString(bytes);
            content = content.Substring(0, content.Length - padding); // Remove the padding bits

            br.Close();
        }
        public static string BytesToBinaryString(List<byte> byteArray)
        {
            StringBuilder binaryString = new StringBuilder();

            // Convert each byte to its binary representation
            foreach (byte b in byteArray)
            {
                binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0')); // Ensure each byte is represented as 8 bits
            }

            return binaryString.ToString();
        }

        static string writeCodeAsChar(string content, Dictionary<string, char> dic)
        {
            StringBuilder decodedContent = new StringBuilder();

            int start = 0;

            // Iterate through the binary string to decode each Huffman code
            for (int i = 1; i <= content.Length; i++)
            {
                string code = content.Substring(start, i - start); // Extract a substring representing a Huffman code

                if (dic.ContainsKey(code))
                {
                    decodedContent.Append(dic[code]); // Append the corresponding character to the decoded content
                    start = i; // Move the start pointer to the next potential code
                }
            }

            return decodedContent.ToString(); // Return the decoded string
        }
    }
}
