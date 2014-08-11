using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace DesktopOrganizer.Shell.Utils
{
    public static class InputBindingParser
    {
        public static bool CanParse(string trigger_text)
        {
            return !string.IsNullOrWhiteSpace(trigger_text) && trigger_text.Contains("Shortcut");
        }

        public static TriggerBase CreateTrigger(string trigger_text)
        {
            var trigger_detail = trigger_text
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("Shortcut", string.Empty)
                .Trim();

            var mod_keys = ModifierKeys.None;

            var all_keys = trigger_detail.Split('+');
            var key = (Key)Enum.Parse(typeof(Key), all_keys.Last());

            foreach (var modifier_key in all_keys.Take(all_keys.Count() - 1))
            {
                mod_keys |= (ModifierKeys)Enum.Parse(typeof(ModifierKeys), modifier_key);
            }

            var key_binding = new KeyBinding(new InputBindingTrigger(), key, mod_keys);
            var trigger = new InputBindingTrigger { InputBinding = key_binding };
            return trigger;
        }
    }
}
