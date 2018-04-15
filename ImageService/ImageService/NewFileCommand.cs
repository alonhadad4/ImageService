//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Commands
{
    /// <summary>
    /// NewFileCommand class implementing the ICommand
    /// </summary>
    public class NewFileCommand : ICommand
    {
        //Members
        private IImageServiceModal m_modal;
        /// <summary>
        /// storing the modal
        /// </summary>
        /// <param name="modal"></param>
        public NewFileCommand(IImageServiceModal modal)
        {
            m_modal = modal;            
        }
        /// <summary>
        /// Executing the command add file
        /// </summary>
        /// <param name="args">args for the command</param>
        /// <param name="result">result of the command</param>
        /// <returns></returns>
        public string Execute(string[] args, out bool result)
        {
            try
            {
                return m_modal.AddFile(args, out result);
            } catch (Exception e)
            {
                result = false;
                return e.ToString();
            }
        }
    }
}
