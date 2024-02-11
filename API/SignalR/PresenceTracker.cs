using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    {
        public static Dictionary<string,List<string>>OnlineUsers=new Dictionary<string,List<string>>();

        public Task<bool> UserConnected(string userName,string connectionId) {
            bool isOnline=false;
            lock (OnlineUsers)
            {
                if (OnlineUsers.ContainsKey(userName))
                {
                    OnlineUsers[userName].Add(connectionId);
                }
                else {
                    OnlineUsers.Add(userName, new List<string>() { connectionId });
                    isOnline = true;
                }
                return Task.FromResult(isOnline);
            }
        }

        public Task<bool> UserDisConnected(string userName, string connectionId) {
            bool isOnline = false;
            lock (OnlineUsers) { 
                if(!OnlineUsers.ContainsKey(userName)) return Task.FromResult(isOnline);

                OnlineUsers[userName].Remove(connectionId);

                if (OnlineUsers[userName].Count == 0) {
                    OnlineUsers.Remove(userName);
                    isOnline = true;
                }
                return Task.FromResult(isOnline);
            }
        }

        public Task<string[]> GetOnlineUsers() {
            string[] onlineUsers;
            lock (OnlineUsers)
            {
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }
            return Task.FromResult(onlineUsers);
        }

        public static Task<List<string>> GetConnectionForUser(string userName) {
            List<string> connectionIds;
            lock (OnlineUsers)
            { 
                connectionIds=OnlineUsers.GetValueOrDefault(userName);
            }
            return Task.FromResult(connectionIds);
        }
    }
}
