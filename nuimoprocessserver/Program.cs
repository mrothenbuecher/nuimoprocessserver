using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nuimoprocessserver
{
    static class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        static JObject GetActiveProcessFileName()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            Process p = Process.GetProcessById((int)pid);

            JObject ob = new JObject();
            ob["process"] = p.ProcessName;
            ob["windowtitle"] = p.MainWindowTitle;

            return ob;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");

            int port = 1337;
            TcpListener listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();

            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("client connected");
            NetworkStream stream = client.GetStream();
            StreamWriter writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            StreamReader reader = new StreamReader(stream, Encoding.ASCII);

            while (true)
            {
                string inputLine = "";
                try
                {
                    while (inputLine != null)
                    {
                        inputLine = reader.ReadLine();
                        writer.WriteLine(GetActiveProcessFileName().ToString(Formatting.None));
                        Console.WriteLine("Echoing string: " + GetActiveProcessFileName().ToString());
                    }
                }
                catch (Exception ex) { Console.WriteLine("Server saw disconnect from client: "+ex.Message); }
                
                listener.Stop();
                Console.WriteLine("Starting server again...");

                listener = new TcpListener(IPAddress.Loopback, port);
                listener.Start();

                client = listener.AcceptTcpClient();
                Console.WriteLine("client connected");
                stream = client.GetStream();
                writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
                reader = new StreamReader(stream, Encoding.ASCII);
            }
        }
    }
}
