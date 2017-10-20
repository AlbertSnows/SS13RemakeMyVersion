﻿using System.Collections.Generic;
using Lidgren.Network;
using SS13.IoC;
using ServerInterfaces.Configuration;
using ServerInterfaces.Network;

namespace ServerServices.Network
{
    public class SS13NetServer : NetServer, ISS13NetServer
    {
        public SS13NetServer()
            : base(LoadNetPeerConfig())
        {
        }

        #region ISS13NetServer Members

        public void SendToAll(NetOutgoingMessage message)
        {
            SendToAll(message, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendMessage(NetOutgoingMessage message, NetConnection client)
        {
            SendMessage(message, client, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendToMany(NetOutgoingMessage message, List<NetConnection> recipients)
        {
            SendMessage(message, recipients, NetDeliveryMethod.ReliableOrdered, 0);
        }

        #endregion

        public static NetPeerConfiguration LoadNetPeerConfig()
        {
            var _config = new NetPeerConfiguration("SS13_NetTag");
            _config.Port = IoCManager.Resolve<IConfigurationManager>().Port;
            return _config;
        }
    }
}