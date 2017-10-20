﻿using ClientInterfaces.GOC;
using GorgonLibrary.InputDevices;
using Lidgren.Network;
using SS13_Shared;

namespace ClientInterfaces.UserInterface
{
    public interface IUserInterfaceManager
    {
        IDragDropInfo DragInfo { get; }
        IPlayerAction currentTargetingAction { get; }

        void AddComponent(IGuiComponent component);
        void RemoveComponent(IGuiComponent component);
        void ComponentUpdate(GuiComponentType type, params object[] args);
        void DisposeAllComponents();
        void DisposeAllComponents<T>();
        void ResizeComponents();
        void SetFocus(IGuiComponent newFocus);
        void RemoveFocus();
        void Update(float frameTime);
        void Render();

        void StartTargeting(IPlayerAction action);
        void SelectTarget(object target);
        void CancelTargeting();

        void ToggleMoveMode();

        bool KeyDown(KeyboardInputEventArgs e);
        void MouseWheelMove(MouseInputEventArgs e);
        void MouseMove(MouseInputEventArgs e);
        bool MouseUp(MouseInputEventArgs e);
        bool MouseDown(MouseInputEventArgs e);

        void HandleNetMessage(NetIncomingMessage msg);
    }
}