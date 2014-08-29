using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Core.Data;
using DesktopOrganizer.Utils;
using Newtonsoft.Json;
using NLog;
using ReactiveUI;
using LogManager = NLog.LogManager;
using WindowManager = DesktopOrganizer.Utils.WindowManager;

namespace DesktopOrganizer.Data
{
    [Export(typeof(ApplicationSettings))]
    public class ApplicationSettings
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string TASK_NAME = @"LyCilph\DesktopOrganizerStart";
        private const string TASK_INPUT_FILE = "Startup.xml";
        private readonly KeyboardHook keyboard_hook = new KeyboardHook();

        public List<string> ExcludedProcesses { get; set; }
        public ReactiveList<ILayout> Layouts { get; private set; }

        [JsonIgnore]
        public bool SuppressShortcuts { get; set; }

        [JsonIgnore]
        public bool LaunchOnWindowsStart
        {
            get { return TaskScheduler.HasTask(TASK_NAME); }
            set { SetLaunchOnWindowsStart(value); }
        }

        public ApplicationSettings()
        {
            logger.Trace("Create");

            Layouts = new ReactiveList<ILayout>();
            keyboard_hook.KeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, KeyPressedEventArgs args)
        {
            if (SuppressShortcuts) return;

            var layout = Layouts.SingleOrDefault(l => l.Shortcut.Match(args.Modifier, args.GetWpfKey()));
            if (layout == null) return;

            logger.Trace("Apply layout {0} [{1} - {2}]", layout.Name, layout.Shortcut, layout.GetType());

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
                TaskScheduler.CreateTask(TASK_NAME, TASK_INPUT_FILE);
            else
                TaskScheduler.DeleteTask(TASK_NAME);
        }

        public ApplicationSettings Reset()
        {
            logger.Trace("Reset");

            ExcludedProcesses = new List<string> {"clover", "buildnotificationapp"};
            SetLaunchOnWindowsStart(false);
            return this;
        }

        public void ApplyShortcuts()
        {
            keyboard_hook.UnregisterAll();
            Layouts.Where(l => !l.Shortcut.IsEmpty()).Apply(l => keyboard_hook.RegisterHotKey(l.Shortcut));
        }

        public bool IsShortcutUsed(Shortcut sc, ILayout layout)
        {
            return !sc.IsEmpty() && Layouts.Any(l => l != layout && l.Shortcut.Match(sc));
        }

        public void Add(ILayout layout)
        {
            // Do this first, in case this throws an error
            if (!layout.Shortcut.IsEmpty())
                keyboard_hook.RegisterHotKey(layout.Shortcut);

            Layouts.Add(layout);
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

        public void Move(ILayout source, int insert_index)
        {
            var index = Layouts.IndexOf(source);
            Layouts.Remove(source);
            if (index < insert_index)
                --insert_index;
            Layouts.Insert(insert_index, source);
        }

        private static string GetFilename()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrWhiteSpace(dir))
                throw new InvalidDataException();

            return Path.Combine(dir, "settings.txt");
        }

        public void Load()
        {
            logger.Trace("Load");
            
            var filename = GetFilename();
            if (!File.Exists(filename))
            {
                logger.Trace("Not settings found");
                Reset();
                return;
            }

            var json = File.ReadAllText(filename);
            var settings = JsonConvert.DeserializeObject<ApplicationSettings>(json, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });

            ExcludedProcesses = new List<string>(settings.ExcludedProcesses);
            Layouts = new ReactiveList<ILayout>(settings.Layouts);
        }

        public void Save()
        {
            logger.Trace("Save");

            var filename = GetFilename();
            var json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Objects});
            File.WriteAllText(filename, json);
        }
    }
}
