using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class ClientEventArgs : EventArgs
    {
        public int id { get; set; }
        public ClientEventArgs(int num)
        {
            this.id = num;
        }
    }
}
