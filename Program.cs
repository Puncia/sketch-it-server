using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace sketch_it_server
{
    public class sketchit
    {
        public static int Main(string[] args)
        {
            ServerSocket socket = new ServerSocket();

            socket.onConnection += Socket_onConnection;
            socket.StartListening();

            return 0;
        }

        private static void Socket_onConnection(ClientSocket cs)
        {
            Console.WriteLine("Client connected.");
        }
    }
}

