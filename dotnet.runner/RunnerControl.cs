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
                var start = richTextBoxOutput.TextLength;

                richTextBoxOutput.AppendText(text);

                HighlightLogLevel(text, start);
                Highlight(HyperlinkRegex, start, text, Color.Blue);

                richTextBoxOutput.AppendText("\r\n");
            }
        }

        private void HighlightLogLevel(string text, int start)
        {
            if (Highlight(Verbosegex, start, text, Color.Gray)) return;
            if (Highlight(InfoRegex, start, text, Color.Green)) return;
            if (Highlight(DebugRegex, start, text, Color.Cyan)) return;
            if (Highlight(WarnRegex, start, text, Color.Orange)) return;
            if (Highlight(ErrorRegex, start, text, Color.Red)) return;
            Highlight(FatalRegex, start, text, Color.MediumPurple);
        }

        private bool Highlight(Regex pattern, int start, string text, Color color)
        {
            var matches = pattern.Matches(text);

            if (matches.Count == 0) return false;

            var originalSelectionStart = richTextBoxOutput.SelectionStart;
            var originalSelectionLength = richTextBoxOutput.SelectionLength;

            for (var i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                richTextBoxOutput.Select(start + match.Index, match.Length);
                richTextBoxOutput.SelectionColor = color;
            }

            richTextBoxOutput.SelectionStart = originalSelectionStart;
            richTextBoxOutput.SelectionLength = originalSelectionLength;

            return true;
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

        private const string SerilogPattern = @"^(\[\d{{2}}\:\d{{2}}\:\d{{2}}[ ]{0}\][ ]|\d{{2}}\:\d{{2}}\:\d{{2}}[ ]\[{0}\][ ])";
        private static readonly Regex Verbosegex = new Regex(string.Format(SerilogPattern, "VRB"));
        private static readonly Regex InfoRegex = new Regex(string.Format(SerilogPattern, "INF"));
        private static readonly Regex DebugRegex = new Regex(string.Format(SerilogPattern, "DBG"));
        private static readonly Regex WarnRegex = new Regex(string.Format(SerilogPattern, "WRN"));
        private static readonly Regex ErrorRegex = new Regex(string.Format(SerilogPattern, "ERR"));
        private static readonly Regex FatalRegex = new Regex(string.Format(SerilogPattern, "FTL"));
        private static readonly Regex HyperlinkRegex = new Regex(@"((ht|f)tp(s?)\:\/\/([^ ]+))");
    }
}