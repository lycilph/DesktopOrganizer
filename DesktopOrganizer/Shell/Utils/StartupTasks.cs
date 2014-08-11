using Caliburn.Micro;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DesktopOrganizer.Shell.Utils
{
    public delegate void StartupTask();

    public class StartupTasks
    {
        [Export(typeof(StartupTask))]
        public void ApplyBindingScopeOverride()
        {
            var get_named_elements = BindingScope.GetNamedElements;
            BindingScope.GetNamedElements = o =>
            {
                var metro_window = o as MetroWindow;
                if (metro_window != null)
                    return ResolveMetroWindow(o, get_named_elements);

                return get_named_elements(o);
            };
        }

        private IEnumerable<FrameworkElement> ResolveMetroWindow(DependencyObject o, Func<DependencyObject, IEnumerable<FrameworkElement>> get_named_elements)
        {
            var list = new List<FrameworkElement>(get_named_elements(o));
            var type = o.GetType();
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                             .Where(f => f.DeclaringType == type);
            var flyouts = fields.Where(f => f.FieldType == typeof(FlyoutsControl))
                                .Select(f => f.GetValue(o))
                                .Cast<FlyoutsControl>();
            var commands = fields.Where(f => f.FieldType == typeof(WindowCommands))
                                .Select(f => f.GetValue(o))
                                .Cast<WindowCommands>();
            list.AddRange(flyouts);
            list.AddRange(commands);

            if (!flyouts.Any())
            {
                var contained_flyouts = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                            .Where(p => p.PropertyType == typeof(FlyoutsControl))
                                            .Select(p => p.GetValue(o))
                                            .Cast<FlyoutsControl>();
                foreach (var flyout in contained_flyouts)
                {
                    if (flyout != null)
                        list.AddRange(get_named_elements(flyout));
                }
            }

            if (!commands.Any())
            {
                var contained_commands = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                             .Where(p => p.PropertyType == typeof(WindowCommands))
                                             .Select(p => p.GetValue(o))
                                             .Cast<WindowCommands>();
                foreach (var command in contained_commands)
                {
                    if (command != null)
                        list.AddRange(get_named_elements(command));
                }
            }

            return list;
        }

        [Export(typeof(StartupTask))]
        public void ApplyParserOverride()
        {
            var current_parser = Parser.CreateTrigger;
            Parser.CreateTrigger = (target, trigger_text) => InputBindingParser.CanParse(trigger_text)
                                                             ? InputBindingParser.CreateTrigger(trigger_text)
                                                             : current_parser(target, trigger_text);
        }
    }
}
