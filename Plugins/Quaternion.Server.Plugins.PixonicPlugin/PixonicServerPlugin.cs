using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Transport.Connections.Peers;

namespace Quaternion.Server.Plugins.PixonicPlugin
{
    public class PixonicServerPlugin: GameBasePlugin.ServerGameBaseServerPlugin
    {
        private Timer _trigger;
        public static ConcurrentDictionary<string,DateTime> LastRoomAction = new ConcurrentDictionary<string, DateTime>();
        public PixonicServerPlugin(string appId) : base(appId)
        {
            _trigger = new Timer(CleanSilentRoom,null,TimeSpan.FromSeconds(20),TimeSpan.FromMinutes(1));
        }

        private void CleanSilentRoom(object state)
        {
            var silentRooms = LastRoomAction.Where(x => x.Value <= DateTime.UtcNow.AddMinutes(-1)).Select(x=>x.Key);

            if (silentRooms.Any())
            foreach (var usersInScene in UsersInScenes)
            {
                if (silentRooms.Contains(usersInScene.Key))
                {
                    foreach (var usr in usersInScene.Value)
                    {
                        foreach (var usrConnection in usr.Connections)
                        {
                            usrConnection.Value.CloseConnection();
                        }
                    }
                }
            }

            foreach (var silentRoom in silentRooms)
            {
                List<PeerBase> users;
                UsersInScenes.TryRemove(silentRoom, out users);
            }
        }
    }
}
