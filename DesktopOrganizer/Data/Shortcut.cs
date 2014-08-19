using System.Windows.Input;

namespace DesktopOrganizer.Data
{
    public class Shortcut
    {
        public int Id { get; set; }
        public ModifierKeys Modifiers { get; set; }
        public Key Key { get; set; }

        public Shortcut(int id, ModifierKeys mod, Key key)
        {
            Id = id;
            Modifiers = mod;
            Key = key;
        }

        public Shortcut() : this(-1, ModifierKeys.None, Key.None) { }
        public Shortcut(Shortcut sc) : this(sc.Id, sc.Modifiers, sc.Key) { }

        public Shortcut Clone()
        {
            return new Shortcut(this);
        }

        public void Copy(Shortcut sc)
        {
            Id = sc.Id;
            Modifiers = sc.Modifiers;
            Key = sc.Key;
        }

        public bool IsEmpty()
        {
            return (Modifiers == ModifierKeys.None && Key == Key.None);
        }

        public bool Match(ModifierKeys modifiers, Key key)
        {
            return Modifiers == modifiers && Key == key;
        }

        public override string ToString()
        {
            if (Modifiers == ModifierKeys.None && Key == Key.None)
                return string.Empty;

            var mod = (Modifiers == ModifierKeys.None ? string.Empty : Modifiers.ToString());
            var key = (Key == Key.None ? string.Empty : Key.ToString());

            mod = mod.Replace(", ", "+").Replace("Control", "Ctrl").Replace("Windows", "Win");

            return (mod + "+" + key).Trim(new[] {'+'});
        }
    }
}
