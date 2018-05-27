//Alon Hadad 204566871 & Jonathan Berger 308120930
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService;

/// <summary>
/// LoggingModal class implementing the ILogging interface 
/// </summary>
public class LoggingModal : ILogging
{
    // a on massage event
    public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
    /// <summary>
    /// controlling the logs of the service
    /// </summary>
    public void Log(string message, MessageTypeEnum type)
    {
        MessageRecievedEventArgs mArgs = new MessageRecievedEventArgs(type, message);
        MessageRecieved.Invoke(this, mArgs);
    }
}