using System.Collections.Generic;
using System.ComponentModel.Composition;
using DesktopOrganizer.Data;
using DesktopOrganizer.Utils;

namespace DesktopOrganizer.Main
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : ViewModelBase
    {
        public List<CaptureCommand> CaptureCommands { get; set; }

        //private int _Index = 1;
        //public int Index
        //{
        //    get { return _Index; }
        //    set { this.RaiseAndSetIfChanged(ref _Index, value); }
        //}

        //private ProgramLayoutViewModel _CurrentProgramLayout;
        //public ProgramLayoutViewModel CurrentProgramLayout
        //{
        //    get { return _CurrentProgramLayout; }
        //    set { this.RaiseAndSetIfChanged(ref _CurrentProgramLayout, value); }
        //}

        //public IReactiveDerivedList<ProgramLayoutViewModel> ProgramLayouts { get; set; }

        //private readonly ObservableAsPropertyHelper<bool> _CanApplyProgramLayout;
        //public bool CanApplyProgramLayout
        //{
        //    get { return _CanApplyProgramLayout.Value; }
        //}

        //private readonly ObservableAsPropertyHelper<bool> _CanEditProgramLayout;
        //public bool CanEditProgramLayout
        //{
        //    get { return _CanEditProgramLayout.Value; }
        //}

        //private readonly ObservableAsPropertyHelper<bool> _CanDeleteProgramLayout;
        //public bool CanDeleteProgramLayout
        //{
        //    get { return _CanDeleteProgramLayout.Value; }
        //}

        [ImportingConstructor]
        public MainViewModel(ApplicationSettings application_settings) : base(application_settings)
        {
            CaptureCommands = new List<CaptureCommand>
            {
                new CaptureCommand {Title = "Windows"},
                new CaptureCommand {Title = "Icon"}
            };

            //ProgramLayouts = application_settings.ProgramLayouts.CreateDerivedCollection(p => new ProgramLayoutViewModel(p));

            //var obs = this.WhenAny(x => x.CurrentProgramLayout, x => x.Value != null);
            //_CanApplyProgramLayout = obs.ToProperty(this, x => x.CanApplyProgramLayout);
            //_CanEditProgramLayout = obs.ToProperty(this, x => x.CanEditProgramLayout);
            //_CanDeleteProgramLayout = obs.ToProperty(this, x => x.CanDeleteProgramLayout);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            //if (ProgramLayouts.Any())
            //    CurrentProgramLayout = ProgramLayouts.First();
        }

        public void Capture(CaptureCommand capture_command)
        {
            
        }

        //public void NewProgramLayout()
        //{
        //    var layout = new Layout<Program> {Name = "Default"};
        //    var window = new CaptureWindowsViewModel(shell, application_settings, layout, false);
        //    shell.Show(window);
        //}

        //public void ApplyProgramLayout()
        //{
        //    WindowManager.ApplyLayout(CurrentProgramLayout.AssociatedObject);
        //}

        //public void EditProgramLayout()
        //{
        //    var window = new CaptureWindowsViewModel(shell, application_settings, CurrentProgramLayout.AssociatedObject, true);
        //    shell.Show(window);
        //}

        //public void DeleteProgramLayout()
        //{
        //    application_settings.ProgramLayouts.Remove(CurrentProgramLayout.AssociatedObject);
        //    CurrentProgramLayout = null;
        //}

        //public void NewIconLayout()
        //{
        //    var layout = new Layout<Icon> {Name = "Default"};
        //    var window = new CaptureIconsViewModel(shell, application_settings, layout, false);
        //    shell.Show(window);
        //}
    }
}
