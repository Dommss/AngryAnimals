using Godot;

public partial class Animal : RigidBody2D {
    private SignalManager _signalManager;

    private VisibleOnScreenNotifier2D _notifier;
    private AudioStreamPlayer2D _stretchSound;
    private AudioStreamPlayer2D _launchSound;

    private readonly Vector2 _dragLimMax = new(0, 60);
    private readonly Vector2 _dragLimMin = new(-60, 0);
    private readonly float _impulseMultiplication = 20f;
    
    private bool _isDead;
    private bool _isDragging;
    private bool _isReleased;
    private Vector2 _start = Vector2.Zero;
    private Vector2 _dragStart = Vector2.Zero;
    private Vector2 _draggedVector = Vector2.Zero;
    private Vector2 _lastDraggedPos = Vector2.Zero;
    private float _lastDraggedAmount;
    private float _firedTime;
    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");

        _notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        _stretchSound = GetNode<AudioStreamPlayer2D>("StretchSound");
        _launchSound = GetNode<AudioStreamPlayer2D>("LaunchSound");
        
        _start = GlobalPosition;
        
        _notifier.ScreenExited += OnScreenExited;
        InputEvent += OnInputEvent;
    }

    public override void _PhysicsProcess(double delta) {
        UpdateDebugLabel();

        if (_isReleased) return;
        else {
            if (!_isDragging) return;
            else {
                if (Input.IsActionJustReleased("Drag")) {
                    ReleaseIt();
                }
                else {
                    DragIt();
                }
            }
        }
    }

    public override void _ExitTree() {
        _notifier.ScreenExited -= OnScreenExited;
        InputEvent -= OnInputEvent;
    }

    private void UpdateDebugLabel() {
        var s = "g_pos: " + Utilities.VectorToString(GlobalPosition) + "\n";

        s += "ang: " + AngularVelocity.ToString("F1") + " - linear: " + Utilities.VectorToString(LinearVelocity) + "\n";
        s += "dragging: " + _isDragging + " - released: " + _isReleased + " - firedTime: " + _firedTime + "\n";
        s += "start: " + Utilities.VectorToString(_start) + " - dragStart: " + Utilities.VectorToString(_dragStart) +
            " - draggedVector: " + Utilities.VectorToString(_draggedVector) + "\n";
        s += "lastDraggedPos: " + Utilities.VectorToString(_lastDraggedPos) + " - lastDraggedAmount: " +
             _lastDraggedAmount;
            
        _signalManager.EmitSignal(SignalManager.SignalName.UpdateDebugLabel, s);
    }

    private void GrabIt() {
        _isDragging = true;
        _dragStart = GetGlobalMousePosition();
        _lastDraggedPos = _dragStart;
    }

    private void DragIt() {
        var gmp = GetGlobalMousePosition();
        _lastDraggedAmount = (_lastDraggedPos - gmp).Length();
        _lastDraggedPos = gmp;

        if (_lastDraggedAmount > 0 && !_stretchSound.Playing) {
            _stretchSound.Play();
        }

        _draggedVector = gmp - _dragStart;
        _draggedVector.X = Mathf.Clamp(_draggedVector.X, _dragLimMin.X, _dragLimMax.X);
        _draggedVector.Y = Mathf.Clamp(_draggedVector.Y, _dragLimMin.Y, _dragLimMax.Y);
        
        GlobalPosition = _start + _draggedVector;
    }

    private void ReleaseIt() {
        _isDragging = false;
        _isReleased = true;
        Freeze = false;
        
        ApplyCentralImpulse(GetImpulse());
        _stretchSound.Stop();
        _launchSound.Play();
    }

    private Vector2 GetImpulse() {
        return _draggedVector * -1 * _impulseMultiplication;
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
