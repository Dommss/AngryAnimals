using Godot;

public partial class GameUi : CanvasLayer {
    private SignalManager _signalManager;
    private GameManager _gameManager;
    private ScoreManager _scoreManager;
    
    private Label _levelLabel;
    private Label _attemptsLabel;
    private VBoxContainer _boxContainer;
    private AudioStreamPlayer _sound;

    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");
        _gameManager = GetNode<GameManager>("/root/GameManager");
        _scoreManager = GetNode<ScoreManager>("/root/ScoreManager");
        
        _levelLabel = GetNode<Label>("MC/VB/LevelLabel");
        _attemptsLabel = GetNode<Label>("MC/VB/AttemptsLabel");
        _boxContainer = GetNode<VBoxContainer>("MC/VB2");
        _sound = GetNode<AudioStreamPlayer>("Sound");

        _levelLabel.Text = "Level " + _scoreManager.GetLevelSelected();
        OnAttemptMade();
        _signalManager.AttemptMade += OnAttemptMade;
        _signalManager.GameOver += OnGameOver;
    }

    public override void _Process(double delta) {
        if (_boxContainer.Visible && Input.IsKeyPressed(Key.Space)) {
            _gameManager.LoadMainScene();
        }
    }

    public override void _ExitTree() {
        _signalManager.AttemptMade -= OnAttemptMade;
        _signalManager.GameOver -= OnGameOver;
    }

    private void OnGameOver() {
        _boxContainer.Show();
        _sound.Play();
    }

    private void OnAttemptMade() {
        _attemptsLabel.Text = "Attempts " + _scoreManager.GetAttempts();
    }
}
