using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using Core.Data;
using Newtonsoft.Json;

namespace DesktopOrganizer.Utils
{
    public static class IconManagerWrapper
    {
        public static IEnumerable<Icon> GetIcons()
        {
            var proc = Process.Start(new ProcessStartInfo("IconHelper.exe")
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            if (proc == null)
                throw new Exception();

            string text;
            using (var server = new NamedPipeServerStream("DesktopOrganizerPipe", PipeDirection.InOut))
            {
                try
                {
                    server.WaitForConnection();

                    var sw = new StreamWriter(server);
                    var sr = new StreamReader(server);

                    sw.WriteLine("get");
                    sw.Flush();
                    server.WaitForPipeDrain();

                    text = sr.ReadLine();
                }
                finally
                {
                    server.WaitForPipeDrain();
                    if (server.IsConnected)
                        server.Disconnect();
                }
            }

            proc.WaitForExit();
            return JsonConvert.DeserializeObject<List<Icon>>(text);
        }

        public static void ApplyLayout(Layout<Icon> layout)
        {
            var proc = Process.Start(new ProcessStartInfo("IconHelper.exe")
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            if (proc == null)
                throw new Exception();

            using (var server = new NamedPipeServerStream("DesktopOrganizerPipe", PipeDirection.InOut))
            {
                try
                {
                    server.WaitForConnection();

                    var sw = new StreamWriter(server);

                    sw.WriteLine("set");

                    var json = JsonConvert.SerializeObject(layout);
                    sw.WriteLine(json);
                    sw.Flush();
                    server.WaitForPipeDrain();
                }
                finally
                {
                    server.WaitForPipeDrain();
                    if (server.IsConnected)
                        server.Disconnect();
                }
            }

            proc.WaitForExit();
        }
    }
}
