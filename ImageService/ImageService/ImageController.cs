//Alon Hadad 204566871 & Jonathan Berger 308120930
using ImageService.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        //Members
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands;
        /// <summary>
        /// ImageController constructor
        /// </summary>
        /// <param name="modal"> the service modal</param>
        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            NewFileCommand nfc = new NewFileCommand(m_modal);
            commands = new Dictionary<int, ICommand>()
            {
                {1, nfc}
            };
        }
        /// <summary>
        /// returning the command by id from the dictionary of commands
        /// </summary>
        /// <param name="commandID">the command id</param>
        /// <param name="args">the args of the command</param>
        /// <param name="resultSuccesful"> the result</param>
        /// <returns></returns>
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            return commands[commandID].Execute(args, out resultSuccesful);
        }
    }
}
