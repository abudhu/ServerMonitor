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
        public float cpu_percent { get; set; }
        public float available_mem { get; set; }
        public float total_mem { get; set; }
        public float mem_percent { get; set; }
        public double[] disk_info { get; set; }
        public string disk_read { get; set; }
        public string disk_write { get; set; }
        public double[] net_info { get; set; }
        public double[] disk_wieght { get; set; }
        public float cpu_weight { get; set; }
        public float mem_weight { get; set; }
        public float total_weight { get; set; }
        public double unix_timestamp { get; set; }
    }
}
