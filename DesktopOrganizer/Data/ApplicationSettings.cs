using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Caliburn.Micro;
using Core.Data;
using DesktopOrganizer.Utils;
using Microsoft.Win32;
using Newtonsoft.Json;
using ReactiveUI;
using WindowManager = DesktopOrganizer.Utils.WindowManager;

namespace DesktopOrganizer.Data
{
    public class ApplicationSettings
    {
        private readonly KeyboardHook keyboard_hook = new KeyboardHook();

        public List<string> ExcludedProcesses { get; set; }
        public bool SuppressShortcuts { get; set; }
        //public ReactiveList<Layout<Program>> ProgramLayouts { get; set; }
        //public ReactiveList<Layout<Icon>> IconLayouts { get; set; }
        public ReactiveList<ILayout> Layouts { get; private set; }

        [JsonIgnore]
        public bool LaunchOnWindowsStart
        {
            get { return IsLaunchingOnWindowsStart(); }
            set { SetLaunchOnWindowsStart(value); }
        }

        public ApplicationSettings()
        {
            //ProgramLayouts = new ReactiveList<Layout<Program>>();
            //IconLayouts = new ReactiveList<Layout<Icon>>();
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

        private static void SetLaunchOnWindowsStart(bool state)
        {
            var is_launching = IsLaunchingOnWindowsStart();
            if (state == is_launching) return;

            var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (key == null)
                throw new Exception();

            var info = new AssemblyInfo(Assembly.GetExecutingAssembly());
            if (state)
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
            //ProgramLayouts.Apply(l => keyboard_hook.RegisterHotKey(l.Shortcut));
        }

        //public void AddProgramLayout(Layout<Program> layout)
        //{
        //    ProgramLayouts.Add(layout);
        //    keyboard_hook.RegisterHotKey(layout.Shortcut);
        //}

        //public void RemoveProgramLayout(Layout<Program> layout)
        //{
        //    ProgramLayouts.Remove(layout);
        //    keyboard_hook.UnregisterHotKey(layout.Shortcut);
        //}

        //public void UpdateProgramLayout(Layout<Program> old_layout, Layout<Program> new_layout)
        //{
        //    var index = ProgramLayouts.IndexOf(old_layout);
        //    ProgramLayouts[index] = new_layout;

        //    keyboard_hook.UnregisterHotKey(old_layout.Shortcut);
        //    keyboard_hook.RegisterHotKey(new_layout.Shortcut);
        //}

        //public void AddIconLayout(Layout<Icon> layout)
        //{
        //    IconLayouts.Add(layout);
        //    keyboard_hook.RegisterHotKey(layout.Shortcut);
        //}

        //public void RemoveIconLayout(Layout<Icon> layout)
        //{
        //    IconLayouts.Remove(layout);
        //    keyboard_hook.UnregisterHotKey(layout.Shortcut);
        //}

        //public void UpdateIconLayout(Layout<Icon> old_layout, Layout<Icon> new_layout)
        //{
        //    var index = IconLayouts.IndexOf(old_layout);
        //    IconLayouts[index] = new_layout;

        //    keyboard_hook.UnregisterHotKey(old_layout.Shortcut);
        //    keyboard_hook.RegisterHotKey(new_layout.Shortcut);
        //}

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
            var settings = JsonConvert.DeserializeObject<ApplicationSettings>(json);
            settings.ApplyShortcuts();
            return settings;
        }

        public static void Save(ApplicationSettings settings)
        {
            var filename = GetFilename();
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filename, json);
        }
    }
}
