﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using StardewValley;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace StackSplitX
{
    public class StackSplitMenu //: NamingMenu
    {
        public delegate void TextSubmittedDelegate(string input);

        /// <summary>The amount being currently held by the player.</summary>
        public int HeldStackAmount { get; private set; }

        /// <summary>The amount in the original stack.</summary>
        public int CurrentStackAmount { get; private set; }

        /// <summary>The dialogue title.</summary>
        public string Title { get; set; } = "Select Amount";

        private InputTextBox InputTextBox;
        private TextSubmittedDelegate OnTextSubmitted;
        private ClickableTextureComponent OKButton;

        public StackSplitMenu(TextSubmittedDelegate textSubmittedCallback, int heldStackAmount, int currentStackAmount)
        {
            this.OnTextSubmitted = textSubmittedCallback;
            this.CurrentStackAmount = currentStackAmount;
            this.HeldStackAmount = heldStackAmount;

            this.InputTextBox = new InputTextBox(0, heldStackAmount.ToString())
            {
                Position = new Vector2(Game1.getMouseX(), Game1.getMouseY() - Game1.tileSize),
                Extent = new Vector2(Game1.tileSize * 3, Game1.tileSize),
                NumbersOnly = true,
                Selected = true
            };
            this.InputTextBox.OnSubmit += (sender) => Submit(sender.Text);
            Game1.keyboardDispatcher.Subscriber = this.InputTextBox;

            // TODO: clean up
            this.OKButton = new ClickableTextureComponent(
                new Rectangle((int)this.InputTextBox.Position.X + (int)this.InputTextBox.Extent.X + Game1.pixelZoom,
                              (int)this.InputTextBox.Position.Y,
                              Game1.tileSize, 
                              Game1.tileSize), 
                Game1.mouseCursors, 
                Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 
                1f, 
                false);
        }

        public void draw(SpriteBatch b)
        {
			//SpriteText.drawStringWithScrollCenteredAt(b, this.Title, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - Game1.tileSize * 2, "", 1f, -1, 0, 0.88f, false);
            this.InputTextBox.Draw(b);
            this.OKButton.draw(b);
            
            // TODO: find a nicer way to do this or encapsulate it
            if (!Game1.options.hardwareCursor)
            {
                b.Draw(Game1.mouseCursors, new Vector2((float)Game1.getMouseX(), (float)Game1.getMouseY()), new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 0, 16, 16)), Color.White * Game1.mouseCursorTransparency, 0f, Vector2.Zero, (float)Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 1f);
            }
        }

        public void ReceiveLeftClick(int x, int y)
        {
            if (this.OKButton.containsPoint(x, y))
            {
                Game1.playSound("smallSelect");
                Submit(this.InputTextBox.Text);
            }
        }

        public bool ContainsPoint(int x, int y)
        {
            return (this.OKButton.containsPoint(x, y) || this.InputTextBox.ContainsPoint(x, y));
        }

        public void Update()
        {
            this.InputTextBox.Update();
        }

        private void Submit(string text)
        {
            Debug.Assert(this.OnTextSubmitted != null);
            this.OnTextSubmitted(text);
        }
    }
}
