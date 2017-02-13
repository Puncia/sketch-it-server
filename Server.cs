using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sketch_it_server
{
    static class Server
    {
        static ServerSocket server;

        static List<Client> clients;
        
        public static void Run()
        {
            server = new ServerSocket();
            clients = new List<Client>();
            
            server.onConnection += Server_onConnection;

            server.StartListening();

        }

        private static void Server_onConnection(ClientSocket cs)
        {
            ClientProtocol cp = new ClientProtocol(cs);
            cp.onLogin += Cp_onLogin;

            Console.WriteLine("New client connected.");

            //clients.Add((Client)cs);
        }

        private static bool Cp_onLogin(string Username, string Password)
        {
            return true;
        }
    }
}
