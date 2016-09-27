using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PluginBase;
using SocketServer;
using Transport.Server;

namespace Launcher
{
    public class Launcher
    {
        public readonly List<IServerTransport> TransportList = new List<IServerTransport>();
        protected bool IsStarted = false;
        public Launcher()
        {
            var tcp = new TcpSocketServer();
            TransportList.Add(tcp);
        }

        public void LoadPlugin(string pathToPlugin)
        {
            if (!IsStarted)
            {
                var assembly = Assembly.LoadFrom(pathToPlugin);
                var pluginType = assembly.GetTypes().FirstOrDefault(x => x.IsAssignableFrom(typeof(ServerPluginBase)));
                if (pluginType != null)
                {
                    var plugin = (ServerPluginBase)Activator.CreateInstance(pluginType);

                    if (plugin != null)
                        LoadPlugin(plugin);
                }
            }
        }

        public void LoadPlugin(ServerPluginBase plugin)
        {
            if (!IsStarted)
            {
                plugin.InitTransports(TransportList);
                IsStarted = true;
            }
        }
        
    }
}
