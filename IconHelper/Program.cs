using System.IO;
using System.IO.Pipes;
using System.Windows;
using Core;
using Core.Data;
using Newtonsoft.Json;

namespace IconHelper
{
    class Program
    {
        static void Main()
        {
            using (var client = new NamedPipeClientStream(".", "DesktopOrganizerPipe", PipeDirection.InOut))
            {
                client.Connect();

                var sw = new StreamWriter(client);
                var sr = new StreamReader(client);

                var input = sr.ReadLine();
                switch (input)
                {
                    case "get":
                        {
                            var icons = IconManager.GetIconsPositions();
                            var output = JsonConvert.SerializeObject(icons);
                            sw.WriteLine(output);
                            sw.Flush();
                            client.WaitForPipeDrain();
                        }
                        break;
                    case "set":
                        {
                            var json = sr.ReadLine();
                            var layout = JsonConvert.DeserializeObject<Layout<Icon>>(json);
                            IconManager.ApplyLayout(layout);
                        }
                        break;
                    default:
                        MessageBox.Show(string.Format("Unknown command [{0}]", input));
                        break;
                }
            }
        }
    }
}
