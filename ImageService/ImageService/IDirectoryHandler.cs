//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Modal;

namespace ImageService
{
    /// <summary>
    /// IDirectoryHandler interface
    /// </summary>
    public interface IDirectoryHandler
    {
        event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed
        void StartHandleDirectory(string dirPath);             // The Function Recieves the directory to Handle
        void OnCommandRecieved(object sender, CommandRecievedEventArgs e);     // The Event that will be activated upon new Command
        void CloseHandler(object sender, CommandRecievedEventArgs e);         // handling closing the handlers
    }
}

