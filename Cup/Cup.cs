using Godot;

public partial class Cup : StaticBody2D {
    private SignalManager _signalManager;
    
    private AnimationPlayer _animationPlayer;
    private AudioStreamPlayer2D _vanishSound;

    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");
        
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _vanishSound = GetNode<AudioStreamPlayer2D>("VanishSound");

        _vanishSound.Finished += OnFinish;
    }

    public override void _ExitTree() {
        _vanishSound.Finished -= OnFinish;
    }

    public void Die() {
        _vanishSound.Play();
        _animationPlayer.Play("Vanish");
    }

    private void OnFinish() {
        _signalManager.EmitSignal(SignalManager.SignalName.CupDestroyed);
        QueueFree();
    }
}
