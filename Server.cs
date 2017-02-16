using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.IO;

namespace sketch_it_server
{
    static class Server
    {
        const string LoginDBPath = "login.db";

        static ServerSocket server;

        static List<Client> clients;

        static FileStream loginDB;
        
        public static void Run()
        {
            server = new ServerSocket();
            clients = new List<Client>();

            loginDB = new FileStream(LoginDBPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            loginDB.Close();

            server.onConnection += Server_onConnection;

            server.StartListening();
        }

        private static void Server_onConnection(ClientSocket cs)
        {
            ClientProtocol cp = new ClientProtocol(cs);
            cp.onLogin += Cp_onLogin;
            cp.onRegister += Cp_onRegister;
            cp.onSNDMessage += cp_onSNDMessage;

            Console.WriteLine("New client connected.");
        }

        private static void cp_onSNDMessage(string Content, ClientSocket s)
        {
            GenericMessage receive_response = new GenericMessage();
            receive_response.command = ClientProtocol.RCV_MESSAGE;
            receive_response.parameters = new LobbyMessage(
                Content,
                s.Username,
                false);

            foreach (Client c in clients)
            {
                c.Send(JsonConvert.SerializeObject(receive_response));
            }            
        }

        private static bool Cp_onLogin(string Username, string Password, ClientSocket s)
        {
            string[] accounts = getAccounts();

            foreach(string userdata in accounts)
            {
                if(userdata.Length > 0)
                    if (userdata.Split(':')[0] == Username)
                    {
                        string pwd = userdata.Split(':')[1];
                        if (pwd == Password)
                        {
                            clients.Add(new Client(s));
                            return true;
                        }
                    }
            }
            
            foreach(Client c in clients)
            {
                if (c.Username == Username)
                    clients.RemoveAt(clients.IndexOf(c));
            }

            return false;
        }

        private static bool Cp_onRegister(string Username, string Password, string Email, string Language)
        {
            string[] accounts = getAccounts();

            foreach (string userdata in accounts)
            {
                if (userdata.Length > 0)
                {
                    if (userdata.Split(':')[0] == Username || userdata.Split(':')[2] == Email)
                        return false;
                }
            }
            
            registerAccount(Username, Password, Email, Language);

            return true;
        }

        private static void registerAccount(string Username, string Password, string Email, string Language)
        {
            loginDB = new FileStream(LoginDBPath, FileMode.Append, FileAccess.Write);
            string strAcc = Username + ':' + Password + ':' + Email + ':' + Language + '\n';
            byte[] newAccount = new byte[strAcc.Length];

            newAccount = Encoding.ASCII.GetBytes(strAcc);

            loginDB.Write(newAccount, 0, (int)newAccount.Length);
            loginDB.Close();
        }

        private static string[] getAccounts()
        {
            loginDB = new FileStream(LoginDBPath, FileMode.Open, FileAccess.Read);
            byte[] logins_raw = new byte[loginDB.Length];
            loginDB.Read(logins_raw, 0, (int)loginDB.Length);
            loginDB.Close();

            string logins_str = Encoding.ASCII.GetString(logins_raw);            

            return logins_str.Split('\n');            
        }
    }
}
