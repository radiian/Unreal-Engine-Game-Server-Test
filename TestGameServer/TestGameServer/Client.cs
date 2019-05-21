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

            c_caller = caller;
            lastContact = DateTime.Now;
            Console.WriteLine("Client logged in and was assigned id " + c_id.ToString());
            listenThread = new Thread(new ThreadStart(this.ThreadLoop));
            listenThread.Start();
            byte[] c_idBuffer = BitConverter.GetBytes((Int32)c_id);
            byte[] cidMessage = new byte[6];
            cidMessage[0] = 0xAA;   //0xAA is binary 10101010, the indicator that we're sending the client their id
            //cidMessage.CopyTo(c_idBuffer, 1);
            c_idBuffer.CopyTo(cidMessage, 1);
            c_sock.Send(cidMessage);
            
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

        public void SendData(byte[] buffer, int length)
        {
            //This whole block is just for testing
            if (buffer[0] == 0x01)
            {
                byte[] sId = new byte[4];
                for (int i = 0; i < 4; ++i)
                {
                    sId[i] = buffer[i + 1];
                }
                
                int senderId = BitConverter.ToInt32(sId, 0);
                int datLen = buffer[5];
                if (datLen != length)
                {
                    Console.WriteLine("Data mismatch!");
                    Console.WriteLine("dat len is " + datLen.ToString() + " and full length is " + length.ToString());
                }

                //This was way harder than it should have been, honestly I don't know why I do this shit so late at night
                byte[] fixedMessage = new byte[length];
                for (int i = 0; i < datLen-6; ++i)
                {
                    char c = (char)buffer[i+6];
                    fixedMessage[i] = (byte)(c + 1);
                }
                //string test = Encoding.ASCII.GetString(buffer);
                string test = Encoding.UTF8.GetString(fixedMessage, 0, length - 6);
                Console.WriteLine("Sending message to client " + c_id.ToString() + ", from client " + senderId.ToString() + ": " + test);

                
            }
            if (c_sock.Connected)
            {
                c_sock.Send(buffer);
            }
            else
            {
                c_caller.closeClient(this);
            }
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

            byte packetId = buffer[0];

            switch (packetId)
            {
                case 0x01: c_caller.Chat(this, buffer, buffer[5]);
                    break;

                default: { }
                    break;
            }

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
