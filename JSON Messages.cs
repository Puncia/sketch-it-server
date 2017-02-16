using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sketch_it_server
{
    class GenericMessage
    {
        [JsonProperty]
        public string command { get; set; }

        [JsonProperty]
        public object parameters { get; set; }
    }
    
    class Response
    {
        public Response(bool response)
        {
            this.response = response;
        }

        [JsonProperty]
        public bool response { get; set; }

        [JsonProperty]
        public string error { get; set; }
    }
    
    class LobbyMessage
    {
        public LobbyMessage(string content, string user, bool system)
        {
            this.content = content;
            this.user = user;
            this.system = system;
        }

        [JsonProperty]
        public string content;

        [JsonProperty]
        public string user;

        [JsonProperty]
        public bool system;
    }
}
