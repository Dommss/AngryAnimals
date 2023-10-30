using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class ScoreManager : Node {
    private SignalManager _signalManager;
    
    private readonly int _defaultScore = 1000;

    private Dictionary _levelScores = new();
    private int _levelSelected;
    private int _attempts;
    private int _cupsHit;
    private int _targetCups;

    public override void _Ready() {
        _signalManager = GetNode<SignalManager>("/root/SignalManager");
        
        _signalManager.CupDestroyed += OnCupDestroyed;
    }

    public override void _ExitTree() {
        _signalManager.CupDestroyed -= OnCupDestroyed;
    }

    private void CheckAndAdd(int level) {
        if (!_levelScores.ContainsKey(level)) {
            _levelScores[level] = _defaultScore;
        }
    }

    public void LevelSelected(int level) {
        CheckAndAdd(level);
        _levelSelected = level;
        _attempts = 0;
        _cupsHit = 0;
        GD.Print("LevelSelected: " + _levelSelected + " - LevelScores: " + _levelScores);
    }

    public int GetBestForLevel(int level) {
        CheckAndAdd(level);
        return (int)_levelScores[level];
    }

    public int GetAttempts() {
        return _attempts;
    }

    public int GetLevelSelected() {
        return _levelSelected;
    }

    public void SetTargetCups(int t) {
        _targetCups = t;
        GD.Print("SetTargetCups: " + _targetCups);
    }

    private void OnCupDestroyed() {
        _cupsHit++;
    }
}
