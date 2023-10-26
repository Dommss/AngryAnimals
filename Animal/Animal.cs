using Godot;

public partial class Animal : RigidBody2D {
    private SignalManager _signalManager;

    private VisibleOnScreenNotifier2D _notifier;

    private bool _isDead = false;
    private bool _isDragging = false;
    private bool _isReleased = false;
    private Vector2 _start = Vector2.Zero;
    private Vector2 _dragStart = Vector2.Zero;
    private Vector2 _draggedVector = Vector2.Zero;
    private Vector2 _lastDraggedPos = Vector2.Zero;
    private float _lastDraggedAmount = 0f;
    private float _firedTime = 0f;
    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");

        _notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");

        _start = GlobalPosition;
        
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

        s += "ang: " + AngularVelocity.ToString("F1") + " - linear: " + Utilities.VectorToString(LinearVelocity) + "\n";
        s += "dragging: " + _isDragging + " - released: " + _isReleased + " - firedTime: " + _firedTime + "\n";
        s += "start: " + Utilities.VectorToString(_start) + " - dragStart: " + Utilities.VectorToString(_dragStart) + "\n";
        s += "lastDraggedPos: " + Utilities.VectorToString(_lastDraggedPos) + " - lastDraggedAmount: " +
             _lastDraggedAmount;
            
        _signalManager.EmitSignal(SignalManager.SignalName.UpdateDebugLabel, s);
    }

    private void GrabIt() {
        _isDragging = true;
        _dragStart = GetGlobalMousePosition();
        _lastDraggedPos = _dragStart;
    }

    private void OnInputEvent(Node viewport, InputEvent @event, long shapeIdx) {
        if (_isDragging || _isReleased) return;
        
        if (@event.IsActionPressed("Drag")) {
            GrabIt();
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
