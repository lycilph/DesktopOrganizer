using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Core.Data;
using DesktopOrganizer.Data;

namespace DesktopOrganizer.Resources
{
    public partial class ShortcutControl
    {
        private readonly List<Key> unavailable;
        private readonly Shortcut current_shortcut = new Shortcut();
        private bool captured;

        public Shortcut Shortcut
        {
            get { return (Shortcut)GetValue(ShortcutProperty); }
            set { SetValue(ShortcutProperty, value); }
        }
        public static readonly DependencyProperty ShortcutProperty =
            DependencyProperty.Register("Shortcut", typeof(Shortcut), typeof(ShortcutControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        
        public ShortcutControl()
        {
            InitializeComponent();
            layout_root.DataContext = this;

            unavailable = new List<Key> { Key.LeftShift, Key.RightShift, Key.LeftCtrl, Key.RightCtrl, Key.LeftAlt, Key.RightAlt, Key.LWin, Key.RWin, Key.Back };
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            UpdateShortcut(e);
        }

        private void OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            UpdateShortcut(e);

            if (!captured && !current_shortcut.IsEmpty())
            {
                captured = true;
                Shortcut = current_shortcut.Clone();
            }

            if (KeysPressed() == 0)
                captured = false;
        }

        private void UpdateShortcut(KeyEventArgs e)
        {
            current_shortcut.Modifiers = ModifierKeys.None;
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                current_shortcut.Modifiers |= ModifierKeys.Control;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                current_shortcut.Modifiers |= ModifierKeys.Shift;
            if ((Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                current_shortcut.Modifiers |= ModifierKeys.Alt;
            if (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin))
                current_shortcut.Modifiers |= ModifierKeys.Windows;

            current_shortcut.Key = (e.Key == Key.System || unavailable.Contains(e.Key) ? Key.None : e.Key);

            shortcut_textbox.Text = current_shortcut.ToString();
        }

        private static int KeysPressed()
        {
            return Enum.GetValues(typeof(Key))
                       .Cast<Key>()
                       .Skip(1)
                       .Count(Keyboard.IsKeyDown);
        }
    }
}
