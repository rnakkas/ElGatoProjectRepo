using Godot;
using System;

namespace ElGatoProject.Components.Scripts;

[GlobalClass]
public partial class PlayerDetectionComponent : Node2D
{
    [Export] private Area2D _playerDetectionArea;
    [Export] private RayCast2D _playerDetectionRay;

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
	    _player = playerArea;
    }

    private void OnPlayerExitedDetectionArea(Area2D playerArea)
    {
	    if (!playerArea.IsInGroup("Players"))
		    return;
	    _playerInRange = false;
    }

    public bool PlayerDetectionBehaviour()
    {
	    if (_playerInRange)
	    {
		    _playerDetectionRay.Enabled = true;
		    _playerDetectionRay.TargetPosition = ToLocal(_player.GlobalPosition);
		    _canSeePlayer = true;
	    }
	    else
	    {
		    _playerDetectionRay.Enabled = false;
		    _canSeePlayer = false;
	    }

	    return _canSeePlayer;
    }
}


/*
 *
 * IF player detection area is null
 *	- set the player detection ray to a certain range value (set this value as an [export], for melee enemies
 *	- IF player detection ray collides with player
 *		- _canSeePlayer = true
 * 
 * 
 * IF player is in detection area
	- playerInRange = true
	- activate the player detection ray
	- set the player detection ray to point towards player's position
	- IF player detection ray is not colliding with wall
		- canSeePlayer = true
		
	Pass the playerInRange and canSeePlayer values back to parent
 * 
 */
