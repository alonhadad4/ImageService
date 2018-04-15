using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LoggingModal : ILogging
{
    public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
    public void Log(string message, MessageTypeEnum type)
    {
        MessageRecieved.Invoke(this, new MessageRecievedEventArgs(message, type));
    }
}