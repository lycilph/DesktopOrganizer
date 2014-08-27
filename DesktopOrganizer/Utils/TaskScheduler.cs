using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Caliburn.Micro;

namespace DesktopOrganizer.Utils
{
    public static class TaskScheduler
    {
        public static bool HasTask(string name)
        {
            var p = Process.Start(new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = string.Format("/Query /TN {0} /nh /fo csv", name),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                UseShellExecute = false
            });
            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            
            if (string.IsNullOrWhiteSpace(output)) return false;

            var columns = output.Split(new[] { ',' });
            var task_output = columns[0].Replace("\"", "").Trim().ToLowerInvariant();
            var task_status = columns[2].Replace("\"", "").Trim().ToLowerInvariant();
            return (task_output == name.ToLowerInvariant() && task_status == "ready");
        }

        public static void CreateTask(string name, string input_file)
        {
            using (var file = new TempFile())
            {
                var user_id = string.Format("{0}\\{1}", Environment.UserDomainName, Environment.UserName);
                var app = Assembly.GetExecutingAssembly().Location;
                var input = Path.Combine(Path.GetDirectoryName(app), input_file);

                var doc = new XmlDocument();
                doc.Load(input);

                var manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("ns", doc.DocumentElement.NamespaceURI);

                // Set user name nodes
                doc.DocumentElement
                   .SelectNodes("//ns:UserId", manager)
                   .ToList()
                   .Apply(n => n.InnerText = user_id);

                // Set application name node
                doc.DocumentElement
                    .SelectSingleNode("//ns:Command", manager)
                    .InnerText = "\"" + app + "\"";

                doc.Save(file.Name);

                var p = Process.Start(new ProcessStartInfo
                {
                    FileName = "schtasks.exe",
                    Arguments = "/Create /TN LyCilph\\DesktopOrganizerStart /XML " + file.Name,
                    CreateNoWindow = true,
                    UseShellExecute = false
                });
                p.WaitForExit();
            }
        }

        public static void DeleteTask(string name)
        {
            var p = Process.Start(new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = string.Format("/Delete /TN {0} /F", name),
                CreateNoWindow = true,
                UseShellExecute = false
            });
            p.WaitForExit();
        }
    }
}
