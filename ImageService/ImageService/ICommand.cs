//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// ICommand Interface
    /// </summary>
    interface ICommand
    {
        /// <summary>
        /// execute a command given to it
        /// </summary>
        /// <param name="args"> the args the set the command</param>
        /// <param name="result"> the command result</param>
        /// <returns></returns>
        string Execute(string[] args, out bool result);
    }
}
