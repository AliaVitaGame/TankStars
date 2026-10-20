using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
   [SerializeField] private WeaponPoolObject poolProjectile;
   private MainActions _gameInput;
   private WeaponController _weaponController;
   private BaseVehicleController _controller;
   
   public static PlayerInputManager Instance { get; private set; }

   private void OnEnable() 
      => CharacterSpawnerBase.PlayerInitializedEvent += Initialize;
   private void OnDisable() 
      => CharacterSpawnerBase.PlayerInitializedEvent -= Initialize;

   private void OnDestroy()
   {
      if(_gameInput != null)
       _gameInput.Disable();
   }

   private void Awake()
   {
      if(Instance) Destroy(gameObject);
      else Instance = this;
   }

   private void Initialize()
   {
      InitializeController();
      _weaponController = GetComponentInChildren<WeaponController>();
      
      _weaponController.SetPoolProjectile(poolProjectile);
      
      if(_controller ==  null)
         Debug.LogError("No vehicle controller found");

      _gameInput.Gameplay.Shot.performed += Shoot;
      
      _gameInput.Enable(); 
   }

   private void Update()
   {
      if(_gameInput == null) return;
      
      ReadMovement();
      ReadAim();
   }

   private void ReadMovement()
   {
      Vector2 movement = _gameInput.Gameplay.Movement.ReadValue<Vector2>();
      _controller.SetMove(movement);
   }

   private void ReadAim()
   {
      Vector2 aimPos = _gameInput.Gameplay.AimDirection.ReadValue<Vector2>();
      _controller.SetAim(aimPos);
   }

   private void Shoot(InputAction.CallbackContext context)
   {
      _weaponController.StartShoot();
   }
   
   public MainActions GetInput() => _gameInput;

   public void SetInverted(bool isInvertedZ, bool isInvertedX)
   {
      InitializeController();
      _controller.SetInverted(isInvertedZ, isInvertedX);
   }

   private void InitializeController()
   {
      if(_controller) return;
      
      _gameInput = new MainActions();
      _controller = GetComponentInChildren<BaseVehicleController>();
   }
}
