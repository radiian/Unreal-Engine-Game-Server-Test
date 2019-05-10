using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace TestGameServer
{
    class Program
    {
        static void Main(string[] args)
        {

            Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 65500);
            s.Bind(ipe);
            s.Listen(10);
            Console.WriteLine("Listening");

            while (true)
            {
                Socket client = s.Accept();


                /*
                 * Spawn test:
                 * send 3, 4 byte integers in order: X, Y, Z to spawn an object
                 * 
                 */

                byte[] x = BitConverter.GetBytes((Int32)(-7470));
                byte[] y = BitConverter.GetBytes((Int32)(2980));
                byte[] z = BitConverter.GetBytes((Int32)(200));
                byte[] buff = new byte[4 * 3];
                x.CopyTo(buff, 0);
                y.CopyTo(buff, 4);
                z.CopyTo(buff, 8);
                client.Send(buff);
                StringBuilder hex = new StringBuilder(4 * 3 * 2);
                foreach (byte b in buff)
                {
                    hex.AppendFormat("{0:x2} ", b);
                }
                Console.WriteLine(hex.ToString());
                Console.WriteLine("Socket accepted!");

            }
            Console.ReadLine();
        }
    }
}
