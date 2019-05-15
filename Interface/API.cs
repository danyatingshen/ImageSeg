using Google.Cloud.Vision.V1;
using Google.Apis.Auth.OAuth2;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace VisionApi
{
    class Vision
    {
        public static List<String> containsElement(Image image_name)
        {
            List<String> myList = new List<String>();

            //setting up the Google Credentials
            try
            {
                string API_key = Directory.GetCurrentDirectory() + "\\key_thing.json"; ;
                GoogleCredential cred = GoogleCredential.FromFile(API_key);
                Channel channel = new Channel(
                ImageAnnotatorClient.DefaultEndpoint.Host, ImageAnnotatorClient.DefaultEndpoint.Port, cred.ToChannelCredentials());
                ImageAnnotatorClient client = ImageAnnotatorClient.Create(channel);

                // Shutdown the channel when it is no longer required.
                //channel.ShutdownAsync().Wait();
                // Load an image from a local file
                var image = image_name;

                // create client, get response
                //var client = ImageAnnotatorClient.Create();


                var response = client.DetectLabels(image);



                foreach (var annotation in response)
                {
                    String current = annotation.Description;
                    if (current != null)
                    {
                        myList.Add(current.ToLower());
                    }
                }
                return myList;
            }

            //Console.WriteLine(annotation.Description);



            catch (Grpc.Core.RpcException ex)
            {
                System.Windows.Forms.MessageBox.Show("Authentication error with the API. Check your \"key_thing.json\" Close this program and try again");
                return myList;
            }
            catch (FileNotFoundException ex)

            {
                System.Windows.Forms.MessageBox.Show("\"key_thing.json\" Not found. Close this program and try again");
                return myList;

            }
        
            }
    }
}