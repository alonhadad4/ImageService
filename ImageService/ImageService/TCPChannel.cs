using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ImageService
{
    public class TCPChannel
    {
        public event EventHandler<MessageRecievedEventArgs> GotCommand;
        public event EventHandler<EventArgs> startSendingLogs;
        private int port;
        private TcpListener listener;
        //private ClientHandler ch;
        private static TCPChannel instance;
        private ILogging logger;
        private System.Diagnostics.EventLog offLogger;
        private bool didSentInitial = false;
        private List<TcpClient> clients = new List<TcpClient>();
        private List<ClientHandler> clientHandlers = new List<ClientHandler>();
        private Dictionary<ClientHandler, int> clientIDs = new Dictionary<ClientHandler, int>();
        private int count = 0;
        private static Mutex MutexOfCount = new Mutex();
        private static Mutex mut = new Mutex();
        private TCPChannel(int port, ILogging logger, System.Diagnostics.EventLog offLogger)
        {
            this.port = port;
            //this.ch = ch;
            //this.ch.InitialWritingHappened += StartListening;
            //this.ch.InitialWritingHappened += StartWritingLogs;
            this.logger = logger;
            this.offLogger = offLogger;
        }

        public static TCPChannel Instance(int port, ILogging logger, System.Diagnostics.EventLog offLogger)
        {
            if (instance == null)
            {
                instance = new TCPChannel(port, logger, offLogger);
            }
            return instance;
        }

        public void Start()
        {
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            listener = new TcpListener(ep);
            listener.Start();
            //Console.WriteLine("Waiting for connections...");
            Task task = new Task(() => {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        ClientHandler ch = new ClientHandler(mut, count);
                        ch.settingsWritten += sendInitialLogs;
                        MutexOfCount.WaitOne();
                        count++;
                        MutexOfCount.ReleaseMutex();
                        clientHandlers.Add(ch);
                        clientIDs.Add(ch, count - 1);
                        ch.setStream(client);
                        ch.InitialWritingHappened += StartListening;
                        ch.InitialWritingHappened += StartWritingLogs;
                        //Console.WriteLine("Got new connection");
                        //this.logger.Log("about to get Initial logs", MessageTypeEnum.INFO);

                        String settings = AppConfigAdapter.getStringOfConfiguration();
                        ch.writeStringToClient(settings);
                    }
                    catch (Exception)
                    {
                        logger.Log("couldn't send settings or set client", MessageTypeEnum.FAIL);
                    }
                }

            });
            task.Start();
        }

        public void sendInitialLogs(object sender, ClientEventArgs e)
        {
            List<String> logs = AppConfigAdapter.getInitialLogs(this.logger);
            Task task = new Task(() =>
            {
                foreach (ClientHandler ch in this.clientHandlers)
                {
                    //if ((ClientHandler)sender == ch)
                    if (this.clientIDs[ch] == e.id)
                    {
                        try
                        {
                            ch.writeListOfStringsToClient(logs, this.logger);
                        }
                        catch (Exception ex)
                        {
                            logger.Log("couldn't write logs to client", MessageTypeEnum.FAIL);
                            mut.WaitOne();
                            this.clientHandlers.Remove(ch);
                            this.clientIDs.Remove(ch);
                            mut.ReleaseMutex();
                        }
                    }
                }
            });
            task.Start();
        }

        public void StartListening(object sender, EventArgs e)
        {
            this.didSentInitial = true;
            /*
            foreach(ClientHandler ch in this.clientHandlers)
            {
                if (ch.hasSentInitial)
                {
                    ch.gotNewMessage += updateServer;
                    ch.Listen();
                }
            }
            */
        }

        public void updateServer(object Sender, MessageRecievedEventArgs args)
        {
            this.GotCommand.Invoke(Sender, args);
        }

        public void StartWritingLogs(object sender, EventArgs e)
        {
            if (this.startSendingLogs != null)
            {
                //logger.Log("in channel, set to start sending logs", MessageTypeEnum.INFO);
                this.startSendingLogs.Invoke(sender, e);
            }
        }

        public void sendCloseHandler(String name)
        {
            String message = "Close:" + name;
            foreach (ClientHandler ch in this.clientHandlers)
            {
                if (ch.hasSentInitial)
                {
                    try
                    {
                        ch.writeStringToClient(message);
                    }
                    catch (Exception e)
                    {
                        mut.WaitOne();
                        this.clientHandlers.Remove(ch);
                        this.clientIDs.Remove(ch);
                        mut.ReleaseMutex();
                    }
                }
            }         
        }

        public void sendLogMessage(object sender, MessageRecievedEventArgs eventArgs)
        {
            //logger.Log("in send Log Message to GUI with message: " + eventArgs.Message, MessageTypeEnum.INFO);
            /*
            if (!this.didSentInitial)
            {
                return;
            }
            */
            //logger.Log("in send Log Message to GUI, about to send", MessageTypeEnum.INFO);
            //offLogger.WriteEntry("in send Log Message to GUI, about to send message: " + eventArgs.Message);
            String toSend = CommandEnum.LogCommand.ToString() + ";" + eventArgs.Status+ ";" + eventArgs.Message;
            foreach (ClientHandler ch in this.clientHandlers)
            {
                if (ch.hasSentInitial)
                {
                    try
                    {
                        ch.writeStringToClient(toSend);
                    }
                    catch (Exception e)
                    {
                        logger.Log("couldn't write: " + toSend + " to client", MessageTypeEnum.FAIL);
                        mut.WaitOne();
                        this.clientHandlers.Remove(ch);
                        this.clientIDs.Remove(ch);
                        mut.ReleaseMutex();
                    }  
                }
            }
            //this.ch.writeStringToClient(toSend);
            offLogger.WriteEntry("in send Log Message to GUI, message was sent: " + eventArgs.Message);
        }

        public bool hasSentInitial ()
        {
            return this.didSentInitial;
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}
