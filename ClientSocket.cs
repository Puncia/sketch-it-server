using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace sketch_it_server
{    
    public class ClientSocket
    {
        const string EOF = "\n";

        struct StateObject
        {
            public Socket socket;
            public const int bufferSize = 1024;
            public byte[] buffer;
            public StringBuilder msg;
        }

        Socket s;

        public ClientSocket(Socket s)
        {
            this.s = s;
            StateObject sObject = new StateObject();
            sObject.buffer = new byte[StateObject.bufferSize];
            sObject.msg = new StringBuilder();

            s.BeginReceive(sObject.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReceiveCallback), sObject);
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            string content = string.Empty;
            StateObject state = (StateObject)ar.AsyncState;

            int bytesRead = s.EndReceive(ar);

            if(bytesRead > 0)
            {
                state.msg.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                content = state.msg.ToString();
                if(content.IndexOf(EOF) > -1)
                {
                    Console.WriteLine("{0} [{1}bytes]", content, content.Length);
                    state.msg.Clear();
                }

                s.BeginReceive(state.buffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
        }
    }
}
