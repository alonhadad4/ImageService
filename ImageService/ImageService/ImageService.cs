﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Configuration;

namespace ImageService
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public partial class ImageService : ServiceBase
    {
        private int eventId = 1;
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);
        private ILogging logger;
        private ImageServer server;
        public EventHandler<MessageRecievedEventArgs> writeToLog;

        public ImageService()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("MySource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "MySource", "MyNewLog");
            }
            eventLog1.Source = GetAppSettings().Get("SourceName");
            eventLog1.Log = GetAppSettings().Get("LogName");
        }

        private static System.Collections.Specialized.NameValueCollection GetAppSettings()
        {
            return ConfigurationSettings.AppSettings;
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.  
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            this.writeToEventLogger("Service Started Pending");
            
            
            // Set up a timer to trigger every minute.  
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 10000; // 10 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
            this.writeToEventLogger("timer initialized successfully");

            // Update the service state to Running.  
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
            
            this.writeToEventLogger("service status set to running");

            this.logger = new LoggingModal();
            logger.MessageRecieved += onMsg;

            this.writeToEventLogger("logger set");
            //////////////////////////////////
            this.server = new ImageServer(this.logger, eventLog1);
            string handlerList = GetAppSettings().Get("Handler");
            string[] handlerListArray = handlerList.Split(';');
            for (int i = 0; i < handlerListArray.Length; i++)
            {
                this.server.CreateHandler(handlerListArray[i]);
            }
            this.writeToLog += this.server.onLogWrite;
            this.writeToEventLogger("Service Started");
        }

        public void writeToEventLogger(String message)
        {
            if (this.writeToLog != null)
            {
                this.writeToLog.Invoke(this, new MessageRecievedEventArgs( MessageTypeEnum.INFO, message));
            }
            this.eventLog1.WriteEntry(message);
        }
        
        public void onMsg(object sender, MessageRecievedEventArgs msgArgs)
        {
            this.writeToEventLogger(msgArgs.Message);
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            this.writeToEventLogger("Monitoring the System");
        }

        protected override void OnStop()
        {
            this.server.ServiceStopped(this, null);
            this.writeToEventLogger("Service Stopped");
        }

        protected override void OnContinue()
        {
            this.writeToEventLogger("In OnContinue.");
        }
    }
}
