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
        public static int containsElement(Image image_name, String query)
        {
            //setting up the Google Credentials
            GoogleCredential cred = GoogleCredential.FromFile("key_thing.json");
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

            // loop through each returned label, check for match with query
            foreach (var annotation in response)
            {
                String current = annotation.Description;
                if (current != null)
                {
                    if (current.ToLower().Equals(query.ToLower()))
                    {
                        return 1; // true
                    }/
                }
                //Console.WriteLine(annotation.Description);

            }
            return 0; // false
        }
    }
}