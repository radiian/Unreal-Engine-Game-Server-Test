using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace TestGameServer
{
    class Server
    {
        private List<Client> clients;

        private List<ChatDat> chatLog;

        public Server()
        {
            Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 65500);
            s.Bind(ipe);
            s.Listen(10);
            Console.WriteLine("Listening");

            clients = new List<Client>();

            chatLog = new List<ChatDat>();

            Int32 ccid = 1; //Current client id to hand out to connecting clients

            while (true)
            {
                //Socket client = s.Accept();
                Client newClient = new Client(ccid++, s.Accept(), this);
                clients.Add(newClient);
                //newClient.TestSend();


         
                Console.WriteLine("Socket accepted!");

            }
        }

        public void Chat(Client C, string message)
        {
            foreach(Client c_ in clients)
            {
                if(c_.getId() != C.getId())
                {
                    //c_.SendData()
                }
            }
            //chatLog.Add(new ChatDat(C.getId(), message));
        }

        public void closeClient(Client C)
        {
            //This may not be necessary, but just in case
            Client cc = clients.Find(c => c.getId() == C.getId());
            clients.Remove(cc); 
        }
    }
}
