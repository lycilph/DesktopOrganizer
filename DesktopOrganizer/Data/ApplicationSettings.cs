using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Core.Data;
using DesktopOrganizer.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using ReactiveUI;

namespace DesktopOrganizer.Data
{
    public class ApplicationSettings
    {
        private readonly KeyboardHook keyboard_hook = new KeyboardHook();

        public List<string> ExcludedProcesses { get; set; }
        public bool SuppressShortcuts { get; set; }
        public ReactiveList<ILayout> Layouts { get; private set; }

        [JsonIgnore]
        public bool LaunchOnWindowsStart
        {
            get { return IsLaunchingOnWindowsStart(); }
            set { SetLaunchOnWindowsStart(value); }
        }

        public ApplicationSettings()
        {
            Layouts = new ReactiveList<ILayout>();
            keyboard_hook.KeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            //if (SuppressShortcuts) return;

            //var window_layout = ProgramLayouts.SingleOrDefault(l => l.Shortcut.Match(args.Modifier, args.GetWpfKey()));
            //if (window_layout != null)
            //    WindowManager.ApplyLayout(window_layout);

            //var icon_layout = IconLayouts.SingleOrDefault(l => l.Shortcut.Match(args.Modifier, args.GetWpfKey()));
            //if (icon_layout != null)
            //    IconManagerWrapper.ApplyLayout(icon_layout);
        }

        private static bool IsLaunchingOnWindowsStart()
        {
            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            if (key == null)
                throw new Exception();

            var info = new AssemblyInfo(Assembly.GetExecutingAssembly());
            return key.GetValueNames().Any(n => n == info.ProductTitle);
        }

        private static void SetLaunchOnWindowsStart(bool enable)
        {
            var is_enabled = IsLaunchingOnWindowsStart();
            if (enable == is_enabled) return;

            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (key == null)
                throw new Exception();

            var info = new AssemblyInfo(Assembly.GetExecutingAssembly());
            if (enable)
                key.SetValue(info.ProductTitle, Assembly.GetExecutingAssembly().Location, RegistryValueKind.String);
            else
                key.DeleteValue(info.ProductTitle, true);
        }

        public ApplicationSettings Reset()
        {
            ExcludedProcesses = new List<string> {"clover", "buildnotificationapp"};
            SetLaunchOnWindowsStart(false);
            return this;
        }

        public void ApplyShortcuts()
        {
            keyboard_hook.UnregisterAll();
            Layouts.Where(l => !l.Shortcut.IsEmpty()).Apply(l => keyboard_hook.RegisterHotKey(l.Shortcut));
        }

        public void Add(ILayout layout)
        {
            Layouts.Add(layout);

            if (!layout.Shortcut.IsEmpty())
                keyboard_hook.RegisterHotKey(layout.Shortcut);
        }

        public void Remove(ILayout layout)
        {
            Layouts.Remove(layout);

            if (!layout.Shortcut.IsEmpty())
                keyboard_hook.UnregisterHotKey(layout.Shortcut);
        }

        public void Update(ILayout new_layout, ILayout old_layout)
        {
            var index = Layouts.IndexOf(old_layout);
            Layouts[index] = new_layout;

            if (!old_layout.Shortcut.IsEmpty())
                keyboard_hook.UnregisterHotKey(old_layout.Shortcut);
            if (!new_layout.Shortcut.IsEmpty())
                keyboard_hook.RegisterHotKey(new_layout.Shortcut);
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
            var settings = JsonConvert.DeserializeObject<ApplicationSettings>(json, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
            settings.ApplyShortcuts();
            return settings;
        }

        public void Save()
        {
            var filename = GetFilename();
            var json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
            File.WriteAllText(filename, json);
        }
    }
}
