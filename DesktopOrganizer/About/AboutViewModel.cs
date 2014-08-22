using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.About
{
    public class AboutViewModel : ReactiveObject
    {
        private List<Package> _Packages;
        public List<Package> Packages
        {
            get { return _Packages; }
            set { this.RaiseAndSetIfChanged(ref _Packages, value); }
        }

        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private string _Version;
        public string Version
        {
            get { return _Version; }
            set { this.RaiseAndSetIfChanged(ref _Version, value); }
        }

        public AboutViewModel()
        {
            var info = new AssemblyInfo(Assembly.GetExecutingAssembly());
            Title = info.ProductTitle;
            Name = info.Company;
            Version = info.Version;

            var config_file = Assembly.GetExecutingAssembly().GetManifestResourceNames().Single(s => s.EndsWith("packages.config"));
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(config_file))
            {
                var doc = new XmlDocument();
                doc.Load(stream);

                var nodes = doc.SelectNodes("//package");
                Packages = nodes.OfType<XmlNode>()
                                .Select(n => new Package { Name = n.Attributes["id"].Value, Version = n.Attributes["version"].Value })
                                .ToList();
            }
        }
    }
}
