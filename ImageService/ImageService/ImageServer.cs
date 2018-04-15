//Alon Hadad 204566871 & Jonathan Berger 308120930
using ImageService.Commands;
using ImageService.Controller;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    /// <summary>
    /// ImageServer class
    /// creating and handeling a server ,his handles and listeners events
    /// </summary>
    public class ImageServer
    {
        private ILogging logger;
        private IImageController imageController;
        public event EventHandler<CommandRecievedEventArgs> onCommand;
        public event EventHandler<CommandRecievedEventArgs> informHandlerClose;
        private Dictionary<int, ICommand> commands;

        /// <summary>
        /// a constructor fore the server, setting a logger, imageservice model and an imange controller,
        /// also a dictionary of commans
        /// </summary>
        /// <param name="m_logging"></param>
        public ImageServer(ILogging m_logging)
        {
            this.logger = m_logging;
            ImageServiceModal iModal = new ImageServiceModal();
            this.imageController = new ImageController(iModal);
            Dictionary<int, ICommand> commandsList = new Dictionary<int, ICommand>()
            {
                {1, new NewFileCommand(iModal)}
            };
        }
        /// <summary>
        /// Creating handlers and adding them to the event listener
        /// </summary>
        /// <param name="directory"> the handler directory</param>
        public void CreateHandler(string directory)
        {
            IDirectoryHandler handler = new DirectoryHandler(this.imageController, this.logger);
            handler.StartHandleDirectory(directory);
            onCommand += handler.OnCommandRecieved;
            informHandlerClose += handler.CloseHandler;
            handler.DirectoryClose += OnCloseServer;
        }
        /// <summary>
        /// Handling the close of the server
        /// </summary>
        /// <param name="sender"> the handler most likely</param>
        /// <param name="args"> arguments</param>
        public void OnCloseServer (object sender, DirectoryCloseEventArgs args)
        {
            IDirectoryHandler handler = (IDirectoryHandler)sender;
            onCommand -= handler.OnCommandRecieved;
            informHandlerClose -= handler.CloseHandler;
            handler.DirectoryClose -= OnCloseServer;
        }
        /// <summary>
        /// invoking the informHandlerClose of each handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ServiceStopped (object sender, CommandRecievedEventArgs args)
        {
            informHandlerClose.Invoke(this, args);
        }


    }
}
