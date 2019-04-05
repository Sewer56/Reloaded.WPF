using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Principal;
using System.Windows;

namespace Reloaded.WPF.TestWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            if (!IsElevated)
            {
                MessageBox.Show("You need to run this application/demo as administrator.\n" +
                                "Administrative priviledges are needed to receive application launch/exit events" +
                                "from Windows Management Instrumentation (WMI).\n" +
                                "Developers: Run your favourite IDE e.g. Visual Studio as Admin.");
                Environment.Exit(0);
            } 


        }

        /// <summary>
        /// Checks if the application is running as root/sudo/administrator.
        /// </summary>
        private static bool IsElevated =>
            new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
