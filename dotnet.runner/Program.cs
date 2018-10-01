using System;
using System.IO;
using System.Windows.Forms;

namespace dotnet.runner
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet.runner <solutions...>");
                return -1;
            }

            foreach (var arg in args)
            {
                if (File.Exists(arg) && arg.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)) continue;

                Console.WriteLine($"Solution {arg} not found.");
                Console.WriteLine("Usage: dotnet.runner <solutions...>");
                return -1;
            }

            var sourceFolder = new SourceFolder(args);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(sourceFolder));

            return 0;
        }
    }
}