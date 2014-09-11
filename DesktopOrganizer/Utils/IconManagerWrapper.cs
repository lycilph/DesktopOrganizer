using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using Core.Data;
using Newtonsoft.Json;
using NLog;

namespace DesktopOrganizer.Utils
{
    public static class IconManagerWrapper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static IEnumerable<Icon> GetIcons()
        {
            logger.Trace("GetIcons - start");

            Process proc = null;
            try
            {
                proc = Process.Start(new ProcessStartInfo("IconHelper.exe")
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
                if (proc == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }

            logger.Trace("GetIcons - process started");

            string text;
            using (var server = new NamedPipeServerStream("DesktopOrganizerPipe", PipeDirection.InOut))
            {
                logger.Trace("GetIcons - pipe server stream created");

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
            logger.Trace("GetIcons - end");
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
