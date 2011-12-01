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
                unix_timestamp = BPM_Data.unix_timestamp(),
                driveWeight = BPM_Data.arrayAlert(BPM_Data.get_DISK(), 512),
                cpuWeight = BPM_Data.singleAlert(BPM_Data.get_CPU(), 512),
                memWeight = BPM_Data.singleAlert(BPM_Data.get_MEM(), 512),
                netData = BPM_Data.arrayAlert(BPM_Data.get_NETData(), 512)
                
            };

            collection.Save(post);
           
        }       

    }
}
