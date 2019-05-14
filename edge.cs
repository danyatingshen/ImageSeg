using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;

// Alex Jensen, Sam Fertig, Amanda Shen, and Eunjun Choo
// We used sample source code from softwarebydefault.com/2013/05/11/image-edge-detection/ as inspiration.

namespace ImageEdgeDetection
{
    // These are the matrices that we use in our edge detection
    public static class Matrix
    {
        public static double[,] Laplacian3x3
        {
            get
            {
                return new double[,]  
                { { -1, -1, -1,  }, 
                  { -1,  8, -1,  }, 
                  { -1, -1, -1,  }, };
            }
        }

        public static double[,] Laplacian5x5
        {
            get
            {
                return new double[,] 
                { { -1, -1, -1, -1, -1, }, 
                  { -1, -1, -1, -1, -1, }, 
                  { -1, -1, 24, -1, -1, }, 
                  { -1, -1, -1, -1, -1, }, 
                  { -1, -1, -1, -1, -1  }, };
            }
        }

    }

    public static class ExtendBitmap
    {
	// This is our convolution filter that we apply to our original picture (Bitmap source)
        private static Bitmap convFilter(Bitmap source, double[,] fMatrix) 
	{
	    // First, we lock all of the pixels
            BitmapData sourceData = source.LockBits
	    	       		    (new Rectangle(0, 0, source.Width, source.Height),
				    ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

	    // We now make a pixel buffer and a result buffer
            byte[] pixelBuffer = new byte[sourceData.Stride*sourceData.Height];
            byte[] resultBuffer = new byte[sourceData.Stride*sourceData.Height];

            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length);

	    // Now, we can unlock the bits
            source.UnlockBits(sourceData);

            float rgb = 0;

            for (int i=0; i<pixelBuffer.Length; i+=4) {
                rgb = pixelBuffer[i]*0.11f;
                rgb += pixelBuffer[i+1]*0.59f;
                rgb += pixelBuffer[i+2]*0.3f;

                pixelBuffer[i] = (byte)rgb;
                pixelBuffer[i+1] = pixelBuffer[i];
                pixelBuffer[i+2] = pixelBuffer[i];
                pixelBuffer[i+3] = 255;
            }
         
            double red = 0.0;
            double green = 0.0;
            double blue = 0.0;

            int fWidth = fMatrix.GetLength(1);
            int fHeight = fMatrix.GetLength(0);

            int fOffset = (fWidth-1)/2;
            int cOffset = 0;

            int bOffset = 0;

            for (int offsetY=fOffset; offsetY<source.Height-fOffset; offsetY++) {
                for (int offsetX=fOffset; offsetX<source.Width-fOffset; offsetX++) {
		    red = 0;
		    green = 0;
                    blue = 0;
                 
                    bOffset = offsetY*sourceData.Stride+offsetX*4;

                    for (int fY=-fOffset; fY<=fOffset; fY++) {
                        for (int fX=-fOffset; fX<=fOffset; fX++) {
                            cOffset = bOffset+(fX*4)+(fY*sourceData.Stride);

                            blue += (double)(pixelBuffer[cOffset])*fMatrix[fY+fOffset, fX+fOffset];

                            green += (double)(pixelBuffer[cOffset+1])*fMatrix[fY+fOffset, fX+fOffset];

                            red += (double)(pixelBuffer[cOffset+2])*fMatrix[fY+fOffset, fX+fOffset];
                        }
                    }

		    if (red > 255){
		         red = 255;
		    } else if (red < 0) {
		        red = 0;
		    }

		    if (green > 255){
		         green = 255;
		    } else if (green < 0) {
		        green = 0;
		    }

                    if (blue > 255){
		         blue = 255;
		    } else if (blue < 0) {
		        blue = 0;
		    }

                    resultBuffer[bOffset] = (byte)(blue);
                    resultBuffer[bOffset+1] = (byte)(green);
                    resultBuffer[bOffset+2] = (byte)(red);
                    resultBuffer[bOffset+3] = 255;
                }
            }
	    
	    // Now, we begin making the Bitmap to be returned
            Bitmap result = new Bitmap(source.Width, source.Height);

	    // We lock its bits and then copy the data over
            BitmapData resultData = result.LockBits
	    	       		    (new Rectangle(0, 0, result.Width, result.Height),
				    ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length);
            result.UnlockBits(resultData);

            return result;
        }

        public static Bitmap Laplacian3x3Filter(this Bitmap source) 
        {
	    // Calls the convolution filter with the 3x3 matrix
            Bitmap result = ExtendBitmap.convFilter(source, Matrix.Laplacian3x3);

            return result;
        }

        public static Bitmap Laplacian5x5Filter(this Bitmap source)
        {
	    // Calls the convolution filter with the 5x5 matrix
            Bitmap result = ExtendBitmap.convFilter(source, Matrix.Laplacian5x5);

            return result;
        }
    }  

    class Program {
    	  static Bitmap ApplyFilter(Bitmap source) {
    	   
		Bitmap result = null;

//      	result = source.Laplacian3x3Filter();
       	   	result = source.Laplacian5x5Filter();

	   	return result;
           }

        static void Main(string[] args) {
	    Bitmap original = null;

	    // We first figure out what image to examine
            Console.Write("Input file name: ");
            string inputFile = Console.ReadLine();

	    // We make a Bitmap based on this
	    original = new Bitmap(inputFile);
	    // And then we apply the convolution filter
	    Bitmap result = ApplyFilter(original);
	    
	    // Now, we figure out where to save the resulting image
            Console.Write("Output file name: ");
            string outputFile = Console.ReadLine();
	    result.Save(outputFile);
        }
    }
}