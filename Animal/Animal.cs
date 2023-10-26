using Godot;

public partial class Animal : RigidBody2D {
    private SignalManager _signalManager;

    private VisibleOnScreenNotifier2D _notifier;

    private bool _isDead = false;
    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");

        _notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");

        _notifier.ScreenExited += OnScreenExited;
        InputEvent += OnInputEvent;
    }

    public override void _PhysicsProcess(double delta) {
        UpdateDebugLabel();
    }

    public override void _ExitTree() {
        _notifier.ScreenExited -= OnScreenExited;
        InputEvent -= OnInputEvent;
    }

    private void UpdateDebugLabel() {
        var s = "g_pos: " + Utilities.VectorToString(GlobalPosition) + "\n";

        s += "ang: " + AngularVelocity.ToString("F1") + "\n";
        s += "linear: " + Utilities.VectorToString(LinearVelocity);

        _signalManager.EmitSignal(SignalManager.SignalName.UpdateDebugLabel, s);
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx) {
        if (@event.IsActionPressed("Drag")) {
         GD.Print(@event); 
        }
    }

    private void Die() {
        if (_isDead) return;

        _isDead = true;
        _signalManager.EmitSignal(SignalManager.SignalName.AnimalDied);
        QueueFree();
    }

    private void OnScreenExited() {
        Die();
    }
}
