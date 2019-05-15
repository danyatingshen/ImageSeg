//Amanda Shen group cs241 Project K-custering 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Collections.Concurrent;
using System.IO;
using System.Drawing.Imaging;
//--------------------------------------------------------------------------------------------------------------------------
namespace Imaging
{   
    //a point is represnet each pixcel of the image:
    class Point<T>{
        // my constructor for the point pixcel: 
        private T x_coord; // X 
        private T y_coord; // Y 
        private Color xy_color; // color
        public Point(T x, T y, Color color) { 
            this.x = x; 
            this.y = y; 
            this.color = color; 
        }   
        //to make point read and write able: set get and set
        // X coordinate: getter and setter
        // public T x{ get; set; }
        // public T y{ get; set; }
        // public T color{ get; set; }

        public T x { 
            get { return x_coord; } 
            set { x_coord = value; } 
        }
        // Y coordinate: getter and setter
        public T y { 
            get { return y_coord; } 
            set { y_coord = value; } 
        }
        // color : getter and setter
        public Color color { 
            get { return xy_color; } 
            set { xy_color = value; } 
        }
  
    }

 //--------------------------------------------------------------------------------------------------------------------------
    //make the image editable 
    class MyBitmap{
        //------------------------------------------------------------------------
        //------------------------------------------------------------------------
        //start of simple constructor: 
        public Int32 Width { 
            get { 
                return my_bitmap.Width; 
            } 
        }
        public Int32 Height { 
            get { 
                return my_bitmap.Height;
             } 
        }
        //use Bitmap class to build mine: 
        public Bitmap my_bitmap = null;
        private Rectangle rectangle; 
        //inicialized mybitmap
        //filename of the image to try to do seg
        public MyBitmap(string name){       
            if (my_bitmap == null){
                my_bitmap = new Bitmap(name);
                rectangle = new Rectangle(new Point(0, 0), my_bitmap.Size);
            }
        }
        public MyBitmap(Int32 width, Int32 height){
            if (my_bitmap == null){
                my_bitmap = new Bitmap(width, height);
                rectangle = new Rectangle(new Point(0, 0), my_bitmap.Size);
            }
        }
        public MyBitmap(Bitmap new_map){
            if (my_bitmap == null){
                my_bitmap = new Bitmap(new_map);
                rectangle = new Rectangle(new Point(0, 0), my_bitmap.Size);
            }
        }
        public static implicit operator MyBitmap(Bitmap new_map){
            return new MyBitmap(new_map);
        }
        //------------------------------------------------------------------------
        //------------------------------------------------------------------------
        private BitmapData data_info = null;
        private IntPtr bitmap_pointer;
        private byte[] pixels = null;
        

        //save the image: 
        public void Save(string name){
            my_bitmap.Save(name);
        }
        //-------------------------------------
        //borrow the idea from bit map class for lock and unlock bits: 
        //"Use the LockBits method to lock an existing bitmap in system memory so that it can be changed programmatically."
        public void LockBits(){
            //paramter: rectangle, flag(r/w),A PixelFormat enumeration that specifies the data format of this Bitmap.
            //Returns BitmapData: A BitmapData that contains information about this lock operation.
            data_info = my_bitmap.LockBits(rectangle, System.Drawing.Imaging.ImageLockMode.ReadWrite, my_bitmap.PixelFormat);
            bitmap_pointer = data_info.Scan0;
            
            //inicailized pixels: 
            int height=my_bitmap.Height;
            int arraysize=Math.Abs(data_info.Stride) * height;
            pixels = new byte[arraysize];
            //System.Runtime.InteropServices.Marshal.Copy:
            //Copies data from a managed array to an unmanaged memory pointer, or from an unmanaged memory pointer to a managed array.
            System.Runtime.InteropServices.Marshal.Copy(bitmap_pointer, pixels,0, Math.Abs(data_info.Stride) * my_bitmap.Height);
        }
        //-------------------------------------
        //borrow the idea from bit map class for lock and unlock bits: 
        public void UnlockBits(){
            bitmap_pointer = data_info.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0,bitmap_pointer, Math.Abs(data_info.Stride) * my_bitmap.Height);

            // Unlock the bits.
            my_bitmap.UnlockBits(data_info);
        }
        //-------------------------------------

