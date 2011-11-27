using System;
using System.Collections.Generic;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using MongoDB;
using MongoDB.Configuration;
using MongoDB.Linq;

namespace BPMonitor_Service
{
    public partial class MonitorService : ServiceBase
    {
        /////////////////////////////////////////////////
        // CONSTANT VARIABLES
        /////////////////////////////////////////////////

        private const string sourceName = "BPMEvents";

        public MonitorService()
        {
            InitializeComponent();
            
            /////////////////////////////////////////////////
            // CREATE EVENT LOGS
            /////////////////////////////////////////////////

            if (!System.Diagnostics.EventLog.SourceExists(sourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(sourceName, "Application");
            }

            this.BPM_EventLog.Source = sourceName;
            this.BPM_EventLog.WriteEntry("Service initialized");

        }

        /////////////////////////////////////////////////
        // START SERVICE
        /////////////////////////////////////////////////

        protected override void OnStart(string[] args)
        {
            
            this.BPM_EventLog.WriteEntry("Service Started Successfully");

            BPM_Timer.Enabled = true;
            BPM_Timer.Start();
        }

        /////////////////////////////////////////////////
        // STOP SERVICE
        /////////////////////////////////////////////////

        protected override void OnStop()
        {
            BPM_EventLog.WriteEntry("BPMonitor Service stoping.");
            BPM_Timer.Stop();
            BPM_Timer.Enabled = false;
        }

        /////////////////////////////////////////////////
        // TIMER FIRED LOGIC
        /////////////////////////////////////////////////

        private void BPM_Timer_Tick(object sender, EventArgs e)
        {
            BPM_EventLog.WriteEntry("BPMonitor Timer fired.");

            /////////////////////////////////////////////////
            // CONNECT MONGO AND POST DATA
            /////////////////////////////////////////////////

            var mongoPost = new DataPost();

            try
            {
                mongoPost.makeConnection();
                BPM_EventLog.WriteEntry("Connection Succeeded");

                try
                {
                    mongoPost.postData();
                    BPM_EventLog.WriteEntry("Post Data Succeeded");
                }
                catch (MongoCommandException m)
                {
                    BPM_EventLog.WriteEntry("Failed to Post Data: " + m, EventLogEntryType.Error);
                }
            }
            catch (MongoConnectionException m)
            {
                BPM_EventLog.WriteEntry("Connection Failed: " + m, EventLogEntryType.Error);

            }
            catch (Exception ex)
            {
                BPM_EventLog.WriteEntry("This Broke: " + ex, EventLogEntryType.Error);
            }
            
            


        }
    }
}
