using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using NLog;
using NzbDrone.Core;

namespace NzbDrone.Console
{
    class Program
    {

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            CentralDispatch.ConfigureNlog();
            Logger.Info("Starting NZBDrone WebUI");
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(System.Environment.CurrentDirectory);
            var server = new CassiniDev.Server(System.IO.Path.Combine(dir.Parent.Parent.Parent.FullName, "NzbDrone.Web"));
            server.Start();

            System.Diagnostics.Process.Start(server.RootUrl);
            Logger.Info("Server available at: " + server.RootUrl);
            System.Console.ReadLine();

        }
    }
}
