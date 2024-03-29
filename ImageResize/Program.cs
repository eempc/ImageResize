﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace ImageResize {
    class Program {
        static List<string> imageFiles = new List<string> { ".JPEG", ".PNG", ".GIF", ".BMP", ".JPG" };

        static void Main(string[] args) {
            Console.WriteLine("Hello World!");

            string date = DateTime.Now.ToString("yyyy-MM-dd");

            string targetFolder = Directory.GetCurrentDirectory();
            string[] fileEntries = Directory.GetFiles(targetFolder);

            foreach (string file in fileEntries) {
                Console.WriteLine(file);
                if (imageFiles.Contains(Path.GetExtension(file).ToUpperInvariant())) {

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    string ext = Path.GetExtension(file);

                    //if (ext.ToLower() == ".jpg") ext = ".Jpeg";
                  
                    Console.WriteLine("is image: " + ext);

                    Image newImg = ResizeImage(file);
                    //ImageFormat imgFormat = ParseImageFormat(ext);

                    string saveFile = $"{fileName} {date}" + ".png";
                    newImg.Save(saveFile, ImageFormat.Png);
                    newImg.Dispose();
                } else {
                    Console.WriteLine("nope, not image");
                }
            }
        }

        public static ImageFormat ParseImageFormat(string ext) {
            if (!imageFiles.Contains(ext.ToUpper())) {
                throw new ArgumentException(ext + " is not a valid image format");
            } else {
                return (ImageFormat)typeof(ImageFormat)
                    .GetProperty(RemoveDotFirstCharUpper(ext), BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
                    .GetValue(null);
            }
        }

        public static string RemoveDotFirstCharUpper(string str) {
            return char.ToUpper(str[1]) + str.Substring(2);
        }

        public static Image ResizeImage(string file, int desiredX = 1920, int desiredY = 1080) {
            Image img = Image.FromFile(file);
            double sourceX = img.Width;
            double sourceY = img.Height;

            if (img.Width == desiredX && img.Height == desiredY) {
                return img;
            }

            double desiredRatio = (double)desiredX / (double)desiredY; // 1920 / 1080 = 1.7778
            double sourceRatio = sourceX / sourceY;
            double yMultiplier, xMultiplier;
            double yPadding, xPadding;
            double xStartingPixel, yStartingPixel;
            double newX, newY;

            // Behaviour if the ratio is less than 
            if (sourceRatio < desiredRatio) {
                // E.g. 1500 / 1000 = 1.5 source ratio, i.e. more portrait
                // Lengthen/Shorten Y axis with a height multiplier
                yMultiplier = desiredY / sourceY; // 1080 / 1000 = 1.08
                xMultiplier = 0; // X will get padding and needs no multiplier

                yPadding = 0; // Y will be at the desired value after multiplying so needs no multiplier
                xPadding = desiredX - yMultiplier * sourceX; // 1920 is desired. After multiplying 1.08 * 1500 = 1620, then the padding needed is 300                

                newX = yMultiplier * sourceX;
                newY = yMultiplier * sourceY;

                yStartingPixel = 0;
                xStartingPixel = xPadding / 2;
            } else {
                // Example is a tall image, e.g. 2000x500 = ratio of 4, more squashed
                // Lengthen/shorten X axis with a width multiplier
                xMultiplier = desiredX / sourceX; // 1920 / 2000 = 0.96
                yMultiplier = 0;

                xPadding = 0;
                yPadding = desiredY - xMultiplier * sourceY; // 1080 - 0.96 * 500 = 600 pixels needed

                newX = xMultiplier * sourceX;
                newY = xMultiplier * sourceY;

                xStartingPixel = 0;
                yStartingPixel = yPadding / 2;
            }

            // Instantiate a new blank bitmap and set its resolution
            Bitmap bmp = new Bitmap(desiredX, desiredY, PixelFormat.Format24bppRgb);
            bmp.SetResolution(img.HorizontalResolution, img.VerticalResolution);
            bmp.MakeTransparent();

            Graphics graphics = Graphics.FromImage(bmp);
            graphics.Clear(Color.Transparent);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            Rectangle newRectangle = new Rectangle((int)xStartingPixel, (int)yStartingPixel, (int)newX, (int)newY);
            Rectangle sourceRectangle = new Rectangle(0, 0, (int)sourceX, (int)sourceY);

            graphics.DrawImage(img, newRectangle, sourceRectangle, GraphicsUnit.Pixel);
            graphics.Dispose();
            return bmp;


        }
    }
}
