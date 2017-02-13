using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace sketch_it_server
{
    public class ClientProtocol : ClientSocket
    {
        public delegate bool LoginResultHandler(string Username, string Password);
        public event LoginResultHandler onLogin;

        const string LOGIN = "Authentication/Login";

        public ClientProtocol(ClientSocket cp) : base(cp)
        {
            cp.onMessageReceive += Cp_onMessageReceive;
        }

        private void Cp_onMessageReceive(string message)
        {
            JObject o = JObject.Parse(message);
            string cmd = (string)o["command"];

            switch(cmd)
            {
                case LOGIN:

                    if (onLogin == null)
                        return;

                    if(onLogin((string)o["Username"], (string)o["Password"]))
                    {
                        GenericMessage response = new GenericMessage();
                        response.command = LOGIN;
                        response.parameters = new Response(true);

                        Send(JsonConvert.SerializeObject(response));
                    }
                    break;
            }
        }
    }
}
