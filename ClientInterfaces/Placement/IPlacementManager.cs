﻿using System;
using ClientInterfaces.Map;
using GameObject;
using GorgonLibrary;
using Lidgren.Network;
using SS13_Shared;

namespace ClientInterfaces.Placement
{
    public interface IPlacementManager
    {
        Boolean IsActive { get; }
        Boolean Eraser { get; }

        event EventHandler PlacementCanceled;

        void HandleDeletion(Entity entity);
        void HandlePlacement();
        void BeginPlacing(PlacementInformation info);
        void Render();
        void Clear();
        void ToggleEraser();
        void Rotate();

        void Update(Vector2D mouseScreen, IMapManager currentMap);
        void HandleNetMessage(NetIncomingMessage msg);
    }
}