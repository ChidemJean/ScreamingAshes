using Godot;
using System;
using ChidemGames.Core.Characters;
using ChidemGames.Ui;

namespace ChidemGames
{
    public enum StateFocus { GAME, GAME_MENU, INVISIBLE }

    public class GlobalManager : Node
    {
        public Player currentPlayer;

        public Spatial main3dNode = null;

        public StateFocus stateFocus = StateFocus.GAME;

        public Inventory inventory = null;

        public HUD hud = null;

        public string itemsResourcePath = "res://resources/inventory/";

        public bool isCursorInvisible = false;

        public void ChangeStateFocus(StateFocus newStateFocus)
        {
            stateFocus = newStateFocus;

            // if (isCursorInvisible) return;

            switch (stateFocus) {
                case StateFocus.GAME:
                    Input.MouseMode = Input.MouseModeEnum.Captured;
                    break;
                case StateFocus.GAME_MENU:
                    Input.MouseMode = Input.MouseModeEnum.Visible;
                    break;
                case StateFocus.INVISIBLE:
                    Input.MouseMode = Input.MouseModeEnum.Hidden;
                    break;
            }
        }

    }
}