﻿using System;
using System.Drawing;
using CGO;
using ClientInterfaces.Resource;
using ClientServices.Tiles;
using GameObject;
using GorgonLibrary;
using GorgonLibrary.Graphics;
using SS13.IoC;
using SS13_Shared.GO;
using Image = GorgonLibrary.Graphics.Image;

namespace ClientServices.Helpers
{
    internal static class Utilities
    {
        public static string GetObjectSpriteName(Type type)
        {
            return type.IsSubclassOf(typeof (Tile)) ? "tilebuildoverlay" : "nosprite";
        }

        public static Sprite GetSpriteComponentSprite(Entity entity)
        {
            ComponentReplyMessage reply = entity.SendMessage(entity, ComponentFamily.Renderable,
                                                             ComponentMessageType.GetSprite);
            if (reply.MessageType == ComponentMessageType.CurrentSprite)
            {
                var sprite = (Sprite) reply.ParamsList[0];
                return sprite;
            }
            return null;
        }

        public static Sprite GetIconSprite(Entity entity)
        {
            if(entity.HasComponent(ComponentFamily.Icon))
            {
                var icon = entity.GetComponent<IconComponent>(ComponentFamily.Icon).Icon;
                if (icon == null)
                    return IoCManager.Resolve<IResourceManager>().GetNoSprite();
                return icon;
            }
            return IoCManager.Resolve<IResourceManager>().GetNoSprite();
        }

        public static bool SpritePixelHit(Sprite toCheck, Vector2D clickPos)
        {
            var clickPoint = new PointF(clickPos.X, clickPos.Y);
            if (!toCheck.AABB.Contains(clickPoint)) return false;

            var spritePosition = new Point((int) clickPos.X - (int) toCheck.Position.X + (int) toCheck.ImageOffset.X,
                                           (int) clickPos.Y - (int) toCheck.Position.Y + (int) toCheck.ImageOffset.Y);

            Image.ImageLockBox imgData = toCheck.Image.GetImageData();

            imgData.Lock(false);
            Color pixColour = Color.FromArgb((int) (imgData[spritePosition.X, spritePosition.Y]));
            imgData.Dispose();
            imgData.Unlock();

            return pixColour.A != 0;
        }
    }

    public class ColorInterpolator
    {
        private static readonly ComponentSelector RedSelector = color => color.R;
        private static readonly ComponentSelector GreenSelector = color => color.G;
        private static readonly ComponentSelector BlueSelector = color => color.B;

        public static Color InterpolateBetween(
            Color endPoint1,
            Color endPoint2,
            double lambda)
        {
            if (lambda < 0 || lambda > 1)
            {
                throw new ArgumentOutOfRangeException("lambda");
            }
            Color color = Color.FromArgb(
                InterpolateComponent(endPoint1, endPoint2, lambda, RedSelector),
                InterpolateComponent(endPoint1, endPoint2, lambda, GreenSelector),
                InterpolateComponent(endPoint1, endPoint2, lambda, BlueSelector)
                );

            return color;
        }

        private static byte InterpolateComponent(
            Color endPoint1,
            Color endPoint2,
            double lambda,
            ComponentSelector selector)
        {
            return (byte) (selector(endPoint1)
                           + (selector(endPoint2) - selector(endPoint1))*lambda);
        }

        #region Nested type: ComponentSelector

        private delegate byte ComponentSelector(Color color);

        #endregion
    }
}