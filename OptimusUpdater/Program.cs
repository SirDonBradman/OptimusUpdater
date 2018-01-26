using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.IO;

namespace OptimusUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Waiting for Optimus to terminate");

            string NugetSourceRepo = null, OutputDirectory = null, Version = null, PID = null;
            if (args.Length > 0)
            {
                int Index = Array.IndexOf(args, "-AppPath");
                if (Index >= 0)
                    OutputDirectory = args[Index + 1];
                Index = Array.IndexOf(args, "-Version");
                if (Index >= 0)
                    Version = args[Index + 1];
                Index = Array.IndexOf(args, "-NugetSourceRepo");
                if (Index >= 0)
                    NugetSourceRepo = args[Index + 1];
                Index = Array.IndexOf(args, "-PID");
                if (Index >= 0)
                    PID = args[Index + 1];
            }
            if (PID != null)
            {
                Console.WriteLine("Killing the Process with Id " + PID);
                Process Optimus = Process.GetProcessById(int.Parse(PID));
                Optimus.Kill();
                Optimus.WaitForExit();
            }

            Console.WriteLine("Initiating the process...");

            if (NugetSourceRepo == null)
                NugetSourceRepo = ConfigurationManager.AppSettings["NugetSourceRepo"].ToString();

            if (OutputDirectory == null)
                OutputDirectory = ConfigurationManager.AppSettings["OutputDirectory"].ToString();

            if (Version == null)
                Version = ConfigurationManager.AppSettings["Version"].ToString();

            string NugetInstallCommand = @"/c nuget install aurigo.optimus -Source " + NugetSourceRepo + " -OutputDirectory " + OutputDirectory + " -Version " + Version;


            Console.WriteLine("Clearing the output directory");
            DirectoryInfo OutputDir = new DirectoryInfo(OutputDirectory + @"\Aurigo.Optimus." + Version);
            //if (OutputDir.Exists)
            //{
            //    foreach (FileInfo file in OutputDir.GetFiles())
            //    {
            //        file.Delete();
            //    }
            //    foreach (DirectoryInfo dir in OutputDir.GetDirectories())
            //    {
            //        dir.Delete(true);
            //    }
            //}

            foreach (FileInfo file in OutputDir.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            foreach (DirectoryInfo dir in OutputDir.GetDirectories())
            {
                try
                {
                    dir.Delete(true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Console.WriteLine("Output directory cleared");

            Console.WriteLine("Initiating fetch from nuget");
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo.FileName = @"cmd.exe";
            startInfo.Arguments = NugetInstallCommand;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            Console.WriteLine("Fetch complete");

            Console.WriteLine("Initiate Optimus");
            string OptimusDir = OutputDirectory + @"\Aurigo.Optimus." + Version + @"\lib\net46\";
            string OptimusFile = "AutomationStart.exe";

            Process process1 = new Process();
            ProcessStartInfo startInfo1 = new ProcessStartInfo();
            startInfo1.WindowStyle = ProcessWindowStyle.Maximized;
            startInfo1.FileName = OptimusFile;
            startInfo1.WorkingDirectory = OptimusDir;
            process1.StartInfo = startInfo1;
            process1.Start();
            Console.WriteLine("Optimus Respawned");
        }
    }
}
