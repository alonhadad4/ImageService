using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService
{
    class AppConfigAdapter
    {
        public static String getStringOfConfiguration()
        {
            String conf = ConfigurationSettings.AppSettings.Get("Handler");
            /*
            string[] handlerListArray = handlerList.Split(';');
            for (int i = 0; i < handlerListArray.Length; i++)
            {
                conf += handlerListArray[i] + "*";
            }
            */
            conf += "#";
            conf += ConfigurationSettings.AppSettings.Get("OutputDir") + "#";
            conf += ConfigurationSettings.AppSettings.Get("SourceName") + "#";
            conf += ConfigurationSettings.AppSettings.Get("LogName") + "#";
            conf += ConfigurationSettings.AppSettings.Get("ThumbnailSize");
            return conf;
        }

        public static List<string> getInitialLogs(ILogging logger)
        {
            List<string> logs = new List<string>();
            EventLog myLog = new EventLog();
            myLog.Log = ConfigurationSettings.AppSettings.Get("LogName");
            int i = 0;
            int lengthOfLogInLogs = 0, lengthOfCurrentLog;
            int size = myLog.Entries.Count;
            //foreach (EventLogEntry entry in myLog.Entries)
            for (int j = myLog.Entries.Count - 1; j >=0; j --)
            {
                String entry = AppConfigAdapter.changeToString(myLog.Entries[j]);
                //logger.Log("in first entry: {$entry.Message}", MessageTypeEnum.INFO);
                if (logs.Any()) // if logs isn't empty
                {
                    //lengthOfCurrentLog = entry.Message.Length;
                    lengthOfCurrentLog = myLog.Entries[j].Message.Length;
                    if (lengthOfLogInLogs + lengthOfCurrentLog > 100)
                    { // if sending two logs together is too big
                        i++;
                        logs.Add(entry);
                        lengthOfLogInLogs = lengthOfCurrentLog;
                    } else // sum of lengths <= 100
                    { // concatenate last string with current string
                        //String temp = String.Concat("*", entry);
                        //logs[i] = String.Concat(logs[i], temp);
                        logs[i] += "*" + entry;
                        lengthOfLogInLogs += lengthOfCurrentLog;
                    }
                } else // list is still empty
                {
                    logs.Add(entry);
                    lengthOfLogInLogs = myLog.Entries[j].Message.Length;
                }
            }
            return logs;
        }

        private static String changeToString (EventLogEntry e)
        {
            String temp = e.EntryType + ";" + e.Message;
            return temp;
        }
    }
}
