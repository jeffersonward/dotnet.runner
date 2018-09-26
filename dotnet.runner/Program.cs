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
                Console.WriteLine("Usage: dotnet.runner <root folders...>");
                return -1;
            }

            foreach (var arg in args)
            {
                if (Directory.Exists(arg)) continue;

                Console.WriteLine($"Folder {arg} not found.");
                Console.WriteLine("Usage: dotnet.runner <root folders...>");
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