using Godot;

public partial class Water : Area2D {
    private AudioStreamPlayer2D _splashSound;
    
    public override void _Ready() {
        _splashSound = GetNode<AudioStreamPlayer2D>("SplashSound");
        BodyEntered += OnBodyEntered;
    }

    public override void _ExitTree() {
        BodyEntered -= OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body) {
        if (body.IsInGroup("Animal")) {
            _splashSound.Play();
        }
    }
}
