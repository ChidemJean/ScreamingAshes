using Godot;
using System;
using System.Collections.Generic;
using ChidemGames.Events;

namespace ChidemGames.Debug
{
    public partial class CLI : VBoxContainer
    {

        [Export] public NodePath cmdLabelPath;
        [Export] public NodePath inputPath;
        [Export] public NodePath closePath;
        [Export] public NodePath labelsCtnPath;
        [Export] public PackedScene labelCmd;

        public Label cmdLabel;
        public LineEdit input;
        public Control close;
        private bool isOpen = false;
        private Tween tween;
        private string initialCmdText;
        private List<string> history = new List<string>();
        private int selectedHistoryKey = 0;
        private VBoxContainer labelsCtn;
        private GlobalEvents globalEvents;
        GlobalManager globalManager;

        public enum LineType {
            Error,
            Success,
            Default
        }

        public Dictionary<string, string> cmds = new Dictionary<string, string> 
        { 
            {"cs", "cs [scene_key] - Muda a cena atual"},  
            {"clear", "clear - Limpa cmd"},
            {"help", "help - Lista os comandos disponíveis"},
            {"clr_hist", "clr_hist - Limpa o histórico de comandos"},
            {"list_hist", "list_hist - Lista o histórico de comandos"},
            {"crs", "crs [value: 1 - 10] - Altera o render size"},
            {"inv", "inv - Solicita operação no inventário"},
            {"sp_enemy", "sp_enemy [EnemyId] - Spawna um novo inimigo"},
            {"get_item", "get_item [ItemId] - Adiciona um item no inventário"},
            {"kill_all", "kill_all - Mata todos os inimigos"},
            {"resurrect", "resurrect - Ressuscita o Player"},
        };
        
        public override void _Ready()
        {
            globalEvents = GetNode<GlobalEvents>("/root/GlobalEvents");
            globalManager = GetNode<GlobalManager>("/root/GlobalManager");
            cmdLabel = GetNode<Label>(cmdLabelPath);
            input = GetNode<LineEdit>(inputPath);
            close = GetNode<Control>(closePath);
            labelsCtn = GetNode<VBoxContainer>(labelsCtnPath);
            initialCmdText = cmdLabel.Text;
            // input.Connect("text_entered", this, nameof(OnTextEntered));
            // input.Connect("gui_input", this, nameof(OnGuiInput));
            // close.Connect("gui_input", this, nameof(OnGuiInputClose));
            input.TextSubmitted += OnTextEntered;
            input.GuiInput += OnGuiInput;
            close.GuiInput += OnGuiInputClose;
        }

        public override void _Input(InputEvent @event)
        {
            if (isOpen) {
                if (@event is InputEventKey) {
                    InputEventKey _event = (InputEventKey) @event;
                    if (_event.IsPressed()) return; 
                    bool KeyArrowUp = _event.PhysicalKeycode == Key.Up;
                    bool KeyArrowDown = _event.PhysicalKeycode == Key.Down;
                    if (KeyArrowUp || KeyArrowDown) {
                        if (history.Count > 0) {
                            input.GrabFocus();
                            string cmdHistory = history[selectedHistoryKey];
                            input.Text = cmdHistory;
                            int newIdx = KeyArrowUp ? selectedHistoryKey + 1 : selectedHistoryKey - 1;
                            selectedHistoryKey = Mathf.Clamp(newIdx, 0, history.Count - 1);
                            input.Select(input.Text.Length);
                        }
                    }
                }
            }
            if (@event is InputEventKey) {
                InputEventKey _input = (InputEventKey) @event;
                if (!_input.IsPressed()) {
                    if (_input.PhysicalKeycode == Key.Quoteleft) {
                        if (isOpen) {
                            Close();
                            return;
                        }
                        Open();
                    }
                    if (_input.PhysicalKeycode == Key.Escape) {
                        Close();
                    }
                }
            }
        }

        public void SpawnNewLine(string message, LineType lineType = LineType.Default)
        {
            var label = labelCmd.Instantiate<Label>();
            labelsCtn.AddChild(label);
            Color colorLabel =  new Color(1,1,1);
            switch (lineType) {
                case LineType.Default:
                    break;
                case LineType.Error:
                    colorLabel = new Color(1,0,0);
                    break;
                case LineType.Success:
                    colorLabel = new Color(0,1,0);
                    break;
            }
            label.Set("theme_override_colors/font_color", colorLabel);
            label.Text = message;
        }

        public void OnTextEntered(string newText)
        {
            if (!isOpen) return;
            foreach (KeyValuePair<string, string> keyValueCmd in cmds) {
                if (newText.IndexOf(keyValueCmd.Key) != -1) {
                    ExecuteCmd(keyValueCmd.Key, newText);
                    return;
                }
            }
            SpawnNewLine("Comando não encontrado");
            input.Text = "";
        }

