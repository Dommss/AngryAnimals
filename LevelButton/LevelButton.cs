using Godot;
using System;

public partial class LevelButton : TextureButton {
    [Export] private int _levelNumber = 1;

    private Label _levelLabel;
    private Label _scoreLabel;

    private readonly Vector2 _hoverScale = new(1.1f, 1.1f);
    private readonly Vector2 _defaultScale = new(1f, 1f);

    public override void _Ready() {
        _levelLabel = GetNode<Label>("MC/VB/LevelLabel");
        _scoreLabel = GetNode<Label>("MC/VB/ScoreLabel");
        
        Pressed += OnPressed;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        _levelLabel.Text = _levelNumber.ToString();
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
        
    }
}
