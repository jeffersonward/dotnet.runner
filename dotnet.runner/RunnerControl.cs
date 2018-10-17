using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dotnet.runner
{
    public partial class RunnerControl : UserControl
    {
        private readonly FileSystemWatcher _watcher;

        private Process _process;
        private ProcessStartInfo _startInfo;

        public RunnerControl()
        {
            InitializeComponent();

            _watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true
            };

            _watcher.Changed += Compiling;
            _watcher.Created += Compiling;
            _watcher.Deleted += Compiling;
            _watcher.Renamed += Compiling;
        }

        public delegate void RunningStateChangedDelegate(object sender, EventArgs args);

        private delegate void AppendTextCallback(string text);

        public event RunningStateChangedDelegate RunningStateChanged;

        public bool Running { get; private set; }

        public void SetStartInfo(string workingDirectory)
        {
            _startInfo = new ProcessStartInfo("dotnet")
            {
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            textBoxWorkingDirectory.Text = workingDirectory;

            _watcher.Path = Path.Combine(workingDirectory, "obj");
        }

        private void AppendLine(string text)
        {
            if (richTextBoxOutput.InvokeRequired)
            {
                var d = new AppendTextCallback(AppendLine);
                Invoke(d, text);
            }
            else
            {
                if (MatchesTraceLevel("INF", text, Color.Green, out var newText))
                {
                    text = newText;
                }
                else if (MatchesTraceLevel("DBG", text, Color.Cyan, out newText))
                {
                    text = newText;
                }
                else if (MatchesTraceLevel("WRN", text, Color.Orange, out newText))
                {
                    text = newText;
                }
                else if (MatchesTraceLevel("ERR", text, Color.Red, out newText))
                {
                    text = newText;
                }
                else if (MatchesTraceLevel("CRT", text, Color.MediumPurple, out newText))
                {
                    text = newText;
                }

                richTextBoxOutput.AppendText(text + "\r\n");
            }
        }

        private void AppendText(string text, Color color)
        {
            var originalSelectionStart = richTextBoxOutput.SelectionStart;
            var originalSelectionLength = richTextBoxOutput.SelectionLength;

            var selectionStart = richTextBoxOutput.TextLength;
            richTextBoxOutput.AppendText(text);
            richTextBoxOutput.Select(selectionStart, text.Length);
            richTextBoxOutput.SelectionColor = color;

            richTextBoxOutput.SelectionStart = richTextBoxOutput.TextLength;
            richTextBoxOutput.SelectionLength = 0;
            richTextBoxOutput.SelectionColor = Color.White;

            richTextBoxOutput.SelectionStart = originalSelectionStart;
            richTextBoxOutput.SelectionLength = originalSelectionLength;
        }

        private void ButtonStartStop_Click(object sender, EventArgs e)
        {
            if (Running)
            {
                Stop("User requested");
            }
            else
            {
                Start();
            }
        }

        private void Compiling(object sender, FileSystemEventArgs e)
        {
            if (Running)
            {
                Stop("Detected compilation in progress");
            }
        }

        private void ReadError()
        {
            while (Running)
            {
                string text;
                while ((text = _process.StandardError.ReadLine()) != null)
                {
                    AppendLine(text);
                }
            }
        }

        private void ReadOutput()
        {
            while (Running)
            {
                string text;
                while ((text = _process.StandardOutput.ReadLine()) != null)
                {
                    AppendLine(text);
                }
            }
        }

        private void Start()
        {
            _startInfo.Arguments = checkBoxWatch.Checked ? "watch run" : "run --no-build";

            _process = Process.Start(_startInfo);

            _watcher.EnableRaisingEvents = !checkBoxWatch.Checked;

            richTextBoxOutput.Text = "";
            buttonStartStop.Text = "Stop";
            checkBoxWatch.Enabled = false;
            Running = true;

            Task.Run(() => ReadOutput());
            Task.Run(() => ReadError());
            Task.Run(() => WatchProcess());

            RunningStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private bool MatchesTraceLevel(string level, string text, Color color, out string newText)
        {
            const string serilogPattern = @"^\[\d*\:\d*\:\d*[ ]{0}\]";
            var regex = new Regex(string.Format(serilogPattern, level));

            newText = text;

            var match = regex.Match(text);
            if (!match.Success && match.Index == 0) return false;

            newText = text.Substring(match.Length);
            AppendText(match.Value, color);
            return true;
        }

        private void Stop(string message)
        {
            if (!_process.HasExited)
            {
                var startInfo = new ProcessStartInfo("taskkill", $"/pid {_process.Id} /f /t")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(startInfo);
            }
            _process.WaitForExit();
            Running = false;
            _watcher.EnableRaisingEvents = false;
            _process = null;
            buttonStartStop.Text = "Start";
            checkBoxWatch.Enabled = true;

            AppendLine("");
            AppendLine("");
            AppendLine("Server stopped: " + message);

            RunningStateChanged?.Invoke(this, EventArgs.Empty);
        }

        private void WatchProcess()
        {
            while (Running)
            {
                if (_process.HasExited)
                {
                    Stop("Process was terminated");
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        public void FocusOnOutput()
        {
            richTextBoxOutput.Focus();
        }

        private void richTextBoxOutput_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }
    }
}