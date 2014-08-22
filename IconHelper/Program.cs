using System.IO;
using System.IO.Pipes;
using Core;
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
                if (input == "get")
                {
                    var icons = IconManager.GetIconsPositions();
                    var output = JsonConvert.SerializeObject(icons);
                    sw.WriteLine(output);
                    sw.Flush();
                    client.WaitForPipeDrain();    
                }
                else
                    throw new InvalidDataException();
            }
        }
    }
}
