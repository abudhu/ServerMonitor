using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.Net.NetworkInformation;

namespace BPMonitor_Service
{
    public partial class CounterData : Component
    {
        public CounterData()
        {
            InitializeComponent();

        }

        public CounterData(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        /////////////////////////////////////////////////
        // GET ALL THE DATA!
        /////////////////////////////////////////////////

        
        /////////////////////////////////////////////////
        // GET UNIX TIMESTAMP
        /////////////////////////////////////////////////

        public int unix_timestamp()
        {
            TimeSpan unix_time = (System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (int)unix_time.TotalSeconds;
        }

        /////////////////////////////////////////////////
        // GET COMPUTER NAME
        /////////////////////////////////////////////////

        public string computer_name()
        {
            string name = System.Windows.Forms.SystemInformation.ComputerName;
            return name;
        }


        /////////////////////////////////////////////////
        // GET CPU PERCENTAGE
        /////////////////////////////////////////////////

        public float get_CPU()
        {
            PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            float CPUValue = theCPUCounter.NextValue();
            
            System.Threading.Thread.Sleep(1000);

            CPUValue = theCPUCounter.NextValue();

            CPUValue = 100 - CPUValue;

            theCPUCounter.Dispose(); //Remove the counter
            
            return CPUValue;

        }


        /////////////////////////////////////////////////
        // GET MEMORY PERCENTAGE
        /////////////////////////////////////////////////

        public double get_MEM()
        {
            PerformanceCounter availCounter = new PerformanceCounter("Memory", "Available MBytes");

            Microsoft.VisualBasic.Devices.ComputerInfo VB = new Microsoft.VisualBasic.Devices.ComputerInfo();

            float totalMEM = (VB.TotalPhysicalMemory / 1048576); //This converts the Float +e9 to Megabytes

            double availMEM = availCounter.NextValue();

            System.Threading.Thread.Sleep(1000);

            availMEM = availCounter.NextValue();

            double percentageMEM = (availMEM / totalMEM) * 100;

            availCounter.Dispose(); // Remove the counter

            return percentageMEM;

        }

        /////////////////////////////////////////////////
        // GET DISK PERCENTAGE
        /////////////////////////////////////////////////

        public double[] get_DISK()
        {
            const double bytes_per_gigabyte = 1073741824;
            
            DriveInfo[] drives = DriveInfo.GetDrives();

            drives = drives.Where(d => d.DriveType == DriveType.Fixed).ToArray(); // Gets only Logicial / Physically attached Hard Drives

            double[] percentageFree = new double[drives.Length];
            
            int index = 0;

            foreach (DriveInfo drive in drives)
            {
                percentageFree[index] = (drive.AvailableFreeSpace / bytes_per_gigabyte) / (drive.TotalSize / bytes_per_gigabyte);

                index++;
            }
            
            return percentageFree;
        }
        

        /////////////////////////////////////////////////
        // GET DISK IDLE % (http://msdn.microsoft.com/en-us/library/ms175903.aspx)
        /////////////////////////////////////////////////

        /////////////////////////////////////////////////
        // Rather than get Disk Reads/Writes, we monitor performance on the % Disk Time in use.  
        // The larger the % the more IOPs are being used, which should indicate poor performance
        /////////////////////////////////////////////////

        public double get_DISKTime()
        {

            PerformanceCounter theTimeValue = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");

            double timeValue = theTimeValue.NextValue();

            System.Threading.Thread.Sleep(1000);

            timeValue = theTimeValue.NextValue();

            timeValue = 100 - timeValue;

            theTimeValue.Dispose(); // Remove the counter

            return timeValue;

        }

                
        /////////////////////////////////////////////////
        // GET NETWORK DATA (http://msdn.microsoft.com/en-us/library/cc768535(v=bts.10).aspx)
        /////////////////////////////////////////////////

        public double[] get_NETData()
        {

            NetworkInterface[] netInterface = NetworkInterface.GetAllNetworkInterfaces();

            netInterface = netInterface.Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet).ToArray(); // Gets only the Ethernet NICS

            double[] netData = new double[netInterface.Length];
            int index = 0;

            foreach (NetworkInterface network in netInterface)
            {
                // The networkName returned from PerfMon is different than the one returned from NetInterface Method
                // To fix this we replace the mismatched characters from the NetInterface so that Perfmon can work.

                string networkName = network.Description.Replace('(', '[');
                networkName = networkName.Replace(')', ']');
                networkName = networkName.Replace('/', '_');

                PerformanceCounter theNetValue = new PerformanceCounter("Network Interface", "Bytes Total/sec", networkName);

                IPv4InterfaceStatistics interfaceStats = network.GetIPv4Statistics();

                double speed = network.Speed; // Network Speed (100Mbps / 1Gbps / 10Gbps)  aka Bandwidth

                double netValue = theNetValue.NextValue();

                System.Threading.Thread.Sleep(1000);

                netValue = theNetValue.NextValue();

                netValue = (100 - (((netValue * 8) / speed) * 100)) / 100;

                netData[index] = netValue;
                
                index++;

                theNetValue.Dispose(); // Remove the counter

            }

            return netData;

        }


        /////////////////////////////////////////////////
        // CALCULATE ARRAY WIEGHTS
        /////////////////////////////////////////////////

        public double arrayAlert(double[] p, double weight)
        {
            
            Array.Sort(p);
            
            return p[0] * weight; //p[0] is the minimum %free

        }

        /////////////////////////////////////////////////
        // CALCULATE SINGLE WIEGHTS
        /////////////////////////////////////////////////

        public double singleAlert(double p, double weight)
        {
           
            p = p / 100;

            return p * weight;           
        }

    }
}
