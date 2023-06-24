using Godot;
using System;

namespace ChidemGames.Events 
{
    public class GameEvent { 
        public const string RenderSizeChanged = "RenderSizeChanged";
        public const string ChangeRenderSize = "ChangeRenderSize";
        public const string TakeItem = "TakeItem";
        public const string OpenMenu = "OpenMenu";
        public const string CloseMenu = "CloseMenu";
        public const string OnCloseMenu = "OnCloseMenu";
        public const string OnSlotMouseEnter = "OnSlotMouseEnter";
        public const string OnSlotMouseLeave = "OnSlotMouseLeave";
        public const string DetachItemFromSlot = "DetachItemFromSlot";
        public const string OnFastSlotAttach = "OnFastSlotAttach";
        public const string OnFastSlotDetach = "OnFastSlotDetach";
    }
}