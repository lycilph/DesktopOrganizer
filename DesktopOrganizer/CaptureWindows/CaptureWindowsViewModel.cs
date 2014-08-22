using System;
using System.Linq;
using System.Windows;
using Core.Data;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell.ViewModels;
using DesktopOrganizer.Utils;
using ReactiveUI;
using Window = Core.Data.Window;

namespace DesktopOrganizer.CaptureWindows
{
    public class CaptureWindowsViewModel : ViewModelBase
    {
        private readonly Layout<Program> layout;
        private readonly bool is_editing;

        private string _LayoutName;
        public string LayoutName
        {
            get { return _LayoutName; }
            set { this.RaiseAndSetIfChanged(ref _LayoutName, value); }
        }

        private Shortcut _Shortcut;
        public Shortcut Shortcut
        {
            get { return _Shortcut; }
            set { this.RaiseAndSetIfChanged(ref _Shortcut, value); }
        }

        private ProgramViewModel _CurrentProgram;
        public ProgramViewModel CurrentProgram
        {
            get { return _CurrentProgram; }
            set { this.RaiseAndSetIfChanged(ref _CurrentProgram, value); }
        }

        private ReactiveList<ProgramViewModel> _Programs;
        public ReactiveList<ProgramViewModel> Programs
        {
            get { return _Programs; }
            set { this.RaiseAndSetIfChanged(ref _Programs, value); }
        }

        public CaptureWindowsViewModel(IShell shell, ApplicationSettings application_settings, Layout<Program> layout, bool is_editing) : base(shell, application_settings)
        {
            this.layout = layout;
            this.is_editing = is_editing;

            LayoutName = layout.Name;
            Shortcut = layout.Shortcut.Clone();
            Programs = layout.Items.Select(p => new ProgramViewModel(p)).ToReactiveList();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            application_settings.SuppressShortcuts = true;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            if (close)
                application_settings.SuppressShortcuts = false;
        }

        public void Capture()
        {
            try
            {
                Predicate<Window> filter = (w => application_settings.ExcludedProcesses.Contains(w.ProcessName, StringComparer.InvariantCultureIgnoreCase));
                Programs = WindowManager.GetPrograms(filter)
                                        .Select(p => new ProgramViewModel(p))
                                        .ToReactiveList();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void Back()
        {
            shell.Back();
        }

        public void Ok()
        {
            var new_layout = new Layout<Program>
            {
                Name = LayoutName,
                Shortcut = Shortcut.Clone(),
                Items = Programs.Select(p => p.AssociatedObject).ToList()
            };

            //if (is_editing)
            //    application_settings.UpdateProgramLayout(layout, new_layout);
            //else
            //    application_settings.AddProgramLayout(new_layout);

            shell.Back();
        }

        public void Cancel()
        {
            shell.Back();
        }

        public void Delete()
        {
            if (CurrentProgram != null)
                Programs.Remove(CurrentProgram);
        }
    }
}
