using Godot;

public partial class Animal : RigidBody2D {
    private SignalManager _signalManager;
    private GameManager _gameManager;
    private ScoreManager _scoreManager;

    private VisibleOnScreenNotifier2D _notifier;
    private AudioStreamPlayer2D _stretchSound;
    private AudioStreamPlayer2D _launchSound;
    private AudioStreamPlayer2D _collSound;
    private Sprite2D _arrowSprite;

    private readonly Vector2 _dragLimMax = new(0, 60);
    private readonly Vector2 _dragLimMin = new(-60, 0);
    private readonly float _impulseMultiplication = 20f;
    private readonly float _impulseMax = 1200f;
    private readonly float _fireDelay = 0.25f;
    private readonly float _hasStopped = 0.1f;
    
    private bool _isDead;
    private bool _isDragging;
    private bool _isReleased;
    private Vector2 _start = Vector2.Zero;
    private Vector2 _dragStart = Vector2.Zero;
    private Vector2 _draggedVector = Vector2.Zero;
    private Vector2 _lastDraggedPos = Vector2.Zero;
    private float _lastDraggedAmount;
    private float _firedTime;
    private float _arrowScaleX;
    private int _lastCollCount;
    
    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _scoreManager = GetNode<ScoreManager>("/root/ScoreManager");

        _notifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        _stretchSound = GetNode<AudioStreamPlayer2D>("StretchSound");
        _launchSound = GetNode<AudioStreamPlayer2D>("LaunchSound");
        _collSound = GetNode<AudioStreamPlayer2D>("CollSound");
        _arrowSprite = GetNode<Sprite2D>("ArrowSprite");
        
        _start = GlobalPosition;
        _arrowScaleX = _arrowSprite.Scale.X;
        _arrowSprite.Hide();
        
        _notifier.ScreenExited += OnScreenExited;
        InputEvent += OnInputEvent;
    }

    public override void _PhysicsProcess(double delta) {
        UpdateDebugLabel();

        if (_isReleased) {
            _firedTime += (float)delta;
            if (_firedTime > _fireDelay) {
                PlayColl();
                CheckOnTarget();
            }
        } else {
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
        var s = "g_pos: " + Utilities.VectorToString(GlobalPosition) + " - contacts: " + GetContactCount() + "\n";

        s += "ang: " + AngularVelocity.ToString("F1") + " - linear: " + Utilities.VectorToString(LinearVelocity) + "\n";
        s += "dragging: " + _isDragging + " - released: " + _isReleased + " - firedTime: " + _firedTime + "\n";
        s += "start: " + Utilities.VectorToString(_start) + " - dragStart: " + Utilities.VectorToString(_dragStart) +
            " - draggedVector: " + Utilities.VectorToString(_draggedVector) + "\n";
        s += "lastDraggedPos: " + Utilities.VectorToString(_lastDraggedPos) + " - lastDraggedAmount: " +
             _lastDraggedAmount;
            
        _signalManager.EmitSignal(SignalManager.SignalName.UpdateDebugLabel, s);
    }

    private void ScaleArrow() {
        var impLength = GetImpulse().Length();
        var perc = impLength / _impulseMax;

        var spriteScale = _arrowSprite.Scale;
        spriteScale.X = (_arrowScaleX * perc) + _arrowScaleX;
        _arrowSprite.Scale = spriteScale;
        _arrowSprite.Rotation = (_start - GlobalPosition).Angle();
    }

    private bool StoppedRolling() {
        if (GetContactCount() > 0) {
            if (Mathf.Abs(LinearVelocity.Y) < _hasStopped && Mathf.Abs(AngularVelocity) < _hasStopped) {
                return true;
            }
        }

        return false;
    }

    private void CheckOnTarget() {
        if (!StoppedRolling()) return;

        var cb = GetCollidingBodies();
        if (cb.Count == 0) return;

        if (cb[0].IsInGroup(_gameManager.GroupCup)) {
            cb[0].Call("Die");
            Die();
        }
    }

    private void PlayColl() {
        if (_lastCollCount == 0 && GetContactCount() > 0 && !_collSound.Playing) {
            _collSound.Play();
        }

        _lastCollCount = GetContactCount();
    }

    private void GrabIt() {
        _isDragging = true;
        _dragStart = GetGlobalMousePosition();
        _lastDraggedPos = _dragStart;
        _arrowSprite.Show();
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
        ScaleArrow();
    }

    private void ReleaseIt() {
        _isDragging = false;
        _isReleased = true;
        Freeze = false;
        
        ApplyCentralImpulse(GetImpulse());
        _stretchSound.Stop();
        _launchSound.Play();
        _scoreManager.AttemptMade();
        _arrowSprite.Hide();
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
