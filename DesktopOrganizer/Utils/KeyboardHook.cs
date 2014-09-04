using Core;
using NLog;
using System;
using System.Windows.Forms;
using System.Windows.Input;
using Shortcut = Core.Data.Shortcut;

namespace DesktopOrganizer.Utils
{
    // http://www.liensberger.it/web/blog/?p=207
    public sealed class KeyboardHook : IDisposable
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        // Represents the window that is used internally to get the messages.
        private sealed class Window : NativeWindow, IDisposable
        {
            private const int WM_HOTKEY = 0X0312;

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            public Window()
            {
                // Create the handle for the window.
                CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // Check if we got a hot key pressed.
                if (m.Msg != WM_HOTKEY)
                    return;

                // get the keys.
                var key = (Keys) (((int) m.LParam >> 16) & 0xFFFF);
                var modifier = (ModifierKeys) ((int) m.LParam & 0xFFFF);

                // invoke the event to notify the parent.
                var handle = KeyPressed;
                if (handle != null)
                    handle(this, new KeyPressedEventArgs(modifier, key));
            }

            public void Dispose()
            {
                DestroyHandle();
            }
        }

        private readonly Window internal_window = new Window();
        private int current_id;

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public KeyboardHook()
        {
            // register the event of the inner native window.
            internal_window.KeyPressed += (s, args) =>
            {
                var handle = KeyPressed;
                if (handle != null)
                    handle(this, args);
            };
        }

        public void RegisterHotKey(Shortcut sc)
        {
            var win_forms_key = (Keys)KeyInterop.VirtualKeyFromKey(sc.Key);
            sc.Id = RegisterHotKey(sc.Modifiers, win_forms_key);
        }

        public int RegisterHotKey(ModifierKeys modifier, Keys key)
        {
            current_id = current_id + 1;

            logger.Trace("Registering shortcut [{0} - {1}]", modifier, key);

            if (!User32.RegisterHotKey(internal_window.Handle, current_id, (uint) modifier, (uint) key))
            {
                logger.Error("Registration failded");
                throw new InvalidOperationException("Couldn’t register the hot key\n\rMay already be used by another process");
            }

            return current_id;
        }

        public void UnregisterHotKey(Shortcut sc)
        {
            UnregisterHotKey(sc.Id);
        }

        public void UnregisterHotKey(int id)
        {
            if (!User32.UnregisterHotKey(internal_window.Handle, id))
                throw new InvalidOperationException("Couldn’t unregister the hot key");
        }

        public void UnregisterAll()
        {
            for (var i = current_id; i > 0; i--)
                User32.UnregisterHotKey(internal_window.Handle, i);
            current_id = 0;
        }

        public void Dispose()
        {
            UnregisterAll();
            // Dispose the inner native window.
            internal_window.Dispose();
        }
    }

    public class KeyPressedEventArgs : EventArgs
    {
        public ModifierKeys Modifier { get; private set; }
        public Keys Key { get; private set; }
     
        internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
        {
            Modifier = modifier;
            Key = key;
        }

        public Key GetWpfKey()
        {
            return KeyInterop.KeyFromVirtualKey((int)Key);
        }
    }
}
