using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace dotnet.runner
{
    internal class SourceFolder
    {
        public SourceFolder(IEnumerable<string> folders)
        {
            var solutions = folders.SelectMany(x => new DirectoryInfo(x).GetFiles("*.sln", SearchOption.AllDirectories))
                .Select(ToSolution)
                .Distinct()
                .ToList();

            var allProjects = new List<string>();

            foreach (var solution in solutions.OrderByDescending(x => x.Directory.FullName.ToLowerInvariant()))
            {
                var projectFiles = solution.Directory
                    .GetFiles("*.csproj", SearchOption.AllDirectories)
                    .Where(x => !allProjects.Contains(x.FullName.ToLowerInvariant()))
                    .Where(IsNetcoreWebApp);

                foreach (var projectFile in projectFiles)
                {
                    allProjects.Add(projectFile.FullName.ToLowerInvariant());
                    solution.Projects.Add(ToProject(projectFile));
                }
            }

            Solutions = solutions.Where(x => x.Projects.Count > 0).ToList();
        }

        public IEnumerable<Solution> Solutions { get; }

        private static Solution ToSolution(FileInfo file)
        {
            return new Solution { Name = Path.GetFileNameWithoutExtension(file.Name), Directory = file.Directory };
        }

        private static Project ToProject(FileInfo file)
        {
            return new Project { Name = Path.GetFileNameWithoutExtension(file.Name), Path = file.DirectoryName };
        }

        private static bool IsNetcoreWebApp(FileInfo file)
        {
            try
            {
                var projectXml = XDocument.Load(file.OpenText());
                var webProjectTargetFramework = projectXml.XPathSelectElement("/Project[@Sdk='Microsoft.NET.Sdk.Web']/PropertyGroup/TargetFramework");
                return webProjectTargetFramework != null && webProjectTargetFramework.Value.ToLowerInvariant().StartsWith("netcoreapp");
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    internal class Solution
    {
        public Solution()
        {
            Projects = new List<Project>();
        }

        public string Name { get; set; }

        public DirectoryInfo Directory { get; set; }

        public List<Project> Projects { get; }

        public override bool Equals(object obj)
        {
            return obj is Solution other && string.Equals(Directory?.FullName, other.Directory?.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // ReSharper disable NonReadonlyMemberInGetHashCode
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Directory != null ? Directory.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Projects != null ? Projects.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    internal class Project
    {
        public string Name { get; set; }

        public string Path { get; set; }
    }
}