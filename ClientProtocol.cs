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
        public delegate bool LoginResultHandler(string Username, string Password, ClientSocket s);
        public event LoginResultHandler onLogin;

        public delegate bool RegisterResultHandler(string Username, string Password, string Email, string Language);
        public event RegisterResultHandler onRegister;

        public delegate void LobbySNDMessageEventHandler(string Content, ClientSocket s);
        public event LobbySNDMessageEventHandler onSNDMessage;

        public const string LOGIN = "Authentication/Login";
        public const string REGISTER = "Authentication/Register";
        public const string SND_MESSAGE = "Lobby/SendMessage";
        public const string RCV_MESSAGE = "Lobby/ReceiveMesage";
        
        public ClientProtocol(ClientSocket cp) : base(cp)
        {
            cp.onMessageReceive += Cp_onMessageReceive;
        }

        private void Cp_onMessageReceive(string message)
        {
            try
            {
                JObject o = JObject.Parse(message);
                string cmd = (string)o["command"];

                switch (cmd)
                {
                    case LOGIN:
                        if (onLogin == null)
                            return;

                        GenericMessage login_response = new GenericMessage();
                        login_response.command = LOGIN;

                        Username = (string)o["parameters"]["username"];

                        login_response.parameters = new Response(onLogin(
                            (string)o["parameters"]["username"],
                            (string)o["parameters"]["password"],
                            this));
                        
                        Send(JsonConvert.SerializeObject(login_response));

                        break;

                    case REGISTER:
                        if (onRegister == null)
                            return;

                        GenericMessage register_response = new GenericMessage();
                        register_response.command = REGISTER;
                        register_response.parameters = new Response(onRegister(
                            (string)o["parameters"]["username"],
                            (string)o["parameters"]["password"],
                            (string)o["parameters"]["email"],
                            (string)o["parameters"]["language"]["name"]));

                        Send(JsonConvert.SerializeObject(register_response));
                        break;

                    case SND_MESSAGE:
                        if (onSNDMessage == null)
                            return;
                        else onSNDMessage(
                            (string)o["parameters"]["content"],
                            this);
                        break;
                }
            }
            catch (JsonReaderException e) { }
        }
    }
}
