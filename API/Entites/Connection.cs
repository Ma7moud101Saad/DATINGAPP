﻿namespace API.Entites
{
    public class Connection
    {
        public Connection()
        {

        }
        public Connection(string connectionId, string userName)
        {
            ConnectionId = connectionId;
            UserName = userName;
        }

        public string ConnectionId { get; set; }

        public string UserName { get; set; }
    }
}
