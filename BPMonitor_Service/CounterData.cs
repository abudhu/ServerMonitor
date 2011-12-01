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

            return percentageMEM;

        }

        /////////////////////////////////////////////////
        // GET DISK PERCENTAGE
        /////////////////////////////////////////////////
        public double[] get_DISK()
        {
            const double bytes_per_gigabyte = 1073741824;
            
            DriveInfo[] drives = DriveInfo.GetDrives();

            drives = drives.Where(d => d.DriveType == DriveType.Fixed).ToArray();

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
        // GET DISK WRITES
        /////////////////////////////////////////////////

        public float get_DISKWrites()
        {

            PerformanceCounter theDiskWrite = new PerformanceCounter("PhysicalDisk", "Disk Writes/sec", "_Total");

            float WRITEValue = theDiskWrite.NextValue();

            System.Threading.Thread.Sleep(1000);

            WRITEValue = theDiskWrite.NextValue();

            return WRITEValue;

        }

        /////////////////////////////////////////////////
        // GET DISK READS
        /////////////////////////////////////////////////

        public float get_DISKReads()
        {

            PerformanceCounter theDiskRead = new PerformanceCounter("PhysicalDisk", "Disk Reads/sec", "_Total");

            float READValue = theDiskRead.NextValue();

            System.Threading.Thread.Sleep(1000);

            READValue = theDiskRead.NextValue();

            return READValue;

        }
                
        /////////////////////////////////////////////////
        // GET NETWORK DATA (http://msdn.microsoft.com/en-us/library/cc768535(v=bts.10).aspx)
        /////////////////////////////////////////////////

        public double[] get_NETData()
        {

            const int interations = 10;
            long sumSent = 0;
            long sumRecieve = 0;
            
            NetworkInterface[] netInterface = NetworkInterface.GetAllNetworkInterfaces();

            netInterface = netInterface.Where(n => n.NetworkInterfaceType == NetworkInterfaceType.Ethernet).ToArray();

            double[] netData = new double[netInterface.Length];
            int index = 0;

            foreach (NetworkInterface network in netInterface)
            {
                IPv4InterfaceStatistics interfaceStats = network.GetIPv4Statistics();

                double speed = network.Speed; // bandwidth

                for (int n = 0; n < interations; n++)
                {
                    sumSent += interfaceStats.BytesSent;
                    sumRecieve += interfaceStats.BytesReceived;
                }

                double totalSent = sumSent;
                double totalReceive = sumRecieve;
                
                double totalUsed = (((8 * (totalSent + totalReceive)) * 100) / (speed * interations));
                double totalFree = (100 - totalUsed) / 100;

                netData[index] = totalFree;
                index++;

            }

            return netData;

        }

        /////////////////////////////////////////////////
        // CALCULATE ALL THE WEIGHTS!
        /////////////////////////////////////////////////



        /////////////////////////////////////////////////
        // CALCULATE ARRAY WIEGHTS
        /////////////////////////////////////////////////

        public double arrayAlert(double[] p, double weight)
        {
            
            Array.Sort(p);
            //p[0] is the minimum %free

            return p[0] * weight;

        }

        public double singleAlert(double p, double weight)
        {
            //weight /= 100;
            p = p / 100;

            return p * weight;           
        }

    }
}
