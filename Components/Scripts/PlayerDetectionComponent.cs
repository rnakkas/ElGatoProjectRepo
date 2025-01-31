using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

/*
 * IF player is in detection area
	- playerInRange = true
	- activate the player detection ray
	- set the player detection ray to point towards player's position
	- IF player detection ray is not colliding with wall
		- canSeePlayer = true

	Pass the canSeePlayer values back to parent
 */

[GlobalClass]
public partial class PlayerDetectionComponent : Node2D
{
    [Export] private Area2D _playerDetectionArea;
    [Export] private RayCast2D _playerDetectionRay;
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
	    if (!playerArea.IsInGroup("Players"))
		    return;
	    _playerInRange = true;
	    _playerDetectionRay.Enabled = true;
	    _player = playerArea;
    }

    private void OnPlayerExitedDetectionArea(Area2D playerArea)
    {
	    if (!playerArea.IsInGroup("Players"))
		    return;
	    _playerInRange = false;
	    _playerDetectionRay.Enabled = false;
    }

    public bool PlayerDetectionBehaviour()
    {
	    if (!_playerInRange)
	    {
		    _canSeePlayer = false;
	    } 
	    else if (_playerInRange)
	    {
		    _playerDetectionRay.Rotation = GetAngleTo(_player.GlobalPosition);
		    PlayerPosition = ToLocal(_player.GlobalPosition);

		    // Since rotation is being applied, Delay the checking of the raycasts collisions to the next frame
		    CallDeferred(nameof(CheckRaycast));
	    }
	    
	    return _canSeePlayer;
    }

    private void CheckRaycast()
    {
	    if (!_playerDetectionRay.IsColliding())
	    {
		    _canSeePlayer = true;
	    }
	    else
	    {
		    _canSeePlayer = false;
	    }
	    
    }
}