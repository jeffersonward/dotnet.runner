using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace dotnet.runner
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        internal MainForm(SourceFolder sourceFolder) : this()
        {
            tabControlSolutions.Controls.Clear();

            foreach (var solution in sourceFolder.Solutions.OrderBy(x => x.Name))
            {
                tabControlSolutions.Controls.Add(CreateSolutionTabPage(solution));
            }
        }

        private static void CloseTab(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Middle) return;

            var tabControl = (TabControl)sender;
            for (var i = 0; i < tabControl.TabPages.Count; i++)
            {
                if (!tabControl.GetTabRect(i).Contains(e.Location)) continue;

                tabControl.TabPages.RemoveAt(i);
                break;
            }
        }

        private static TabPage CreateProjectTabPage(Project project)
        {
            var projectRunnerControl = new RunnerControl
            {
                Dock = DockStyle.Fill,
                Location = new Point(3, 3),
                Name = project.Name
            };
            projectRunnerControl.SetStartInfo(project.Path);
            projectRunnerControl.RunningStateChanged += RunningStateChanged;

            var projectTabPage = new TabPage
            {
                Name = project.Name,
                Padding = new Padding(3),
                Text = project.Name,
                UseVisualStyleBackColor = true
            };

            projectTabPage.GotFocus += ProjectTabPage_GotFocus;

            projectTabPage.Controls.Add(projectRunnerControl);

            return projectTabPage;
        }

        private static void ProjectTabPage_GotFocus(object sender, EventArgs e)
        {
            var tabPage = (TabPage)sender;
            var runnerControl = (RunnerControl)tabPage.Controls[0];
            runnerControl.FocusOnOutput();
        }

        private static void RunningStateChanged(object sender, EventArgs args)
        {
            var runnerControl = (RunnerControl)sender;
            var tabPage = (TabPage)runnerControl.Parent;
            tabPage.ImageKey = runnerControl.Running ? "started" : "stopped";
        }

        private TabPage CreateSolutionTabPage(Solution solution)
        {
            var solutionTabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                ImageList = imageList,
                Location = new Point(3, 3),
                Name = solution.Name
            };

            solutionTabControl.MouseUp += CloseTab;

            foreach (var project in solution.Projects.OrderBy(x => x.Name))
            {
                var projectTabPage = CreateProjectTabPage(project);
                solutionTabControl.Controls.Add(projectTabPage);
                projectTabPage.ImageKey = "stopped";
            }

            var solutionTabPage = new TabPage
            {
                Name = solution.Name,
                Padding = new Padding(3),
                Text = solution.Name,
                UseVisualStyleBackColor = true
            };

            solutionTabPage.Controls.Add(solutionTabControl);

            return solutionTabPage;
        }
    }
}