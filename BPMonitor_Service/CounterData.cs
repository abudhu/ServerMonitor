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
        // GET CPU PERCENTAGE
        /////////////////////////////////////////////////

        public float get_CPU()
        {
            PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            float CPUValue = theCPUCounter.NextValue();
            
            System.Threading.Thread.Sleep(1000);

            CPUValue = theCPUCounter.NextValue();
            
            return CPUValue;
        }

        /////////////////////////////////////////////////
        // GET AVAILABLE MEMORY
        /////////////////////////////////////////////////

        public float get_AvailableMEM()
        {
            PerformanceCounter theMEMCounter = new PerformanceCounter("Memory", "Available MBytes");
            
            float MEMValue = theMEMCounter.NextValue();
            
            System.Threading.Thread.Sleep(1000);

            MEMValue = theMEMCounter.NextValue();
            
            return MEMValue;
            
        }

        /////////////////////////////////////////////////
        // GET TOTAL PHYSICAL MEMORY
        /////////////////////////////////////////////////

        public float get_TotalMEM()
        {
            Microsoft.VisualBasic.Devices.ComputerInfo VB = new Microsoft.VisualBasic.Devices.ComputerInfo();
            
            float TOTALValue = (VB.TotalPhysicalMemory / 1048576); //This converts the Float +e9 to Megabytes

            return TOTALValue;        
        }

        /////////////////////////////////////////////////
        // GET PERCENTAGE FREE MEMORY
        /////////////////////////////////////////////////

        public float get_PercentageMEM()
        {
            float percentageMEM = (get_AvailableMEM() / get_TotalMEM());

            return percentageMEM;
        }

        /////////////////////////////////////////////////
        // GET DRIVE SPACE
        /////////////////////////////////////////////////
                
        public double[] get_diskINFO()
        {
                        
            DriveInfo[] drives = DriveInfo.GetDrives();

            drives = drives.Where(d => d.DriveType == DriveType.Fixed).ToArray();
            
            double[] spaces = new double[drives.Length];
            int index = 0;
            
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady == true && drive.DriveType.Equals(DriveType.Fixed))
                {
                   
                    double drivePercent = (((double)(drive.AvailableFreeSpace / 1073741824)) / ((double)(drive.TotalSize / 1073741824)));

                    spaces[index] = (drivePercent);
                    index++;
                }
            }
            return spaces;


        }

        /////////////////////////////////////////////////
        // GET DISK WRITES
        /////////////////////////////////////////////////

        public string get_DISKWrites()
        {

            PerformanceCounter theDiskWrite = new PerformanceCounter("PhysicalDisk", "Disk Writes/sec", "_Total");

            string WRITEValue = theDiskWrite.NextValue().ToString();

            System.Threading.Thread.Sleep(1000);

            WRITEValue = theDiskWrite.NextValue().ToString();

            return WRITEValue;

        }

        /////////////////////////////////////////////////
        // GET DISK READS
        /////////////////////////////////////////////////

        public string get_DISKReads()
        {

            PerformanceCounter theDiskRead = new PerformanceCounter("PhysicalDisk", "Disk Reads/sec", "_Total");

            string READValue = theDiskRead.NextValue().ToString();

            System.Threading.Thread.Sleep(1000);

            READValue = theDiskRead.NextValue().ToString();

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

            netInterface = netInterface.Where(d => d.NetworkInterfaceType == NetworkInterfaceType.Ethernet).ToArray();

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
                
                netData[index] = (((8 * (totalSent + totalReceive)) * 100) / (speed * interations));
                index++;

            }

            return netData;

        }




        // BELOW SECTION IS ALL THE WEIGHT CALCS





        /////////////////////////////////////////////////
        // GET DISK WEIGHT
        /////////////////////////////////////////////////

        public double[] get_DiskWeight()
        {
            double[] disks = get_diskINFO();
            double warn_level = ((Convert.ToDouble(ConfigurationManager.AppSettings["disk_warn"])) / 100);
            double[] levels = new double[disks.Length];
            int index = 0;

            foreach(double disk in disks)
            {
                if (disk < warn_level)
                {
                    levels[index] = 512;
                }
                else
                {
                    levels[index] = 0;
                }
                index++;
            }

            return levels;


        }
        
        /////////////////////////////////////////////////
        // GET CPU WEIGHT
        /////////////////////////////////////////////////

        public float get_CPUWeight()
        {
            float cpuPercent = (get_CPU() / 100);
            cpuPercent = (1 - cpuPercent);
            float returnWeight = weight_Value(cpuPercent);

            return returnWeight;
        }

        /////////////////////////////////////////////////
        // GET MEMORY WEIGHT
        /////////////////////////////////////////////////

        public float get_MEMWeight()
        {
            float memPercent = (get_PercentageMEM());
            float returnWeight = weight_Value(memPercent);

            return returnWeight;
        }

        /////////////////////////////////////////////////
        // GET TOTAL WEIGHT
        /////////////////////////////////////////////////

        public float get_TotalWeight()
        {
            float memValue = get_MEMWeight();
            float cpuValue = get_CPUWeight();
            float totalWeight;

            if (!diskEqual512())
            {
                totalWeight = memValue + cpuValue;
            }
            else
            {
                totalWeight = memValue + cpuValue + 512;
            }
            return totalWeight;
       
        }

        /////////////////////////////////////////////////
        // DO DISK EQAUL 512?
        /////////////////////////////////////////////////

        private bool diskEqual512()
        {
            double[] diskWeights = get_DiskWeight();

            int value = Array.BinarySearch(diskWeights, 512);

            if (value > -1)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /////////////////////////////////////////////////
        // CALCULATE WEIGHT VALUES
        /////////////////////////////////////////////////

        public float weight_Value(float value)
        {
            if (value > 0.0 && value < .11)
            {
                //value = (value * 5120);
                // This is set to a static value because the variations between 0 and 10% are too annoying to calculate
                value = 512;
                return value;

            }
            else if (value > .10 && value < .21)
            {
                value = (value * 2560);
                return value;
            }
            else if (value > .20 && value < .31)
            {
                value = (value * 1280);
                return value;
            }
            else if (value > .30 && value < .41)
            {
                value = (value * 640);
                return value;
            }
            else if (value > .40 && value < .51)
            {
                value = (value * 320);
                return value;
            }
            else if (value > .50 && value < .61)
            {
                value = (value * 160);
                return value;
            }
            else if (value > .60 && value < .71)
            {
                value = (value * 80);
                return value;
            }
            else if (value > .70 && value < .81)
            {
                value = (value * 40);
                return value;
            }
            else if (value > .80 && value < .91)
            {
                value = (value * 20);
                return value;
            }
            else if (value > .90 && value < 1)
            {
                value = (value * 10);
                return value;
            }
            return value;
        }

        /////////////////////////////////////////////////
        // SET UNIX TIMESTAMP
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
    }
}
