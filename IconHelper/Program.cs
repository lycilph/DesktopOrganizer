using System;
using System.IO;
using System.IO.Pipes;
using System.Windows;
using Core;
using Core.Data;
using Newtonsoft.Json;
using NLog;

namespace IconHelper
{
    class Program
    {
        protected static readonly Logger logger = LogManager.GetCurrentClassLogger();

        static void Main()
        {
            try
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
                            logger.Trace("IconHelper - getting icon positions");
                            var icons = IconManager.GetIconsPositions();
                            var output = JsonConvert.SerializeObject(icons);
                            sw.WriteLine(output);
                            sw.Flush();
                            client.WaitForPipeDrain();
                        }
                            break;
                        case "set":
                        {
                            logger.Trace("IconHelper - applying layout");
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
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }
    }
}
