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
		public Vector2 MousePosition { get; private set; }

		public event Action Jump;
		public event Action<bool, bool> Crouch;
		public event Action<bool, bool> Sprint;
		public event Action<bool> Shoot;
		public event Action<bool> Aim;
		public event Action<bool> LeanRight;
		public event Action<bool> LeanLeft;
		public event Action Reload;
		public event Action<float> SwitchWeapon;

		protected override void Awake()
		{
			base.Awake();

			pia ??= new PlayerInputActions();
			
			EnableInput();
			IsReady = true;
		}

		private void Update()
		{
			if (!IsReady) return;
			
			MovementInput = movementAction.ReadValue<Vector2>();
			MousePosition = lookAction.ReadValue<Vector2>();
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

			pia.Player.SwitchWeapon.performed += OnSwitchWeaponPerformed;
			pia.Player.SwitchWeapon.Enable();

			removeListeners = RemoveListeners;
			
			void OnShootPerformed(InputAction.CallbackContext _) => Shoot?.Invoke(true);
			void OnShootCancelled(InputAction.CallbackContext _) => Shoot?.Invoke(false);
			void OnReloadPerformed(InputAction.CallbackContext _) => Reload?.Invoke();
			void OnSwitchWeaponPerformed(InputAction.CallbackContext _) => SwitchWeapon?.Invoke(pia.Player.SwitchWeapon.ReadValue<float>());

			void RemoveListeners()
			{
				pia.Player.Shoot.performed -= OnShootPerformed;
				pia.Player.Shoot.canceled -= OnShootCancelled;
				pia.Player.Reload.performed -= OnReloadPerformed;
				pia.Player.SwitchWeapon.performed -= OnSwitchWeaponPerformed;
			}
		}

		private void DisableInput()
		{
			removeListeners?.Invoke();
			
			movementAction.Disable();
			lookAction.Disable();
			
			pia.Player.Shoot.Disable();
			pia.Player.Reload.Disable();
			pia.Player.SwitchWeapon.Disable();
		}
	}
}