﻿using System;
using System.Drawing;
using ClientInterfaces.Resource;
using GorgonLibrary.Graphics;
using Lidgren.Network;
using SS13.IoC;
using SS13_Shared;
using SS13_Shared.GO;
using SS13_Shared.GO.Component.Renderable;

namespace CGO
{
    public class ItemSpriteComponent : SpriteComponent
    {
        private bool IsInHand;
        private string basename = "";
        private Hand holdingHand = Hand.None;

        public ItemSpriteComponent()
        {
            SetDrawDepth(DrawDepth.FloorObjects);
        }

        public override ComponentReplyMessage RecieveMessage(object sender, ComponentMessageType type,
                                                             params object[] list)
        {
            ComponentReplyMessage reply = base.RecieveMessage(sender, type, list);

            if (sender == this) //Don't listen to our own messages!
                return ComponentReplyMessage.Empty;

            switch (type)
            {
                case ComponentMessageType.MoveDirection:
                    if (!IsInHand)
                        break;
                    SetDrawDepth(DrawDepth.HeldItems);
                    switch ((Direction) list[0])
                    {
                        case Direction.North:
                            if (SpriteExists(basename + "_inhand_back"))
                                SetSpriteByKey(basename + "_inhand_back");
                            else
                                SetSpriteByKey(basename + "_inhand");
                            if (holdingHand == Hand.Left)
                                flip = false;
                            else
                                flip = true;
                            break;
                        case Direction.South:
                            SetSpriteByKey(basename + "_inhand");
                            if (holdingHand == Hand.Left)
                                flip = true;
                            else
                                flip = false;
                            break;
                        case Direction.East:
                            if (holdingHand == Hand.Left)
                                SetDrawDepth(DrawDepth.FloorObjects);
                            else
                                SetDrawDepth(DrawDepth.HeldItems);
                            SetSpriteByKey(basename + "_inhand_side");
                            flip = true;
                            break;
                        case Direction.West:
                            if (holdingHand == Hand.Right)
                                SetDrawDepth(DrawDepth.FloorObjects);
                            else
                                SetDrawDepth(DrawDepth.HeldItems);
                            SetSpriteByKey(basename + "_inhand_side");
                            flip = false;
                            break;
                        case Direction.NorthEast:
                            if (SpriteExists(basename + "_inhand_back"))
                                SetSpriteByKey(basename + "_inhand_back");
                            else
                                SetSpriteByKey(basename + "_inhand");
                            if (holdingHand == Hand.Left)
                                flip = false;
                            else
                                flip = true;
                            break;
                        case Direction.NorthWest:
                            if (SpriteExists(basename + "_inhand_back"))
                                SetSpriteByKey(basename + "_inhand_back");
                            else
                                SetSpriteByKey(basename + "_inhand");
                            if (holdingHand == Hand.Left)
                                flip = false;
                            else
                                flip = true;
                            break;
                        case Direction.SouthEast:
                            SetSpriteByKey(basename + "_inhand");
                            if (holdingHand == Hand.Right)
                                flip = false;
                            else
                                flip = true;
                            break;
                        case Direction.SouthWest:
                            SetSpriteByKey(basename + "_inhand");
                            if (holdingHand == Hand.Right)
                                flip = false;
                            else
                                flip = true;
                            break;
                    }
                    break;
                case ComponentMessageType.Dropped:
                    SetSpriteByKey(basename);
                    IsInHand = false;
                    SetDrawDepth(DrawDepth.FloorObjects);
                    holdingHand = Hand.None;
                    break;
                case ComponentMessageType.PickedUp:
                    IsInHand = true;
                    holdingHand = (Hand) list[0];
                    break;
                case ComponentMessageType.SetBaseName:
                    basename = (string) list[0];
                    break;
            }

            return reply;
        }

        public override void HandleNetworkMessage(IncomingEntityComponentMessage message, NetConnection sender)
        {
            base.HandleNetworkMessage(message, sender);

            switch ((ComponentMessageType) message.MessageParameters[0])
            {
                case ComponentMessageType.SetBaseName:
                    //basename = (string) message.MessageParameters[1];
                    break;
            }
        }

        /// <summary>
        /// Set parameters :)
        /// </summary>
        /// <param name="parameter"></param>
        public override void SetParameter(ComponentParameter parameter)
        {
            //base.SetParameter(parameter);
            switch (parameter.MemberName)
            {
                case "drawdepth":
                    SetDrawDepth((DrawDepth) Enum.Parse(typeof (DrawDepth), parameter.GetValue<string>(), true));
                    break;
                case "basename":
                    basename = parameter.GetValue<string>();
                    LoadSprites();
                    break;
                case "addsprite":
                    var spriteToAdd = parameter.GetValue<string>();
                    LoadSprites(spriteToAdd);
                    break;
            }
        }

        protected override Sprite GetBaseSprite()
        {
            return sprites[basename];
        }

        /// <summary>
        /// Load the mob sprites given the base name of the sprites.
        /// </summary>
        public void LoadSprites()
        {
            LoadSprites(basename);
            SetSpriteByKey(basename);
        }

        public void LoadSprites(string name)
        {
            if (!HasSprite(name))
            {
                AddSprite(name);
                AddSprite(name + "_inhand");
                AddSprite(name + "_inhand_side");
                if (IoCManager.Resolve<IResourceManager>().SpriteExists(name + "_inhand_back"))
                    AddSprite(name + "_inhand_back");
            }
        }

        protected override bool WasClicked(PointF worldPos)
        {
            return base.WasClicked(worldPos) && !IsInHand;
        }

        public override void HandleComponentState(dynamic state)
        {
            base.HandleComponentState((SpriteComponentState) state);

            if (state.BaseName != null && basename != state.BaseName)
            {
                basename = state.BaseName;
                LoadSprites();
            }
        }
    }
}