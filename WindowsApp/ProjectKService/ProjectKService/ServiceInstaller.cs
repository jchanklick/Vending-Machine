using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectKService
{
    // from: http://www.codeproject.com/Articles/106742/Creating-a-simple-Windows-Service
    [System.ComponentModel.RunInstaller(true)]
    public class ProjectInstaller : System.Configuration.Install.Installer
    {   
        private System.ServiceProcess.ServiceInstaller serviceInstaller;
        private System.ServiceProcess.ServiceProcessInstaller 
                serviceProcessInstaller;

        public ProjectInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.serviceInstaller = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller = 
              new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // serviceInstaller
            // 
            this.serviceInstaller.Description = "Vending Machine Application";
            this.serviceInstaller.DisplayName = "Project K Service";
            this.serviceInstaller.ServiceName = "ProjectKService";
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = 
              System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // ServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.serviceInstaller});

        }
    }
}
