using System;
using System.Collections.Generic;
using System.Text;
using Landis.Library.Metadata;

namespace Landis.Library.HarvestManagement
{
    public static class Log
    {
        /// <summary>
        /// A copy of the event log for harvest
        /// </summary>
        public static MetadataTable<EventsLog> eventLog = null;

        /// <summary>
        /// A copy of the summary log for harvest
        /// </summary>
        public static MetadataTable<SummaryLog> summaryLog = null;

        /// <summary>
        /// Initializes the log
        /// </summary>
        public static void Initialize(string eventLogName, string summaryLogName)
        {
            eventLog = new MetadataTable<EventsLog>(eventLogName);
            summaryLog = new MetadataTable<SummaryLog>(summaryLogName);
        }

        public static void WriteEvents(EventsLog log)
        {
            if (eventLog == null)
            {
                return;
            }
            eventLog.Clear();
            eventLog.AddObject(log);
            eventLog.WriteToFile();
        }

        public static void WriteSummary(SummaryLog log)
        {
            if (summaryLog == null)
            {
                return;
            }
            summaryLog.Clear();
            summaryLog.AddObject(log);
            summaryLog.WriteToFile();
        }
    }
}
