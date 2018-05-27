//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// CommandRecievedEventArgs Class implements EventArgs
    /// </summary>
    public class CommandRecievedEventArgs : EventArgs
    {
        // Members
        public int CommandID { get; set; }      // The Command ID
        public string[] Args { get; set; }
        public string RequestDirPath { get; set; }  // The Request Directory
        /// <summary>
        /// A constructor for command recieved event args
        /// </summary>
        /// <param name="id"> id of the command</param>
        /// <param name="args"> the arguments needed in order to run the command</param>
        /// <param name="path">a path</param>
        public CommandRecievedEventArgs(int id, string[] args, string path)
        {
            CommandID = id;
            Args = args;
            RequestDirPath = path;
        }
    }
}
