using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Configuration;
using MongoDB;

namespace BPMonitor_Service
{
    public partial class DataPost : Component
    {

        public DataPost()
        {
            InitializeComponent();
        }

        public DataPost(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        public MongoDB.Mongo makeConnection()
        {
            string MonoDB = ConfigurationManager.AppSettings["mongodb"];
            var BPM_Connection = new Mongo(MonoDB);

            BPM_Connection.Connect();

            return BPM_Connection;
        }

        public void postData()
        {

            var conn = makeConnection();

            var db = conn.GetDatabase("bptest_db");
            var collection = db.GetCollection<datacollection>();

            insertData(collection);

            conn.Disconnect();
           
        }

        private static void insertData(IMongoCollection<datacollection> collection)
        {
            CounterData BPM_Data = new CounterData();

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
                net_info = BPM_Data.get_NETData(),
                disk_wieght = BPM_Data.get_DiskWeight(),
                cpu_weight = BPM_Data.get_CPUWeight(),
                mem_weight = BPM_Data.get_MEMWeight(),
                total_weight = BPM_Data.get_TotalWeight(),
                unix_timestamp = BPM_Data.unix_timestamp(),
            };

            collection.Save(post);
           
        }       

    }
}
