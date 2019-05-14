using System;
using Google.Cloud.Vision.V1;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Amanda's code
using Imaging;
//Alex's code
using ImageEdgeDetection;
using VisionApi;

namespace Interface
{
    public partial class Form1 : Form
    {

        //the first image that user loads
        System.Drawing.Image pic;

        //the wanted output
        Bitmap resultBitmap;
        System.Drawing.Image output_transformed;



        // The user inputs of terms
        TextBox TextBox1;
        TextBox TextBox2;
        TextBox TextBox3;

        //want to save the term for search purposes
        string term;

        // For which image segmentation selections to choose
        string segmentation;


        //for getting our files when K-clustering is used
        string[] files;

        string[] ImageCollections;


        //our user query
        String theText1;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox2 = (TextBox)sender;
            string theText2 = TextBox2.Text;
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            TextBox3 = (TextBox)sender;
            string theText3 = TextBox3.Text;

            ImageSegmentation ImageSeg = new ImageSegmentation();
           // ImageSeg.Compute(InpFile, OutFile);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Image Files (*.jpeg;*.jpg;*.png;*.gif)|(*.jpeg;*.jpg;*.png;*.gif|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pic = new Bitmap(openFileDialog1.FileName);
                output_transformed = new Bitmap(pic.Width, pic.Height);

            }
            
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Image = pic;

        }


        private void Button4_Click(object sender, EventArgs e)
        {

        }

        //the user outputs of terms
        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            
            TextBox1 = (TextBox)sender;
            theText1 = TextBox1.Text;
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

      
        private void Label4_Click(object sender, EventArgs e)
        {

        }

        //This is to name specifically the output file title if the user wishes to save
        private void TextBox4_TextChanged(object sender, EventArgs e)
        {
            term = textBox4.Text;
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == "Edge Detection")
            {
                textBox1.Visible = false;
                textBox1.Enabled = false;
            }

            if (comboBox1.Text == "K Clustering")
            {
                textBox1.Visible = true;
                textBox1.Enabled = true;
            }
           
        }
        //Our "search photo" button
        private void Button5_Click(object sender, EventArgs e)
        {
            //if the user presses "click", we gotta start analyzing what is given

            analyze();
        }
        ////~~~~~~~~~~~~~~~~~~~~~~~~~ Some crucial methods
        ///
        private void analyze()
        {
            //Do different image segmentations, depending on the choices
            if (comboBox1.SelectedIndex > -1)
            {
                String select = comboBox1.Text;

               if (select == "K Clustering")
                {
                    //do the K clustering method

                    segmentation = "K Cluster";
                    //System.Windows.Forms.MessageBox.Show("No image imported");

                }

                else if (select == "Edge Detection")
                {
                    //have this done in edge detection

                    segmentation = "Edge Detection";

                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("No Segmentation Type Selected.");
                    return;
                }
            }
            //first, the base case: we analyze when the user didn't put in a picture

            if (pic == null)
            {
                System.Windows.Forms.MessageBox.Show("No image imported");
                return;
            }
            else
            {
                if(segmentation == "K Cluster")
               {
                    //before doing the segmentation, check if "temp.jpg" exists and "Segmented Image" exists

                    
                    
                    //using the method from KMC.cs
                    Bitmap temp = (Bitmap)pic;
                    //set the name of the files created from the segmentation
                    temp.Save("t3mp.jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
                    //string directoryName = Path.GetDirectoryName("temp.jpg");
                    //string sourcePath = Directory.GetCurrentDirectory();
                    
                    

                    //Trying Amanda's method
                    ImageSegmentation.Compute("t3mp.jpg");
       
                    //Now the color segmented images are in a folder called "Segmented Image"

                    string path = Directory.GetCurrentDirectory();
                    string directory = path + "\\S3gmented Image";

                    files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
                    
                    int numFiles = files.GetLength(0);
                    ImageCollections = new string[numFiles];

                    int tempCounter = 0;
                    //array that collects 0 or 1, depending on user's input
                    int[] API_results = new int[numFiles];
                    //Now we go inside the Segmented Image folder store each image 
                    foreach (string file in files)
                    {
                        ImageCollections[tempCounter] = file;
                        Google.Cloud.Vision.V1.Image integer = Google.Cloud.Vision.V1.Image.FromFile(ImageCollections[tempCounter]);
                        //returns 0 if the word not found. returns 1 if found
                        int testNum = Vision.containsElement(integer, theText1);
                        API_results[tempCounter] = testNum;

                        tempCounter++;
                    }
                    //returns the first picture that is valued 1 and skips the rest...
                    for (int i =1; i <numFiles; i++)
                    {
                        if (API_results[i] == 1)
                        {
                            resultBitmap = new Bitmap(ImageCollections[i]);
                            break;
                        }

                    }
                  
                    //string name = ImageCollections[1];
                    //testNum = Vision.containsElement(ImageCollections[1], theText1);
                    
                    //temp.Dispose();

                    //for the test purpose, get the second image that has been cropped

                    //resultBitmap = new Bitmap(ImageCollections[2]);
                    //Google.Cloud.Vision.V1.Image i = Google.Cloud.Vision.V1.Image.FromFile(ImageCollections[2]);
                    //testNum = Vision.containsElement(i, theText1);

                    //System.Windows.Forms.MessageBox.Show(testNum.ToString());
                    //pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    pictureBox1.Image = resultBitmap;
                    /*
                    foreach (string file in tracker)
                    {
                        Console.WriteLine(file);

                    }
                    */


                }
                //System.Windows.Forms.MessageBox.Show(segmentation);

                if (segmentation == "Edge Detection")
                {
                    //System.Windows.Forms.MessageBox.Show("Here");

                    resultBitmap = Filter.ApplyFilter((Bitmap)pic);

                    //Image finaloutput = (Image)resultBitmap;
                    //pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    //resultBitmap.UnlockBits
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                    pictureBox1.Image = resultBitmap;
                }
            }

        }

        private void PictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void PictureBox2_Click_1(object sender, EventArgs e)
        {
            
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == resultBitmap)
            {
                pictureBox1.Image = pic;
            }
            else
            {
                if (resultBitmap != null)
                {
                    pictureBox1.Image = resultBitmap;
                }
            }
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            if (term == null)
            {
                System.Windows.Forms.MessageBox.Show("Please put the name for the output file");
            }
            if (term != null)
            {
                if (resultBitmap == null)
                {
                    System.Windows.Forms.MessageBox.Show("No picture segmented... please pick the options");
                    return;
                }
                resultBitmap.Save((term+".JPG"), System.Drawing.Imaging.ImageFormat.Jpeg);
                System.Windows.Forms.MessageBox.Show("Saved successfully as "+ (term + ".JPG"));
            }
        }
        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //emptying the files so we can delete
            /*
            int count = files.GetLength(0);
            int index = 0;
            while (index < count + 1)
            {
                files[index] = null;
                ImageCollections[index] = null;
            }
              */
            resultBitmap.Dispose();
            pic.Dispose();
            files = null;
            ImageCollections = null;
            pictureBox1 = null;
            //pictureBox1.Image = null;
           
            if (System.IO.Directory.Exists("S3gmented Image")){
               
                DeleteDirectory("S3gmented Image");
            }/*
            if (System.IO.File.Exists("t3mp.jpg"))
            {
                File.Delete("t3mp.jpg");
                System.Windows.Forms.MessageBox.Show("Deleted");
            }
            */
            Application.Exit();
        }
    }
}
