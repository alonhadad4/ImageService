//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// ImageService
    /// </summary>
    public interface IImageServiceModal
    {
        string AddFile(string[] args,out bool result);
    }
}
