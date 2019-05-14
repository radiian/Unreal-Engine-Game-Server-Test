using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace TestGameServer
{
    class Client
    {
        private Socket c_sock; //Client network socket
        private int c_id;   //Client network id
        private Server c_caller;    //the calling server

        private DateTime lastContact = new DateTime(2000, 1, 1, 0, 0, 0);   //a default value for safety

        Thread listenThread;    //The listener thread for the client

        public Client(int ID, Socket Sock, Server caller)
        {
            c_id = ID;
            c_sock = Sock;
            lastContact = DateTime.Now;
            Console.WriteLine("Client logged in and was assigned id " + c_id.ToString());
            listenThread = new Thread(new ThreadStart(this.ThreadLoop));
            listenThread.Start();
            
        }

        private void ThreadLoop()
        {
            //Wait for data
            while (true)
            {
                if (!c_sock.Connected)
                {
                    //socket disconnected
                    break;
                }
                int avail = c_sock.Available;
                while (avail > 0)
                {
                    byte[] buff = new byte[avail];
                    c_sock.Receive(buff);
                    //Ok now do something with the data
                    parseData(buff, avail);

                    avail = c_sock.Available;
                }
            }
            //Now inform the server that this client is closed
            c_caller.closeClient(this);
            c_sock.Close();
            listenThread.Join();
        }

        public void SendData(byte[] buffer, int size)
        {

        }

        public int getId()
        {
            return c_id;
        }

        //Parse the data received from this client
        private void parseData(byte[] buffer, int length)
        {

            //This is a really weird quirk of UE4's StringToBytes()
            //algorithm, all the characters need to be incremented by one
            for(int i = 0; i < length; ++i)
            {
                char c = (char)buffer[i];
                buffer[i] = (byte) (c + 1);
            }
            //string test = Encoding.ASCII.GetString(buffer);
            string test = Encoding.UTF8.GetString(buffer);
            Console.WriteLine("Received from client " + c_id.ToString() + ": " + test);
        }

        public void TestSend()
        {
            byte[] x = BitConverter.GetBytes((Int32)(-7470));
            byte[] y = BitConverter.GetBytes((Int32)(2980));
            byte[] z = BitConverter.GetBytes((Int32)(200));
            byte[] buff = new byte[4 * 3];
            x.CopyTo(buff, 0);
            y.CopyTo(buff, 4);
            z.CopyTo(buff, 8);
            if (c_sock.Connected)
            {
                c_sock.Send(buff);
            }
            Console.WriteLine("Sending test data");
        }
    }
}
