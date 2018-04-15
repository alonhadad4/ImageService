//Alon Hadad 204566871 & Jonathan Berger 308120930
using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageService
{
    /// <summary>
    /// DirectoryHandler class implementing IDirectoryHandler interface
    /// </summary>
    public class DirectoryHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILogging m_logging;
        private FileSystemWatcher m_dirWatcher;             // The Watcher of the Dir
        private string m_path;                              // The Path of directory
        #endregion
        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose;              // The Event That Notifies that the Directory is being closed

        /// <summary>
        /// A constructor for DirectoryHandler
        /// </summary>
        /// <param name="IController"></param>
        /// <param name="ILog"></param>
        public DirectoryHandler(IImageController IController, ILogging ILog)
        {
            m_controller = IController;
            m_logging = ILog;
        }
        /// <summary>
        /// Handaling a directory, setting a path andattaching to the listener
        /// </summary>
        /// <param name="dirPath"> dirPath is a path of directory that this handler listens to</param>
        public void StartHandleDirectory(string dirPath)
        {
            this.m_dirWatcher = new FileSystemWatcher();
            this.m_path = dirPath;
            m_dirWatcher.Path = dirPath;
            m_dirWatcher.Created += new FileSystemEventHandler(CreateThumbnail);
            m_dirWatcher.EnableRaisingEvents = true;
        }
        /// <summary>
        /// Creating a thumbnail directory if missing and adding a thumbnail and its nessecery folders
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public void CreateThumbnail(object source, FileSystemEventArgs e)
        {
            string strFileExt = Path.GetExtension(e.FullPath);    
            m_logging.Log(strFileExt, MessageTypeEnum.INFO);
            // filter file types ignoring folders 
            switch (strFileExt)
            {
                case @".jpg":
                    break;
                case @".png":
                    break;
                case @".bmp":
                    break;
                case @".gif":
                    break;
                default:
                    return;
            }
            m_logging.Log("started creating thumbnail", MessageTypeEnum.INFO);
            bool result;
            string [] args = new string[3];
            args[0] = e.FullPath;
            //creting a filestream in order to copy the file
            FileStream fs = new FileStream(e.FullPath, FileMode.Open, FileAccess.Read);
            try
            {
                //getting the time and date of the image
                DateTime dt = GetDateTakenFromImage(fs);
                m_logging.Log("date taken from image", MessageTypeEnum.INFO);
                
                args[1] = dt.Year.ToString();
                args[2] = dt.Month.ToString();

            } catch (Exception exce)
            {
                // in case the image has no date
                m_logging.Log("No Available Date", MessageTypeEnum.FAIL);
                args[1] = "Default";
                args[2] = "Default";
            }
            //closing the image
                fs.Close();
                fs.Dispose();
            try
            {
                //execute the command
                m_logging.Log(m_controller.ExecuteCommand(1, args, out result), MessageTypeEnum.INFO);
            } catch(Exception errors)
            {
                //incase there was an error int adding the file
                m_logging.Log(errors.ToString(), MessageTypeEnum.FAIL);
            }      
        }
        /// <summary>
        /// a listener the notices when recieved a message when a file has been added
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (!e.RequestDirPath.Equals(this.m_path))
                return;
            bool result;
            this.m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
            m_logging.Log(@"a command has been recieved and the result is of the transfer is:" + result, MessageTypeEnum.INFO);
        }
        /// <summary>
        /// Invoking the DirectoryCloseEvent closing the handlers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CloseHandler(object sender, CommandRecievedEventArgs e) {
            this.m_dirWatcher.Dispose();
            this.DirectoryClose.Invoke(this, new DirectoryCloseEventArgs(this.m_path, "closing handler"));
        }

        private static Regex r = new Regex(":");
        /// <summary>
        ///retrieves the datetime WITHOUT loading the whole image 
        /// </summary>
        /// <param name="fs">the file to be checked</param>
        public static DateTime GetDateTakenFromImage(FileStream fs)
        {
            fs.Position = 0;
            using (Image myImage = Image.FromStream(fs, false, false))
            {
                PropertyItem propItem = myImage.GetPropertyItem(36867);
                string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                return DateTime.Parse(dateTaken);
            }
        }
    }
}
