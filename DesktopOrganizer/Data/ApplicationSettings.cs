using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Core.Data;
using DesktopOrganizer.Utils;
using Newtonsoft.Json;
using ReactiveUI;
using WindowManager = DesktopOrganizer.Utils.WindowManager;

namespace DesktopOrganizer.Data
{
    public class ApplicationSettings
    {
        private const string TaskName = @"LyCilph\DesktopOrganizerStart";
        private const string TaskInputFile = "Startup.xml";
        private readonly KeyboardHook keyboard_hook = new KeyboardHook();

        public List<string> ExcludedProcesses { get; set; }
        public bool SuppressShortcuts { get; set; }
        public ReactiveList<ILayout> Layouts { get; private set; }

        [JsonIgnore]
        public bool LaunchOnWindowsStart
        {
            get { return TaskScheduler.HasTask(TaskName); }
            set { SetLaunchOnWindowsStart(value); }
        }

        public ApplicationSettings()
        {
            Layouts = new ReactiveList<ILayout>();
            keyboard_hook.KeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (SuppressShortcuts) return;

            var layout = Layouts.SingleOrDefault(l => l.Shortcut.Match(args.Modifier, args.GetWpfKey()));
            if (layout == null) return;

            if (layout is Layout<Program>)
                WindowManager.ApplyLayout(layout as Layout<Program>);
            if (layout is Layout<Icon>)
                IconManagerWrapper.ApplyLayout(layout as Layout<Icon>);
        }

        private void SetLaunchOnWindowsStart(bool enable)
        {
            var has_task = LaunchOnWindowsStart;
            if (enable == has_task) return;
            
            if (enable)
                TaskScheduler.CreateTask(TaskName, TaskInputFile);
            else
                TaskScheduler.DeleteTask(TaskName);
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
