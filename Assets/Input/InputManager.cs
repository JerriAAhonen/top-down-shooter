using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace tds.Input
{
	public class InputManager : Singleton<InputManager>
	{
		private PlayerInputActions pia;
		private InputAction movementAction;
		private InputAction lookAction;
		private Action removeListeners;
		
		public bool IsReady { get; private set; }
		public Vector2 MovementInput { get; private set; }
		public Vector2 LookInput { get; private set; }

		public event Action Jump;
		public event Action<bool, bool> Crouch;
		public event Action<bool, bool> Sprint;
		public event Action<bool> Shoot;
		public event Action<bool> Aim;
		public event Action<bool> LeanRight;
		public event Action<bool> LeanLeft;
		public event Action Reload;
		public event Action SwitchWeapon;

		private void Start()
		{
			pia ??= new PlayerInputActions();
			
			EnableInput();
			IsReady = true;
		}

		private void Update()
		{
			if (!IsReady) return;
			
			MovementInput = movementAction.ReadValue<Vector2>();
			LookInput = lookAction.ReadValue<Vector2>();
		}

		private void OnEnable()
		{
			if (IsReady)
				EnableInput();
		}

		private void OnDisable()
		{
			DisableInput();
		}

		private void EnableInput()
		{
			movementAction = pia.Player.Move;
			movementAction.Enable();
			
			lookAction = pia.Player.Look;
			lookAction.Enable();
			
			pia.Player.Shoot.performed += OnShootPerformed;
			pia.Player.Shoot.canceled += OnShootCancelled;
			pia.Player.Shoot.Enable();

			pia.Player.Reload.performed += OnReloadPerformed;
			pia.Player.Reload.Enable();

			pia.Player.PrimaryWeapon.performed += OnPrimaryWeaponPerformed;
			pia.Player.PrimaryWeapon.Enable();

			pia.Player.SecondaryWeapon.performed += OnSecondaryWeaponPerformed;
			pia.Player.SecondaryWeapon.Enable();

			removeListeners = RemoveListeners;
			
			void OnShootPerformed(InputAction.CallbackContext _) => Shoot?.Invoke(true);
			void OnShootCancelled(InputAction.CallbackContext _) => Shoot?.Invoke(false);
			void OnReloadPerformed(InputAction.CallbackContext _) => Reload?.Invoke();
			void OnPrimaryWeaponPerformed(InputAction.CallbackContext _) => SwitchWeapon?.Invoke();
			void OnSecondaryWeaponPerformed(InputAction.CallbackContext _) => SwitchWeapon?.Invoke();

			void RemoveListeners()
			{
				pia.Player.Shoot.performed -= OnShootPerformed;
				pia.Player.Shoot.canceled -= OnShootCancelled;
				pia.Player.Reload.performed -= OnReloadPerformed;
				pia.Player.PrimaryWeapon.performed -= OnPrimaryWeaponPerformed;
				pia.Player.SecondaryWeapon.performed -= OnSecondaryWeaponPerformed;
			}
		}

		private void DisableInput()
		{
			removeListeners?.Invoke();
			
			movementAction.Disable();
			lookAction.Disable();
			
			pia.Player.Shoot.Disable();
			pia.Player.Reload.Disable();
		}
	}
}