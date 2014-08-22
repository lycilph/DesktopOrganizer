using System.Linq;
using Core.Data;
using DesktopOrganizer.Data;
using DesktopOrganizer.Shell.ViewModels;
using DesktopOrganizer.Utils;
using ReactiveUI;

namespace DesktopOrganizer.CaptureIcons
{
    //public class CaptureIconsViewModel : ViewModelBase
    //{
    //    private readonly Layout<Icon> layout;
    //    private readonly bool is_editing;

    //    private string _LayoutName;
    //    public string LayoutName
    //    {
    //        get { return _LayoutName; }
    //        set { this.RaiseAndSetIfChanged(ref _LayoutName, value); }
    //    }

    //    private Shortcut _Shortcut;
    //    public Shortcut Shortcut
    //    {
    //        get { return _Shortcut; }
    //        set { this.RaiseAndSetIfChanged(ref _Shortcut, value); }
    //    }

    //    private IconViewModel _CurrentIcon;
    //    public IconViewModel CurrentIcon
    //    {
    //        get { return _CurrentIcon; }
    //        set { this.RaiseAndSetIfChanged(ref _CurrentIcon, value); }
    //    }

    //    private ReactiveList<IconViewModel> _Icons;
    //    public ReactiveList<IconViewModel> Icons
    //    {
    //        get { return _Icons; }
    //        set { this.RaiseAndSetIfChanged(ref _Icons, value); }
    //    }

    //    public CaptureIconsViewModel(IShell shell, ApplicationSettings application_settings, Layout<Icon> layout, bool is_editing) : base(shell, application_settings)
    //    {
    //        this.layout = layout;
    //        this.is_editing = is_editing;

    //        LayoutName = layout.Name;
    //        Shortcut = layout.Shortcut.Clone();
    //        Icons = layout.Items.Select(i => new IconViewModel(i)).ToReactiveList();
    //    }

    //    protected override void OnActivate()
    //    {
    //        base.OnActivate();
    //        application_settings.SuppressShortcuts = true;
    //    }

    //    protected override void OnDeactivate(bool close)
    //    {
    //        base.OnDeactivate(close);

    //        if (close)
    //            application_settings.SuppressShortcuts = false;
    //    }

    //    public void Capture()
    //    {
    //        Icons = IconManagerWrapper.GetIcons()
    //                                  .Select(i => new IconViewModel(i))
    //                                  .ToReactiveList();
    //    }

    //    public void Back()
    //    {
    //        shell.Back();
    //    }

    //    public void Ok()
    //    {
    //        var new_layout = new Layout<Icon>
    //        {
    //            Name = LayoutName,
    //            Shortcut = Shortcut.Clone(),
    //            Items = Icons.Select(p => p.AssociatedObject).ToList()
    //        };

    //        //if (is_editing)
    //        //    application_settings.UpdateIconLayout(layout, new_layout);
    //        //else
    //        //    application_settings.AddIconLayout(new_layout);

    //        shell.Back();
    //    }

    //    public void Cancel()
    //    {
    //        shell.Back();
    //    }

    //    public void Delete()
    //    {
    //        if (CurrentIcon != null)
    //            Icons.Remove(CurrentIcon);
    //    }
    //}
}
