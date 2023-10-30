using Godot;
using System;

public partial class LevelButton : TextureButton {
    [Export] private int _levelNumber = 1;

    private ScoreManager _scoreManager;

    private Label _levelLabel;
    private Label _scoreLabel;
    private PackedScene _levelScene;

    private readonly Vector2 _hoverScale = new(1.1f, 1.1f);
    private readonly Vector2 _defaultScale = new(1f, 1f);

    public override void _Ready() {
        _scoreManager = GetNode<ScoreManager>("/root/ScoreManager");
        
        _levelLabel = GetNode<Label>("MC/VB/LevelLabel");
        _scoreLabel = GetNode<Label>("MC/VB/ScoreLabel");
        
        Pressed += OnPressed;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        _levelLabel.Text = _levelNumber.ToString();
        _scoreLabel.Text = _scoreManager.GetBestForLevel(_levelNumber).ToString();
        _levelScene = GD.Load<PackedScene>("res://Level/Level" + _levelNumber + ".tscn");
    }

    public override void _ExitTree() {
        Pressed -= OnPressed;
    }

    private void OnMouseExited() {
        Scale = _defaultScale;
    }

    private void OnMouseEntered() {
        Scale = _hoverScale;
    }

    private void OnPressed() {
        _scoreManager.LevelSelected(_levelNumber);
        GetTree().ChangeSceneToPacked(_levelScene);
    }
}
