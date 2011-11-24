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
            // MAKE MONGO CONNECTION
            /////////////////////////////////////////////////

            //DBConnection conn = new DBConnection();
            //conn.makeConnection();

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


            // WHEN THIS FIRES YOU NEED TO GET A MONGODB AND COLLECTION
            // THEN YOU NEED TO WRITE DATA ON THE COLLECTION

            postData();

            /////////////////////////////////////////////////
            // INTIALIZE COUNTER AND POST DATA
            /////////////////////////////////////////////////

            //CounterData BPM_Data = new CounterData();
            //double[] myDISK = BPM_Data.get_diskINFO();
            //double[] myDISKWeight = BPM_Data.get_DiskWeight();
            //int diskCount = myDISK.Length;
            //int diskCount2 = myDISKWeight.Length;
            //int n = 0;

            //TextWriter tw = new StreamWriter(@"C:\temp\connection.txt");

            //tw.WriteLine("Computer Name: " + BPM_Data.computer_name());
            //tw.WriteLine("CPU Percent: " + BPM_Data.get_CPU());
            //tw.WriteLine("Available Mem: " + BPM_Data.get_AvailableMEM());
            //tw.WriteLine("Total Mem: " + BPM_Data.get_TotalMEM());
            //tw.WriteLine("Mem Percent: " + BPM_Data.get_PercentageMEM());
            //tw.WriteLine("CPU Weight: " + BPM_Data.get_CPUWeight());
            //tw.WriteLine("Mem Weight: " + BPM_Data.get_MEMWeight());

            //while (n < diskCount)
            //{

            //    tw.WriteLine("Drive " + n + ": " + myDISK[n]);
            //    tw.WriteLine("Drive Weight " + n + ": " + myDISKWeight[n]);
            //    n++;
            //}

            //tw.WriteLine("Total Weight: " + BPM_Data.get_TotalWeight());

            //tw.WriteLine("Unix Time Stamp: " + BPM_Data.unix_timestamp());        


            //tw.Close();

        }

        public void postData()
        {
            
            string MonoDB = ConfigurationManager.AppSettings["mongodb"];
            var BPM_Connection = new Mongo(MonoDB);
            BPM_Connection.Connect();

            var db = BPM_Connection.GetDatabase("bptest_db");
            var collection = db.GetCollection<datacollection>();
            
            insertData(collection);

            BPM_Connection.Disconnect();
        }

        private static void insertData(IMongoCollection<datacollection> collection)
        {
            CounterData BPM_Data = new CounterData();
            //TextWriter tw = new StreamWriter(@"C:\temp\connection.txt");

            var post = new datacollection()
            {
                computer_Name = BPM_Data.computer_name(),
                cpu_percent = BPM_Data.get_CPU(),
                available_mem = BPM_Data.get_AvailableMEM(),
                total_mem = BPM_Data.get_TotalMEM(),
                mem_percent = BPM_Data.get_PercentageMEM(),
                disk_info = BPM_Data.get_diskINFO(),
                disk_read = BPM_Data.get_DISKReads(),
                disk_write = BPM_Data.get_DISKWrites(),
                disk_wieght = BPM_Data.get_DiskWeight(),
                cpu_weight = BPM_Data.get_CPUWeight(),
                mem_weight = BPM_Data.get_MEMWeight(),
                total_weight = BPM_Data.get_TotalWeight(),
                unix_timestamp = BPM_Data.unix_timestamp(),
            };
            collection.Save(post);
            //Console.WriteLine(BPM_Data.get_DISKWrites());
            //tw.Close();
        }        
    }
}
