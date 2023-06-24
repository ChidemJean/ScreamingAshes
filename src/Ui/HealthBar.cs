using Godot;
using System;

namespace ChidemGames.Ui
{
    public class HealthBar : Panel
    {
        [Export]
        NodePath progressPath;
        ProgressBar progress;

        [Export]
        NodePath iconPath;
        TextureRect icon;

        GlobalManager globalManager;

        public override void _Ready()
        {
            globalManager = GetNode<GlobalManager>("/root/GlobalManager");
            progress = GetNode<ProgressBar>(progressPath);
            icon = GetNode<TextureRect>(iconPath);
        }

        public override void _Process(float delta)
        {
            if (globalManager.currentPlayer != null) {
                progress.Value = Mathf.Lerp((float) progress.Value, globalManager.currentPlayer.health / globalManager.currentPlayer.maxHealth, .15f);
            }
        }

    }
}