        public void SetPixel(Int32 x, Int32 y, Color color){
            int width=my_bitmap.Width;
            Int32 color_depth = System.Drawing.Bitmap.GetPixelFormatSize(data_info.PixelFormat);
            Int32 Pixel = (x + y * width) * (color_depth / 8);

            if (color_depth == 32){
                pixels[Pixel] = color.B;
                pixels[Pixel + 1] = color.G;
                pixels[Pixel + 2] = color.R;
                pixels[Pixel + 3] = color.A;
            }else if (color_depth == 24){
                pixels[Pixel] = color.B;
                pixels[Pixel + 1] = color.G;
                pixels[Pixel + 2] = color.R;
            }else if (color_depth == 16){
                pixels[Pixel] = color.B;
                pixels[Pixel + 1] = color.G;
            }else if (color_depth == 8){
                pixels[Pixel] = color.B;
            }
        }
        //-------------------------------------
        //this fuction return pixel color of the pixcel sepcified:
        public Color GetPixel(Int32 x, Int32 y){
            int width=my_bitmap.Width;
            //System.Drawing.Bitmap.GetPixelFormatSize:
            //this colde: return the color depth of the specified pixel format.
            Int32 color_depth = System.Drawing.Bitmap.GetPixelFormatSize(data_info.PixelFormat);
            Int32 Pixel = (x + y * width) * (color_depth / 8);
            Int32 Red = 0;
            Int32 Green = 0; 
            Int32 Blue = 0;
            Int32 Alpha = 0;
            //append color based on different color depth: 
            if (color_depth == 32){
                Blue = pixels[Pixel];
                Green = pixels[Pixel + 1];
                Red = pixels[Pixel + 2];
                Alpha = pixels[Pixel + 3];
            }else if (color_depth == 24){
                Blue = pixels[Pixel];
                Green = pixels[Pixel + 1];
                Red = pixels[Pixel + 2];
            }else if (color_depth == 16){
                Blue = pixels[Pixel];
                Green = pixels[Pixel + 1];
            }else if (color_depth == 8){
                Blue = pixels[Pixel];
            }
            //use function FromArgb to formed the final color of pixcel: 
            if(color_depth != 8){
                return Color.FromArgb(Red, Green, Blue);
            }else{
                return Color.FromArgb(Blue, Blue, Blue);
            }
        }     
 
    }
    //--------------------------------------------------------------------------------------------------------------------------
    //frame defined as a modefilyable picture and  with one center and a list of centroids:
    class Frame{          
        private MyBitmap bit_frame = null;// inicialized create bitmap frame 
        private Point<Int32> my_Center;// int32 as type for super_pixel:
        private List<Point<int>> Centroids_list = null;//define a list centroid pixel

        // fram constructor
        public Frame(MyBitmap bitmap_frame, List<Point<int>> centroids_list, Point<int> center){
            this.frame = bitmap_frame;
            this.Centroids = centroids_list;
            this.Center = center;
        }
        
        public MyBitmap frame{
            get { return bit_frame; }
            set { bit_frame = value;  }
        }
        
        public List<Point<int>> Centroids{
            get { return Centroids_list; }
            set { Centroids_list = value; }
        }
        public Point<Int32> Center{
            get { return my_Center; }
            set { my_Center = value; }
        }
        
    }
    //--------------------------------------------------------------------------------------------------------------------------
    //cluster is defined as a cluster of frame that has a modefilyable picture and  with one center and a list of centroids
    class Clusters : IEnumerable<Frame>{
        //set up: this small portion of construction of IEnumerator follow source:
        //https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1?view=netframework-4.8 
        public Frame this[Int32 Index]{
            get { return my_cluster.ElementAt(Index); }
        }

        public IEnumerator<Frame> GetEnumerator(){
            return my_cluster.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator(){
            return this.GetEnumerator();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        private bool is_right_distance(List<Point<int>> Centroids, Point<int> Target, Int32 Distance)
        {
            Int32 index = -1; 
            bool is_exsit = false;
            while (++index < Centroids.Count() && !is_exsit){
                double x=Math.Abs(Target.x - Centroids.ElementAt(index).x);
                double y=Math.Abs(Target.y - Centroids.ElementAt(index).y);
                is_exsit = (x <= Distance) || ( y<= Distance) ? true : false;
            }
            return is_exsit;
        }
        private bool is_right_color(List<Point<int>> Centroids, Point<int> Target, Int32 Offset)
        {
            Int32 index = -1; 
            bool is_exsit = false;
            //check if within boundary
            while (++index < Centroids.Count() && !is_exsit){
                double abs_r=Math.Abs(Centroids[index].color.R - Target.color.R);
                double sqr_r=Math.Pow(abs_r, 2);
                double abs_g=Math.Abs(Centroids[index].color.G - Target.color.G);
                double sqr_g=Math.Pow(abs_g, 2);
                double abs_b=Math.Abs(Centroids[index].color.B - Target.color.B);
                double sqr_b=Math.Pow(abs_b, 2);
                is_exsit = (Math.Sqrt(sqr_r+ sqr_g +sqr_b)) <= Offset ? true : false;
            }
            return is_exsit;
        }
        private System.Random num_rand = new System.Random();
        public void Generate(ref List<Point<int>> Centroids, MyBitmap Image, Int32 distance_limit, Int32 color_limit)
        {
            int width=Image.Width;
            int height= Image.Height;
            Image.LockBits(); //Locks a Bitmap into system memory.
            for (Int32 i = 0; i < (width*height); i++){         //generate a specific amount of super-pixels
                Int32 x_random = num_rand.Next(0, width);       // random generate x coord for random super pixcel 
                Int32 y_random = num_rand.Next(0, Image.Height);  // random generate y coord for random super pixcel 
                Point<int> point = new Point<int>(x_random,y_random, Image.GetPixel(x_random, y_random));//construct point on x and y. 
                //check if the point is right for color and distance, loop through centroids and check
                if (!this.is_right_color(Centroids, point, color_limit) && !this.is_right_distance(Centroids, point, distance_limit)){
                     if (!Centroids.Contains(point)){
                         Centroids.Add(point);
                     }
                }
            }
            //now unlock the image: 
            Image.UnlockBits();
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public Point<int> calculate_mean(MyBitmap Image, List<Point<int>> Centroids){
            //this function allocate and construct mean pixcel for each centroild list: 
            double mean_x = 0;
            double mean_Y = 0;
            double total=(double)Centroids.Count();
            for (Int32 i = 0; i < Centroids.Count(); i++){
                mean_x=mean_x+ Centroids[i].x /total;
                mean_Y=mean_Y+ Centroids[i].y /total;
            }
            Int32 x = Convert.ToInt32(mean_x);
            Int32 y = Convert.ToInt32(mean_Y);
            Image.LockBits();//locked image
            Color color = Image.GetPixel(x, y);
            Image.UnlockBits();
            return new Point<int>(x, y, color);
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------
        public void Add(MyBitmap Image, List<Point<int>> Centroids, Point<int> center){
            my_cluster.Add(new Frame(Image, Centroids, center));
        }
        //------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------

        private static HashSet<Frame> my_cluster = new HashSet<Frame>();//use hashset to coreat a set of cllection of cluster
        public void Init(string file, Int32 Distance, Int32 Offset){ //inite the cluster
            List<Point<int>> Centroids = new List<Point<int>>();
            MyBitmap orginal_image = new MyBitmap(file);//load orginal picture
            this.Generate(ref Centroids, orginal_image, Distance, Offset);
            Point<int> Mean = this.calculate_mean(orginal_image, Centroids);//get mean pixcel for list
            Frame new_frame=new Frame(orginal_image, Centroids, Mean);
            my_cluster.Add(new_frame);
        }
        
    }
    
     //--------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------
    //This is the actual imgae seg program:
    class ImageSegmentation
    {
        
        public  ImageSegmentation() { }

        //calculate Euclidian distance between two colors
        public static double Euclidian_distance(Point<int> a, Point<int> b){
                double abs_r=Math.Abs(a.color.R - b.color.R);
                double sqr_r=Math.Pow(abs_r, 2);
                double abs_g=Math.Abs(a.color.G - b.color.G);
                double sqr_g=Math.Pow(abs_g, 2);
                double abs_b=Math.Abs(a.color.B - b.color.B);
                double sqr_b=Math.Pow(abs_b, 2);
            return Math.Sqrt(sqr_r+ sqr_g +sqr_b);
        }

        private const Int32 color_limit = 50;
        private const Int32 distance_limit = 5;
        private static Clusters master_cluster = new Clusters();

        public static void Compute(string name){
            

            //generate super_pixcel with original pic: 
            master_cluster.Init(name, distance_limit, color_limit);
            // create segmented bitmap: 
            int master_width=master_cluster[0].frame.Width;
            int master_height=master_cluster[0].frame.Height;
            MyBitmap final_pic = new MyBitmap(master_width,master_height);

            //----------------------------------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------

            Int32 number = 0;
            int master_size=master_cluster.Count();
            // Iterate throught the array of clusters until we've process all clusters being generated
            for (Int32 i = 0; i <master_size ; i++){
                //for each cluster: get centroid and frame saperatly: 
                List<Point<int>> Centroids = master_cluster[i].Centroids.ToList();
                MyBitmap myFrame = new MyBitmap(master_cluster[i].frame.my_bitmap);
                //------------------------------------------------------------------------------------------
                //myFrame.Save("Segmented Image" + number + ".png");
                //------------------------------------------------------------------------------------------
                //lock bits to start modify: 
                myFrame.LockBits();
                //deal with frame first: 
                for (Int32 j = 0; j < Centroids.Count(); j++){
                    // Obtain the value of Width and Height of the image for the current cluster
                    Int32 current_w = myFrame.Width;
                    Int32 current_h = myFrame.Height;

                    // new bitmap for new cluster:
                    MyBitmap modefilyable = new MyBitmap(current_w, current_h);
                    modefilyable.LockBits();
                    for (Int32 x = 0; x < current_w; x++){
                         for (Int32 y = 0; y < current_h; y++){
                              // For each pixel in this matrix, compute the value of color offset of the current centroid super-pixel
                            Point<int> first=new Point<int>(x, y, myFrame.GetPixel(x, y));
                            Point<int> second=new Point<int>(Centroids[j].x, Centroids[j].y, Centroids[j].color);
                            double color_depth = Euclidian_distance(first,second);
                            //if (color_depth <= 50){
                            if(color_depth <= 75) { 
                                modefilyable.SetPixel(x, y, Centroids[j].color);
                            }else{
                                modefilyable.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                            } 
                            
                         }
                    }

                    modefilyable.UnlockBits();
                    List<Point<int>> list = new List<Point<int>>();
                    list.Add(Centroids[0]);
                    // calculate mean for new cluster: 
                    Point<int> new_avg = master_cluster.calculate_mean(modefilyable, list);
                    if (new_avg.x != master_cluster[i].Center.x && new_avg.y != master_cluster[i].Center.y){
                        master_cluster.Add(modefilyable, list, new_avg);
                    }
                    //increase seg pic by 1: 
                     number++;
                }//j second for loop stop here
                
                myFrame.UnlockBits();
            }
            //----------------------------------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------
            final_pic.LockBits();

            for (Int32 q = 0; q < master_cluster.Count(); q++){
                 MyBitmap seg = new MyBitmap(master_cluster[q].frame.my_bitmap);
                 int w = seg.Width;
                 int h = seg.Height;
                 seg.LockBits();
                 seg.Save("resulting" + q + ".png");
                 for (Int32 r = 0; r < w; r++){
                      for (Int32 c = 0; c < h; c++){
                           if (seg.GetPixel(r, c) != Color.FromArgb(255, 255, 255)){
                               final_pic.SetPixel(r, c, seg.GetPixel(r, c));
                           }
                      }
                 }

                 seg.UnlockBits();
            }
            //----------------------------------------------------------------------------------------------
            //----------------------------------------------------------------------------------------------
            final_pic.UnlockBits();
            //final_pic.Save(Output); 
            //save the segmented Image into folder: 
                // Use the DirectoryInfo class for typical operations such as copying, moving, renaming, creating, and deleting directories.
                string sourcePath = Directory.GetCurrentDirectory();
                DirectoryInfo my_directory = new DirectoryInfo("S3gmented Image");
                string targetPath=sourcePath+"/S3gmented Image";
                //create directory:
                if (my_directory.Exists == false){
                    Directory.CreateDirectory("S3gmented Image");
                 }
                //copy file: 
                if (System.IO.Directory.Exists(sourcePath)){
                    string[] files = System.IO.Directory.GetFiles(sourcePath,"resulting*");

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files){
                        // Use static Path methods to extract only the file name from the path.
                        string fileName = System.IO.Path.GetFileName(s);
                        string destFile = System.IO.Path.Combine(targetPath, fileName);
                        System.IO.File.Copy(s, destFile, true);
                    }
                }else{
                    Console.WriteLine("Source path does not exist!");
                 }


                 if (System.IO.Directory.Exists(sourcePath)){
                    string[] files = System.IO.Directory.GetFiles(sourcePath,"resulting*");

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string s in files){
                        File.Delete(s);
                    }
                }else{
                    Console.WriteLine("Source path does not exist!");
                 }
               
                 
            // // Copy each file into it's new directory.
            // foreach (FileInfo fi in source.GetFiles()){
            //     GetFiles(String)
            //     Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
            //     fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
            // }       
        }

        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        
    }
     //--------------------------------------------------------------------------------------------------------------------------
    /*
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Image Segmentation implemented version by Amanda, modeled on Arthur V. Ratz");

            Console.Write("Input file name: ");
            string InpFile = Console.ReadLine();

            Console.Write("Output file name: ");
            string OutFile = Console.ReadLine();

            ImageSegmentation ImageSeg = new ImageSegmentation();
            ImageSeg.Compute(InpFile);

        }
    }
    */
}