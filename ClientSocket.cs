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
        public delegate void ClientMessageHandler(string message);
        public event ClientMessageHandler onMessageReceive;

        const char EOF = (char)4;

        struct StateObject
        {
            public Socket socket;
            public const int bufferSize = 1024;
            public byte[] tmpBuffer;
            public StringBuilder msgBuffer;
            public uint msgLength;
        }

        Socket s;

        public ClientSocket(Socket s)
        {
            onMessageReceive += MessageReceived;

            this.s = s;
            StateObject sObject = new StateObject();
            sObject.tmpBuffer = new byte[StateObject.bufferSize];
            sObject.msgBuffer = new StringBuilder();
            
            s.BeginReceive(sObject.tmpBuffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReceiveCallback), sObject);
        }

        public ClientSocket(ClientSocket s)
        {
            this.s = s.s;
        }

        public void ReceiveCallback(IAsyncResult ar)
        {
            string content = string.Empty;
            StateObject state = (StateObject)ar.AsyncState;

            int bytesRead = s.EndReceive(ar);

            if(bytesRead > 0)
            {
                state.msgBuffer.Append(Encoding.ASCII.GetString(
                    state.tmpBuffer, 0, bytesRead));

                content = state.msgBuffer.ToString();

                int occ = 0;
                int i = 0;

                for ( ; i < content.Length; ++i)
                {
                    if (content[i] == EOF)
                    {
                        if (occ == 0)
                        {
                            content = content.Remove(i, 1);
                            i--;
                        }

                        occ++;
                    }
                    else
                    {
                        if (occ == 1)
                        {
                            //Message received
                            onMessageReceive?.Invoke(content.Substring(0, i));

                            content = content.Substring(i, content.Length - i);
                            i = 0;
                        }

                        occ = 0;
                    }
                }

                if (occ == 1)
                {
                    //Message received
                    onMessageReceive?.Invoke(content.Substring(0, i));

                    content = content.Substring(i, content.Length - i);
                    i = 0;
                }

                state.msgBuffer.Clear();
                state.msgBuffer.Append(content);

                s.BeginReceive(state.tmpBuffer, 0, StateObject.bufferSize, 0, new AsyncCallback(ReceiveCallback), state);
            }
        }

        public void Send(string msg)
        {
            //String EOF parsing
            bool seq = false;
            for(int i = 0; i < msg.Length; i++)
            {
                if (msg[i] == EOF)
                {
                    if (!seq)
                    {
                        msg.Insert(i, EOF.ToString());
                    }
                    seq = true;
                }
                else seq = false;
            }

            s.BeginSend(Encoding.ASCII.GetBytes(msg + EOF), 0, msg.Length + 1, 0, SentCallback, s);
        }

        public void SentCallback(IAsyncResult ar)
        {

        }

        public void MessageReceived(string msg)
        {
           
        }
    }
}
