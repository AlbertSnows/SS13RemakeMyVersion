﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SS13_Shared.GO;
using SS13_Shared.GO.Component.Renderable;

namespace SGO
{
    public class WearableAnimatedSpriteComponent : AnimatedSpriteComponent
    {
        public bool IsCurrentlyWorn = false;
        public bool IsCurrentlyCarried = false;
        
        public override ComponentReplyMessage RecieveMessage(object sender, ComponentMessageType type,
                                                      params object[] list)
        {
            ComponentReplyMessage reply = base.RecieveMessage(sender, type, list);

            if (sender == this) //Don't listen to our own messages!
                return ComponentReplyMessage.Empty;

            switch (type)
            {
                case ComponentMessageType.ItemEquipped:
                    IsCurrentlyWorn = true;
                    IsCurrentlyCarried = false;
                    break;
                case ComponentMessageType.ItemUnEquipped:
                    IsCurrentlyWorn = false;
                    break;
                case ComponentMessageType.Dropped:
                    IsCurrentlyCarried = false;
                    break;
                case ComponentMessageType.PickedUp:
                    IsCurrentlyCarried = true;
                    break;
            }

            return reply;
        }

        public override ComponentState GetComponentState()
        {
            var masterUid = master != null ? (int?)master.Owner.Uid : null;
            return new WearableAnimatedSpriteComponentState(IsCurrentlyWorn, IsCurrentlyCarried, Visible, DrawDepth, Name, CurrentAnimation, Loop, masterUid);
        }
    }
}
