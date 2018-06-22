using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    public class ImageRecievedEventArgs : EventArgs
    {
        public byte[] Image { get; set; }
        public string Name { get; set; }
        public ImageRecievedEventArgs(byte[] img, String name)
        {
            this.Image = img;
            this.Name = name;
        }
    }
}
