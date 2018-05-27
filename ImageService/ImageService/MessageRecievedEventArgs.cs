//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// MessageRecievedEventArgs class implementing EventArgs
/// </summary>
namespace ImageService
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public MessageTypeEnum Status { get; set; }
        public string Message { get; set; }
        public MessageRecievedEventArgs(MessageTypeEnum id, string message)
        {
            this.Status= id;
            this.Message = message;
        }
    }
}
