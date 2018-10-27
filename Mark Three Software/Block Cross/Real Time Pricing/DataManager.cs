
namespace MarkThree.Guardian.Server
{
    using System;
    using System.Text;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
   
    class DataManager
    {
        private Process processDM = null;
       
        private const string ESIGNAL_PROCESS_NAME = "winros";
        private const string ESIGNAL_INI_FILENAME = "winros.ini";
        private const string ESIGNAL_USERNAME_INI_KEY = "InternetUserName";
        private const string ESIGNAL_PASSWORD_INI_KEY = "InternetPassword";
        private const string ESIGNAL_HOSTNAME_INI_KEY = "InternetAddress";
        private const string ESIGNAL_STARTPATH_INI_KEY = "StartPath";

        private string username;
        private string password;
        private string startpath;
        private string hostname;

        public DataManager()
        {
           
        }

        /// <summary>
        /// eSignal username from the winros.ini file
        /// </summary>
        internal string Username
        {
            get { return username; }
        }

        /// <summary>
        /// eSignal password from the winros.ini file
        /// </summary>
        internal string Password
        {
            get { return password; }
        }

        /// <summary>
        /// eSignal hostname from the winros.ini file (not used - we connect to the local instance)
        /// </summary>
        internal string Hostname
        {
            get { return hostname; }
        }

        /// <summary>
        /// filepath to the eSignal local data manager .exe
        /// </summary>
        internal string Startpath
        {
            get { return startpath; }
        }

        /// <summary>
        /// Initializes the member variables for creating an eSignal datamanager process.  Reads the eSignal
        /// .ini file for the credentials.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        internal bool Init()
        {
            // figure out the path to the .ini file
            string windir = System.Environment.GetEnvironmentVariable("Windir");
            if (windir.Length == 0)
                return false;
    
            // Set the path to the WINROS.INI file.
            string dmINI = windir + "\\" + ESIGNAL_INI_FILENAME;

            // stringbuilder for extracting values from the .ini file
            StringBuilder str = new StringBuilder(256);

            // get the username from the .ini file
            str.Remove(0, str.Length);
            if (GetPrivateProfileString(ESIGNAL_PROCESS_NAME, ESIGNAL_USERNAME_INI_KEY, "", str, str.Capacity, dmINI)==0)
                return false;
            username = str.ToString();

            // get the password from the .ini file
            str.Remove(0, str.Length);
            if (GetPrivateProfileString(ESIGNAL_PROCESS_NAME, ESIGNAL_PASSWORD_INI_KEY, "", str, str.Capacity, dmINI)==0)
                return false;
            password = str.ToString();

            // get the hostname from the .ini file
            str.Remove(0, str.Length);
            if (GetPrivateProfileString(ESIGNAL_PROCESS_NAME, ESIGNAL_HOSTNAME_INI_KEY, "", str, str.Capacity, dmINI)==0)
                return false;
            hostname = str.ToString();

            // get the password from the .ini file
            str.Remove(0, str.Length);
            if (GetPrivateProfileString(ESIGNAL_PROCESS_NAME, ESIGNAL_STARTPATH_INI_KEY, "", str, str.Capacity, dmINI)==0)  
                return false;
            startpath = str.ToString();
            if (!startpath.EndsWith("\\"))
                startpath += "\\"; 
            startpath += ESIGNAL_PROCESS_NAME + ".exe";

            return true;
        }

        /// <summary>
        /// Starts an instance of the local eSignal Data manager process (winros.exe).  We keep a reference
        /// to the Process object so we can stop the process (only if we started it).  If the Data Manager is 
        /// already running, then we don't start a new instance.
        /// </summary>
        /// <returns></returns>
        internal bool Start()
        {
			try
			{
                // if a winros.exe process isn't running, then we start the process
                // we hold a Process object so we know we started it, and we can stop it.
			    if (!IsRunning())
			    {
                    processDM = Process.Start(this.Startpath);
			    }
            }
            catch (Exception e) 
            {
                MarkThree.EventLog.Error("Unable to start local eSignal data manager: " + e.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Stops the local eSignal data manager process.  We only stop the process if we started it.
        /// If we started the process, then the processDM variable will not be null.
        /// </summary>
        internal void Stop()
        {
            try
            {
                // if processDM is not null, then we started it
                if (processDM != null)
                {
                    // close/stop the process
                    processDM.CloseMainWindow();
                    processDM.Close();
                }
            }
            catch (Exception e)
            {
                MarkThree.EventLog.Error("Unable to stop the local eSignal data manager: " + e.Message);
            }
        }


        /// <summary>
        /// Iterates through the list of running processes, and returns true if a match to "winros" is found.
        /// </summary>
        /// <returns>true if the datamanager is already running, false otherwise.</returns>
        internal bool IsRunning()
        {
            Process[] arrProcesses = Process.GetProcesses();
            foreach (Process process in arrProcesses)
            {
                if (process.ProcessName == ESIGNAL_PROCESS_NAME)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Converts a time from UTC to Local Time
        /// </summary>
        /// <param name="the_date">UTC time to be converted</param>
        /// <returns>Local time converstion of the_date</returns>
        internal static System.DateTime UTCToLocalTime(System.DateTime the_date)
        {
            the_date = the_date.ToLocalTime();

            return the_date;
        }


        // import of method to read an INI file to extract the eSignal credentials
        [DllImport("Kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        public static extern int GetPrivateProfileString(string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
    }
}
