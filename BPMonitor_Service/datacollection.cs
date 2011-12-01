using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB;

namespace BPMonitor_Service
{
    public class datacollection
    {
        public Oid Id { get; set; }
        public string computer_Name { get; set; }
        public double unix_timestamp { get; set; }
        public double driveWeight { get; set; }
        public double cpuWeight { get; set; }
        public double memWeight { get; set; }
        public double netData { get; set; }

    }
}
