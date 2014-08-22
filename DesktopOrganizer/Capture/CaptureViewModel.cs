using System.ComponentModel.Composition;
using DesktopOrganizer.Data;
using DesktopOrganizer.Utils;

namespace DesktopOrganizer.Capture
{
    [Export(typeof(CaptureViewModel))]
    public class CaptureViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public CaptureViewModel(ApplicationSettings application_settings) : base(application_settings)
        {
        }
    }
}
