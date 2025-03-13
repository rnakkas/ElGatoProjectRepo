using Godot;
using ElGatoProject.Utilties;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class PlayerDetectionComponent : Node2D
{
    [Export] private Area2D _playerDetectionArea;
    // [Export] private RayCast2D _playerDetectionRay;
    [Export] public Vector2 PlayerPosition;
    
    private bool _playerInRange, _canSeePlayer;
    private Node2D _player;
    

    public override void _Ready()
    {
	    if (_playerDetectionArea == null)
		    return;
	    _playerDetectionArea.AreaEntered += OnPlayerEnteredDetectionArea;
	    _playerDetectionArea.AreaExited += OnPlayerExitedDetectionArea;
    }

    private void OnPlayerEnteredDetectionArea(Area2D playerArea)
    {
	    if (!playerArea.IsInGroup(Utility.NodeGroupPlayers))
		    return;
	    _playerInRange = true;
	    // if (_playerDetectionRay != null) _playerDetectionRay.Enabled = true;
	    _player = playerArea;
    }

    private void OnPlayerExitedDetectionArea(Area2D playerArea)
    {
	    if (!playerArea.IsInGroup(Utility.NodeGroupPlayers))
		    return;
	    _playerInRange = false;
	    // if (_playerDetectionRay != null) _playerDetectionRay.Enabled = false;
    }

    public bool PlayerDetectionBehaviour()
    {
	    switch (_playerInRange)
	    {
		    case true:
			    // if (_playerDetectionRay != null) _playerDetectionRay.TargetPosition = ToLocal(_player.GlobalPosition);
			    PlayerPosition = _player.GlobalPosition;

			    // // Since target position is being changed this frame, Delay checking of raycasts collisions to the next frame
			    // CallDeferred(nameof(CheckRaycast));
			    break;
		    
		    case false:
			    _canSeePlayer = false;
			    break;
	    }

	    return _canSeePlayer;
    }

    // private void CheckRaycast()
    // {
	   //  _canSeePlayer = !_playerDetectionRay.IsColliding();
    // }
}