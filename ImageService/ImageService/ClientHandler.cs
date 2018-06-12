using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImageService
{
    public class ClientHandler : IClientHandler
    {
        private NetworkStream stream;
        private BinaryReader reader;
        private BinaryWriter writer;
        public bool hasSentInitial = false;
        private bool sentSettings = false;
        private bool isListening = false;
        private int id;
        private static Mutex mutex;
        public EventHandler<MessageRecievedEventArgs> gotNewMessage;
        public EventHandler<MessageRecievedEventArgs> InitialWritingHappened;
        public EventHandler<ClientEventArgs> settingsWritten;
        public ClientHandler(Mutex mut, int id)
        {
            this.id = id;
            mutex = mut;
        }
        public void setStream(TcpClient client)
        {
            this.stream = client.GetStream();
            this.reader = new BinaryReader(stream);
            this.writer = new BinaryWriter(stream);
        }

        public void HandleClient(TcpClient client)
        {
            return;
        }

        public int writeStringToClient(String str)
        {
            int result = 0;
            new Task(() => {
                    try
                    {
                        mutex.WaitOne();
                        this.writer.Write(str);
                        this.writer.Flush();
                        mutex.ReleaseMutex();
                        result = 1;
                        if (this.settingsWritten != null)
                        {
                            this.settingsWritten.Invoke(this, new ClientEventArgs(this.id));
                        }
                    } catch (Exception e)
                    {
                        result = -1;
                        writer.Flush();
                        throw new IOException();
                    }
                    
            }).Start();
            return result;
        }

        public bool settingsSent()
        {
            return this.sentSettings;
        }
        /*
        private string ExecuteCommand(String commandLine, TcpClient client)
        {

        }*/
        public int writeListOfStringsToClient(List<String> list, ILogging logger)
        {
            int result = 0;
            new Task(() => {
                try
                {
                    //logger.Log("starting to write logs to client", MessageTypeEnum.INFO);
                    mutex.WaitOne();
                    this.writer.Write(list.Count);
                    this.writer.Flush();
                    //logger.Log("wrote number to client", MessageTypeEnum.INFO);
                    foreach (String item in list)
                    {
                        //logger.Log($"in loop of writing logs to client: {item}", MessageTypeEnum.INFO);
                        this.writer.Write(item);
                        this.writer.Flush();
                    }
                    logger.Log("wrote logs to client", MessageTypeEnum.INFO);
                    mutex.ReleaseMutex();
                    if (this.InitialWritingHappened != null)
                    {
                        this.InitialWritingHappened.Invoke(this, new MessageRecievedEventArgs(MessageTypeEnum.INFO, ""));
                        logger.Log("initial writing invoked", MessageTypeEnum.INFO);
                    }
                    result = 1;
                    this.hasSentInitial = true;
                }
                catch (Exception e)
                {
                    result = -1;
                    logger.Log("coludn't write logs to client", MessageTypeEnum.FAIL);
                    writer.Flush();
                    throw new IOException();
                }

            }).Start();
            return result;
        }

        public void Listen()
        {
            if (isListening)
            {
                return;
            }
            isListening = true;
            new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        String message = this.reader.ReadString();
                        MessageRecievedEventArgs args = this.getArgsFromString(message);

                        if (this.gotNewMessage != null)
                        {
                            this.gotNewMessage.Invoke(this, args);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new IOException();
                    }
                }
            }).Start();
        }

        private MessageRecievedEventArgs getArgsFromString(String message)
        {
            String[] args = message.Split(';');
            try
            {
                String toSend = args[1] + ";" + args[2];
                return new MessageRecievedEventArgs((MessageTypeEnum)Int32.Parse(args[0]), toSend);
            }
            catch (Exception e)
            {

            }
            return new MessageRecievedEventArgs(0, "");
        }

    }
}
