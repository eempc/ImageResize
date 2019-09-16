using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace ImageResize {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Hello World!");

            List<string> imageFiles = new List<string> { ".JPG", ".PNG", ".GIF", ".BMP" };

            string targetFolder = Directory.GetCurrentDirectory();
            string[] fileEntries = Directory.GetFiles(targetFolder);

            foreach (string file in fileEntries) {
                Console.WriteLine(file);
                if (imageFiles.Contains(Path.GetExtension(file).ToUpperInvariant())) {
                    Console.WriteLine("is image");
                }
                else {
                    Console.WriteLine("nope");
                }
            }




            Console.Read();
        }

        public string[] GetFiles(string directory) {
            string[] fileEntries = Directory.GetFiles(directory);
            return fileEntries;
        }
    }
}