        public void ExecuteCmd(string cmdKey, string cmd)
        {
            AddCmdToHistory(cmd);
            selectedHistoryKey = 0;
            if (cmdKey == "clear") {
                foreach (var node in labelsCtn.GetChildren())
                {
                    node.QueueFree();
                }
                SpawnNewLine(initialCmdText);
            } else {
                string[] cmdParams = cmd.Substr(cmd.IndexOf(cmdKey) + cmdKey.Length, cmd.Length-1).Trim().Split(" ");
                switch (cmdKey) {
                    case "cs":
                        SpawnNewLine("'" + cmd + "' executado.");
                        Close();
                        break;
                    
                    case "help":
                        string helpStr = "";
                        foreach (KeyValuePair<string, string> keyValueCmd in cmds) {
                            helpStr += "\n" + keyValueCmd.Value;
                        }
                        SpawnNewLine(helpStr);
                        break;

                    case "clr_hist":
                        history.Clear();
                        break;

                    case "list_hist":
                        string histStr = "\nHistórico de comandos: ";
                        if (history.Count == 0) {
                            histStr += "\n vazio";
                        }
                        foreach (string histItem in history) {
                            histStr += "\n ------ ["+(history.IndexOf(histItem)+1)+"]: " + histItem;
                        }
                        SpawnNewLine(histStr);
                        break;

                    case "crs":
                        float size = cmdParams[0].ToFloat() / 10f;
                        globalEvents.EmitSignal(GameEvent.ChangeRenderSize, size);
                        Close();
                        break;

                    case "sp_enemy":
                        string resPath = cmdParams[0];
                        int qtdE = cmdParams.Length == 2 ? cmdParams[1].ToInt() : 1;
                        var enemyScene = ResourceLoader.Load<PackedScene>($"res://scenes/enemies/{resPath}.tscn");
                        var rng = new RandomNumberGenerator();
                        rng.Randomize();
                        
                        for (int i = 0; i < qtdE; i++) {
                            var enemyNode = enemyScene.Instantiate<Node3D>();
                            globalManager.main3dNode.AddChild(enemyNode);
                            var _waypoints = GetTree().GetNodesInGroup("DefaultEnemyWaypoints");
                            Vector3 pos = ((Marker3D) _waypoints[rng.RandiRange(0, _waypoints.Count - 1)]).GlobalPosition;
                            enemyNode.GlobalPosition = pos;
                            SpawnNewLine($"     Inimigo spawnado em {pos.ToString()}", LineType.Success);
                        }
                        break;

                    case "get_item":
                        string itemId = cmdParams[0];
                        int qtd = cmdParams.Length == 2 ? cmdParams[1].ToInt() : 1;
                        for (int i = 0; i < qtd; i++) {
                            globalEvents.EmitSignal(GameEvent.TakeItem, itemId, false, -1);
                        }
                        SpawnNewLine($"     {qtd}x {itemId} adicionado(s) no inventário", LineType.Success);
                        break;

                    case "kill_all":
                        foreach (var node in globalManager.main3dNode.GetChildren()) {
                            if (node.Name.ToString().ToLower().Contains("enemy") && node is CharacterBody3D) {
                                node.QueueFree();
                            }
                        }
                        break;

                    case "resurrect":
                        var player = globalManager.currentPlayer;
                        bool ressurect = player.Ressurect();
                        if (ressurect) {
                            SpawnNewLine($"     Player ressuscitado", LineType.Success);
                        } else {
                            SpawnNewLine($"     Não foi possível ressuscitar, player vivo", LineType.Error);
                        }
                        break;
                }
            }
            input.Text = "";
        }

        public void AddCmdToHistory(string cmd)
        {
            if (cmd != "list_histr" && !history.Contains(cmd)) {
                history.Add(cmd);
            }
        }

        public void OnGuiInput(InputEvent @event)
        {
            
        }

        public void Print(string msg, bool open = false)
        {
            cmdLabel.Text += "\n" + msg;
            if (open) {
                Open();
            }    
        }
        
        public void OnGuiInputClose(InputEvent @event)
        {
            if (@event is InputEventMouseButton) {
                InputEventMouseButton _event = (InputEventMouseButton) @event;
                if (!_event.IsPressed() && _event.ButtonIndex == MouseButton.Left) {
                    Close();
                }
            }
        }

        public void Open()
        {
            globalManager.ChangeStateFocus(StateFocus.GAME_MENU);
            this.isOpen = true;
            Visible = true;
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.TweenProperty(this, "position:y", 0f, .4f);
            input.GrabFocus();
        }

        public async void Close()
        {
            globalManager.ChangeStateFocus(StateFocus.GAME);
            this.isOpen = false;
            if (tween != null) {
                tween.Kill();
            }
            tween = CreateTween();
            tween.TweenProperty(this, "position:y", -Size.Y, .25f);
            await ToSignal(tween, "finished");
            Visible = false;
            selectedHistoryKey = 0;
            input.Text = "";
        }

    }
}