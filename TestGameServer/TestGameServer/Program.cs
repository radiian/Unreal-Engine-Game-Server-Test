using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;

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

            /*
             * Super important note: 
             * This is all very experimental. This whole project is an experiment in writing custom
             * server software for specialized Unreal Engine projects and is bound to change often
             * use sometimes strange methods.
             * 
             * The SQL Connector code below is an effort in experimenting and trying to learn best
             * practices and the nuances of the connector library. The server that this is connecting
             * to is NOT a production database, it houses no production databases, and no sensitive information.
             * All data, usernames, passwords, etc. are purely for testing and experimenting should be replaced
             * with much more secure ones for a live system.
             * 
             * You have been warned, testuser and password are definitly not good credentials!
             * 
             */



            //Using hardcoded IP addresses can be a real pain for software sometimes.
            //The Domain Name System makes it easy to connect to services via a hostname
            //instead of a squishy IP Address. This little snippet of code gets the ip
            //address of the database server based on the host name of the database server.
            //Note that srvAddr is instantiated as null and won't cause problems because
            //if the server is not found, the application will throw an error and exit before
            //the address is ever used. Otherwise srvAddr is replaced with the appropriate
            //address for the server.
            IPAddress srvAddr = null;
            try
            {
                //Resolve a list of IPAddresses from a singular hostname (can have multiple ip addresses in a DNS record)
                IPAddress[] addresses = Dns.GetHostAddresses("database.lab.local");

                //If there are none returned, we error (even though if the host lookup fails it will throw an error anyways)
                if (addresses.Length > 0)
                {
                    //set srvAddr to the first address in the list
                    srvAddr = addresses[0];
                }
                else
                {
                    Console.WriteLine("Could not resolve host name for database server!");
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Could not resolve host name for database server!");
                Console.ReadLine();
                Environment.Exit(-1);
            }
            

            //SQL Server connection string
            //This particular server is not running on the development machine which is why we are not using 127.0.0.1 for
            //the ip address. For something like this, a remote SQL server may be best, may not be.
            string sqlStr = "server="+ srvAddr.ToString() +";user=testuser;database=testdb;port=3306;password=password";

            //The actual database connection object
            MySqlConnection dbConn = new MySqlConnection(sqlStr);

            //Try and catch is a good lazy way of catching errors during experimentation
            try
            {
                //Try and connect to the server specified above
                dbConn.Open();

                //And print a message if successful
                Console.WriteLine("Connected to MySQL server");


                //A SQL query/command string
                //A great example is selecting a singular user, but this one gets all of them for testing
                string qry = "SELECT * FROM Users";
                //string qry = "SELECT Password FROM Users WHERE Username='TestUser'";

                //Create a command object to execute the command
                MySqlCommand cmd = new MySqlCommand(qry, dbConn);

                //MySqlCommand has multiple execution methods:
                //ExecuteReader(), ExecuteNonQuery(), and ExecuteScalar()
                //See https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-sql-command.html for better documentation
                //For this example we want a MySqlDataReader 
                MySqlDataReader rdr = cmd.ExecuteReader();

                //object result = cmd.ExecuteScalar();
                //if(result != null)
                //{
                //    Console.WriteLine(result.ToString());
                //}


                //One of the most useful things to do with a database query is to determine how many rows were returned
                //This is a clever way to determine how many rows there are, and create useful data management objects
                //at the same time.
                
                //Create a DataTable to contain the results of the query
                DataTable rows = new DataTable();

                //Load the data from the DataReader into the DataTable
                rows.Load(rdr);


                //Then we can actually get the number of rows returned!
                Console.WriteLine("Collected " + rows.Rows.Count.ToString() + " rows from database");

                //Then we can iterate through the rows and do what we need
                for(int i = 0; i < rows.Rows.Count; ++i)
                { 

                    //And this is not a useful line of code because it doesnt work, but you get the idea
                    
                    Console.WriteLine(rows.Rows[i].ToString());
                }

                //Then close the reader to free the memory
                rdr.Close();

                //and dispose of the rows object to free the memory
                rows.Dispose();

            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to connect to MySQL Server: " + ex.ToString());
            }
            dbConn.Close();
            Server testServer = new Server();
            Console.ReadLine();
        }
    }
}
