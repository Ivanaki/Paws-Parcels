using System;
using MotorHome.Scripts;
using R3;
using Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Paws_Parcels.Scripts.Player
{
	public class MyInput : MonoBehaviour, IStopMove
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private bool _canMoving = true;
		
		private GameInputService _gameInputService;
		
		[Inject]
		private void Construct(GameInputService gameInputService)
		{
			_gameInputService = gameInputService;
		}

		private void OnEnable()
		{
			_gameInputService.InputSystem.Player.Move.performed += OnMove;
			_gameInputService.InputSystem.Player.Move.canceled += OnMove;

			_gameInputService.InputSystem.Look.Look.performed += OnLook;
			_gameInputService.InputSystem.Look.Look.canceled += OnLook;

			_gameInputService.InputSystem.Player.Jump.performed += OnJump;
			_gameInputService.InputSystem.Player.Jump.canceled += OnJump;

			_gameInputService.InputSystem.Player.Sprint.performed += OnSprint;
			_gameInputService.InputSystem.Player.Sprint.canceled += OnSprint;
		}

		private void OnDisable()
		{
			_gameInputService.InputSystem.Player.Move.performed -= OnMove;
			_gameInputService.InputSystem.Player.Move.canceled -= OnMove;

			_gameInputService.InputSystem.Look.Look.performed -= OnLook;
			_gameInputService.InputSystem.Look.Look.canceled -= OnLook;

			_gameInputService.InputSystem.Player.Jump.performed -= OnJump;
			_gameInputService.InputSystem.Player.Jump.canceled -= OnJump;

			_gameInputService.InputSystem.Player.Sprint.performed -= OnSprint;
			_gameInputService.InputSystem.Player.Sprint.canceled -= OnSprint;
		}

		public void OnMove(InputAction.CallbackContext obj)
		{
			if(_canMoving)
				MoveInput(obj.ReadValue<Vector2>());
		}

		public void OnLook(InputAction.CallbackContext obj)
		{
			if(cursorInputForLook && _canMoving)
			{
				LookInput(obj.ReadValue<Vector2>());
			}
		}

		public void OnJump(InputAction.CallbackContext obj)
		{
			if(_canMoving)
				JumpInput(obj.ReadValueAsButton());
		}

		public void OnSprint(InputAction.CallbackContext obj)
		{
			if(_canMoving)
				SprintInput(obj.ReadValueAsButton());
		}


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void StopMove()
		{
			move = Vector2.zero;
			look = Vector2.zero;
			jump = false;
			sprint = false;
			_canMoving = false;
		}

		public void StartMove()
		{
			_canMoving = true;
		}
	}
	
}
