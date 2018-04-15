//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Modal
{
    /// <summary>
    /// DirectoryCloseEventArgs class implementing EventArgs interface
    /// </summary>
    public class DirectoryCloseEventArgs : EventArgs
    {
        //Members
        public string DirectoryPath { get; set; }
        public string Message { get; set; }             // The Message That goes to the logger
        /// <summary>
        /// A constructor for DirectoryCloseEventArgs
        /// </summary>
        /// <param name="dirPath">directory path</param>
        /// <param name="message">a string</param>
        public DirectoryCloseEventArgs(string dirPath, string message)
        {
            DirectoryPath = dirPath;                    // Setting the Directory Name
            Message = message;                          // Storing the String
        }

    }
}
