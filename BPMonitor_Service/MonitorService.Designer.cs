namespace BPMonitor_Service
{
    partial class MonitorService
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
            this.components = new System.ComponentModel.Container();
            this.BPM_EventLog = new System.Diagnostics.EventLog();
            this.BPM_Timer = new System.Timers.Timer(1);
            ((System.ComponentModel.ISupportInitialize)(this.BPM_EventLog)).BeginInit();
            // 
            // BPM_Timer
            // 
            this.BPM_Timer.Interval = 30000D;
            this.BPM_Timer.Elapsed += new System.Timers.ElapsedEventHandler(this.BPM_Timer_Tick);
            // 
            // MonitorService
            // 
            this.ServiceName = "Service1";
            ((System.ComponentModel.ISupportInitialize)(this.BPM_EventLog)).EndInit();

        }

        #endregion

        private System.Diagnostics.EventLog BPM_EventLog;
        private System.Timers.Timer BPM_Timer;
    }
}
