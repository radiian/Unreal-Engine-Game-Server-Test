using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace TestGameServer
{
    class ChatDat
    {
        public int clientId;
        public string message;

        public ChatDat(int i, string m)
        {
            clientId = i;
            message = m;
        }
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            string sqlStr = "server=10.0.0.119;user=testuser;database=testdb;port=3306;password=password";
            MySqlConnection dbConn = new MySqlConnection(sqlStr);
           /* try
            {
                dbConn.Open();
                Console.WriteLine("Connected to MySQL server");
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to connect to MySQL Server: " + ex.ToString());
            }
            dbConn.Close();*/
            Server testServer = new Server();
            Console.ReadLine();
        }
    }
}
