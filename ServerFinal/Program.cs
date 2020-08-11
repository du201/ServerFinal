using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using NetMQ;
using NetMQ.Sockets;

namespace ServerFinal
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new ResponseSocket())
            {
                Console.WriteLine("Server Started");
                server.Bind("tcp://localhost:5555");
                string path = @"C:\yolo\pytorch-yolo-v3\receivedImage\image.jpg";
                
                while (true)
                {
                    byte[] image = server.ReceiveFrameBytes();
                    File.WriteAllBytes(path, image);

                    var psi = new ProcessStartInfo();
                    psi.FileName = @"C:\Python38\python.exe";
                    var script = @"C:\yolo\pytorch-yolo-v3\video_demo.py";
                    var type = "--video";
                    var target = path;
                    psi.Arguments = string.Format("\"{0}\" \"{1}\" \"{2}\"", script, type, target);

                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;

                    var errors = "";
                    var results = "";

                    using (var process = Process.Start(psi))
                    {
                        errors = process.StandardError.ReadToEnd();
                        results = process.StandardOutput.ReadToEnd();
                    }
                    Console.WriteLine("Received image and saved it");
                    server.SendFrame("Nice Job");
                }

            }

        }
    }
}
