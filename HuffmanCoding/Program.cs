using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Unicode;
using HuffmanCoding;
using static System.Net.Mime.MediaTypeNames;

namespace HuffmanCoding
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Write("ENTER THE PATH OF THE FILE THAT YOU WANT TO WORK ON : ");
                string filePath = Console.ReadLine(); // E:\\Git projects\\huffman\\file.txt

                Console.WriteLine("ENTER 'c' IF YOU WANT TO COMPRESS THE FILE");
                Console.WriteLine("ENTER 'd' IF YOU WANT TO EXTRACT THE FILE");
                string c = Console.ReadLine();

                if (c.ToLower() == "c")
                    Encoder.encode(filePath);

                else if (c.ToLower() == "d")
                    Decoder.decode(filePath);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
