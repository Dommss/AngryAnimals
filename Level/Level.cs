using Godot;

public partial class Level : Node2D {
    private GameManager _gameManager;
    private SignalManager _signalManager;
    private ScoreManager _scoreManager;

    private PackedScene _animalScene = GD.Load<PackedScene>("res://Animal/Animal.tscn");
    
    private Label _debugLabel;
    private Marker2D _animalStart;

    public override void _Ready() {
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _signalManager = GetNode<SignalManager>("/root/SignalManager");
        _scoreManager = GetNode<ScoreManager>("/root/ScoreManager");

        _debugLabel = GetNode<Label>("DebugLabel");
        _animalStart = GetNode<Marker2D>("AnimalStart");

        _signalManager.UpdateDebugLabel += OnUpdateDebugLabel;
        _signalManager.AnimalDied += OnAnimalDied;

        Setup();
        OnAnimalDied();
    }

    public override void _Process(double delta) {
        if (Input.IsKeyPressed(Key.Q)) {
            _gameManager.LoadMainScene();
        }
    }

    private void Setup() {
        var tc = GetTree().GetNodesInGroup(_gameManager.GroupCup);
        _scoreManager.SetTargetCups(tc.Count);
    }

    public override void _ExitTree() {
        _signalManager.UpdateDebugLabel -= OnUpdateDebugLabel;
        _signalManager.AnimalDied -= OnAnimalDied;
    }

    private void OnUpdateDebugLabel(string text) {
        _debugLabel.Text = text;
    }

    private void OnAnimalDied() {
        var animalInstance = _animalScene.Instantiate<RigidBody2D>();
        if (animalInstance == null) return;
        
        animalInstance.GlobalPosition = _animalStart.GlobalPosition;
        AddChild(animalInstance);
    }
}