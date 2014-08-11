using System;
using System.Collections.Generic;
using System.Linq;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell.ViewModels;
using DesktopOrganizer.Utils;
using ReactiveUI;

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

        private List<ProgramViewModel> _Programs;
        public List<ProgramViewModel> Programs
        {
            get { return _Programs; }
            set { this.RaiseAndSetIfChanged(ref _Programs, value); }
        }

        public CaptureWindowsViewModel(IShell shell, ApplicationSettings application_settings, Layout<Program> layout, bool is_editing) : base(shell, application_settings)
        {
            this.layout = layout;
            this.is_editing = is_editing;

            LayoutName = layout.Name;
            Programs = layout.Items.Select(p => new ProgramViewModel(p)).ToList();
        }

        public void Capture()
        {
            Predicate<Window> filter = (w => application_settings.ExcludedProcesses.Contains(w.ProcessName, StringComparer.InvariantCultureIgnoreCase));
            Programs = WindowManager.GetPrograms(filter)
                                    .Select(p => new ProgramViewModel(p))
                                    .ToList();
        }

        public void Back()
        {
            shell.Back();
        }

        public void Ok()
        {
            layout.Name = LayoutName;
            layout.Items = new List<Program>(Programs.Select(p => p.AssociatedObject).ToList());

            if (!is_editing)
                application_settings.ProgramLayouts.Add(layout);

            shell.Back();
        }

        public void Cancel()
        {
            shell.Back();
        }
    }
}
