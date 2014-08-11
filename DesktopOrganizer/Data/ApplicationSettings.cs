using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using ReactiveUI;

namespace DesktopOrganizer.Data
{
    public class ApplicationSettings
    {
        public List<string> ExcludedProcesses { get; set; }
        public ReactiveList<Layout<Program>> ProgramLayouts { get; set; }

        public ApplicationSettings()
        {
            ProgramLayouts = new ReactiveList<Layout<Program>>();
        }

        public ApplicationSettings Reset()
        {
            ExcludedProcesses = new List<string> {"clover"};
            return this;
        }

        private static string GetFilename()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(dir))
                throw new InvalidDataException();

            return Path.Combine(dir, "settings.txt");
        }

        public static ApplicationSettings Load()
        {
            var filename = GetFilename();
            if (!File.Exists(filename))
                return new ApplicationSettings().Reset();

            var json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<ApplicationSettings>(json);
        }

        public static void Save(ApplicationSettings settings)
        {
            var filename = GetFilename();
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
    }
}
