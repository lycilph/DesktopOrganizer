namespace DesktopOrganizer.Main
{
    public class CaptureCommand
    {
        public enum CaptureKind { Windows, Icons }

        public CaptureKind Kind { get; set; }
    }
}
