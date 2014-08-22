namespace Core.Data
{
    public interface ILayout
    {
        string Name { get; set; }
        Shortcut Shortcut { get; set; }
    }
}
