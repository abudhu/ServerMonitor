namespace BPMonitor_Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BPMonitorServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.BPMonitorInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // BPMonitorServiceInstaller
            // 
            this.BPMonitorServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BPMonitorServiceInstaller.Password = null;
            this.BPMonitorServiceInstaller.Username = null;
            // 
            // BPMonitorInstaller
            // 
            this.BPMonitorInstaller.Description = "Service Monitors health of Servers";
            this.BPMonitorInstaller.DisplayName = "BPMonitor Service";
            this.BPMonitorInstaller.ServiceName = "BPMonitor";
            this.BPMonitorInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BPMonitorServiceInstaller,
            this.BPMonitorInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BPMonitorServiceInstaller;
        private System.ServiceProcess.ServiceInstaller BPMonitorInstaller;
    }
}