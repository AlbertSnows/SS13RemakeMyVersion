﻿using GameObject;
using SS13_Shared;
using SS13_Shared.GO;
using SS13_Shared.GO.Component.Mover;

namespace SGO
{
    internal class BasicMoverComponent : Component
    {
        public BasicMoverComponent()
        {
            Family = ComponentFamily.Mover;
        }

        public override ComponentReplyMessage RecieveMessage(object sender, ComponentMessageType type,
                                                             params object[] list)
        {
            ComponentReplyMessage reply = base.RecieveMessage(sender, type, list);

            if (sender == this)
                return ComponentReplyMessage.Empty;

            switch (type)
            {
                case ComponentMessageType.PhysicsMove:
                    Translate((float) list[0], (float) list[1]);
                    break;
            }
            return reply;
        }

        public void Translate(float x, float y)
        {
            Owner.GetComponent<TransformComponent>(ComponentFamily.Transform).Position = new Vector2(x, y);
        }

    }
}