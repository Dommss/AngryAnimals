using Godot;

public partial class Water : Area2D {
    private GameManager _gameManager;
    
    private AudioStreamPlayer2D _splashSound;
    
    public override void _Ready() {
        _gameManager = GetNode<GameManager>("/root/GameManager");
        
        _splashSound = GetNode<AudioStreamPlayer2D>("SplashSound");
        BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree() {
        BodyEntered -= OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body) {
        if (body.IsInGroup(_gameManager.GroupAnimal)) {
            _splashSound.Play();
        }
    }
}
