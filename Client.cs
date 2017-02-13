using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sketch_it_server
{
    class Client : ClientProtocol
    {
        public Client(ClientSocket s) : base(s)
        {

        }
    }
}
