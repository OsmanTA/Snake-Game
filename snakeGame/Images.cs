﻿using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace snakeGame
{
    public static class Images
    {
        public readonly static ImageSource Body = LoadImage("Body.png");
        public readonly static ImageSource DeadBody = LoadImage("DeadBody.png");
        public readonly static ImageSource DeadHead = LoadImage("DeadHead.png");
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Food = LoadImage("Food.png");
        public readonly static ImageSource Head = LoadImage("Head.png");

        // Loads given image and loads it as an Image Source
        private static ImageSource LoadImage(string fileName)
        {
            return new BitmapImage(new Uri($"Assets/{fileName}", UriKind.Relative));
        }
    }
}