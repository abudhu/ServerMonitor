using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Configuration;
using MongoDB;


namespace BPMonitor_Service
{
    public partial class DBConnection : Component
    {
        public DBConnection()
        {
            InitializeComponent();
        }

        public DBConnection(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

        }

        public void makeConnection()
        {
            string MonoDB = ConfigurationManager.AppSettings["mongodb"];
            var BPM_Connection = new Mongo(MonoDB);

            try
            {
                BPM_Connection.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}
