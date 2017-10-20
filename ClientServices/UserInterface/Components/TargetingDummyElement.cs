﻿using System;
using System.Drawing;
using ClientInterfaces.Resource;
using GorgonLibrary;
using GorgonLibrary.Graphics;
using GorgonLibrary.InputDevices;
using SS13_Shared;
using Image = GorgonLibrary.Graphics.Image;

namespace ClientServices.UserInterface.Components
{
    internal class TargetingDummyElement : GuiComponent
    {
        #region Delegates

        public delegate void TargetingDummyElementPressHandler(TargetingDummyElement sender);

        #endregion

        private readonly IResourceManager _resourceManager;
        public BodyPart BodyPart;
        public float CurrentHealth;
        public float MaxHealth;
        private Point _clickPoint;

        private Sprite _elementSprite;
        private Boolean _selected;

        public TargetingDummyElement(string spriteName, BodyPart part, IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
            BodyPart = part;
            _elementSprite = _resourceManager.GetSprite(spriteName);
            Update(0);
        }

        public event TargetingDummyElementPressHandler Clicked;

        public void Select()
        {
            _selected = true;
        }

        public bool IsSelected()
        {
            return _selected;
        }

        public void ClearSelected()
        {
            _selected = false;
        }

        public override sealed void Update(float frameTime)
        {
            _elementSprite.Position = Position;
            ClientArea = new Rectangle(Position,
                                       new Size((int) _elementSprite.AABB.Width, (int) _elementSprite.AABB.Height));
        }

        public override void Render()
        {
            //elementSprite.Color = selected ? Color.DarkRed : Color.White;
            float healthPct = CurrentHealth/MaxHealth;

            if (healthPct > 0.75) _elementSprite.Color = Color.DarkGreen;
            else if (healthPct > 0.50) _elementSprite.Color = Color.Yellow;
            else if (healthPct > 0.25) _elementSprite.Color = Color.DarkOrange;
            else if (healthPct > 0) _elementSprite.Color = Color.Red;
            else _elementSprite.Color = Color.Black;

            _elementSprite.Position = Position;
            _elementSprite.Draw();
            _elementSprite.Color = Color.White;

            if (!_selected) return;

            Gorgon.CurrentRenderTarget.Circle(Position.X + _clickPoint.X, Position.Y + _clickPoint.Y, 5, Color.Black);
            Gorgon.CurrentRenderTarget.Circle(Position.X + _clickPoint.X, Position.Y + _clickPoint.Y, 4, Color.DarkRed);
            Gorgon.CurrentRenderTarget.Circle(Position.X + _clickPoint.X, Position.Y + _clickPoint.Y, 3, Color.Black);
        }

        public override void Dispose()
        {
            _elementSprite = null;
            Clicked = null;
            base.Dispose();
            GC.SuppressFinalize(this);
        }

        public override bool MouseDown(MouseInputEventArgs e)
        {
            if (!ClientArea.Contains(new Point((int) e.Position.X, (int) e.Position.Y))) return false;

            var spritePosition = new Point((int) e.Position.X - Position.X + (int) _elementSprite.ImageOffset.X,
                                           (int) e.Position.Y - Position.Y + (int) _elementSprite.ImageOffset.Y);

            Image.ImageLockBox imgData = _elementSprite.Image.GetImageData();
            imgData.Lock(false);

            Color pixColour = Color.FromArgb((int) (imgData[spritePosition.X, spritePosition.Y]));
            imgData.Dispose();
            imgData.Unlock();

            if (pixColour.A != 0)
            {
                if (Clicked != null) Clicked(this);
                _clickPoint = new Point((int) e.Position.X - Position.X, (int) e.Position.Y - Position.Y);
                _selected = true;
                return true;
            }

            _selected = false;
            return false;
        }

        public override bool MouseUp(MouseInputEventArgs e)
        {
            return false;
        }
    }
}