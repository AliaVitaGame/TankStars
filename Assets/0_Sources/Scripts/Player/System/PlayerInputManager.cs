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
      InitializeController(true);
      BindWeaponAndInput();
   }

   private void Update()
   {
      if(_gameInput == null || _controller == null) return;
      
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
      if (_weaponController == null)
         _weaponController = GetComponentInChildren<WeaponController>(true);

      if (_weaponController == null) return;
      _weaponController.StartShoot();
   }
   
   public MainActions GetInput() => _gameInput;

   public void SetInverted(bool isInvertedZ, bool isInvertedX)
   {
      InitializeController(false);
      if (_controller != null)
         _controller.SetInverted(isInvertedZ, isInvertedX);
   }

   public void RefreshCombat()
   {
      InitializeController(true);
      BindWeaponAndInput();
      if (_weaponController != null)
         _weaponController.ResetReload();
   }

   private void BindWeaponAndInput()
   {
      _weaponController = GetComponentInChildren<WeaponController>(true);
      if (_weaponController != null)
         _weaponController.SetPoolProjectile(poolProjectile);

      if (_controller == null)
         Debug.LogError("No vehicle controller found");

      if (_gameInput == null)
         _gameInput = new MainActions();

      _gameInput.Gameplay.Shot.performed -= Shoot;
      _gameInput.Gameplay.Shot.performed += Shoot;
      _gameInput.Enable();
   }

   private void InitializeController(bool forceRefresh)
   {
      if (_gameInput == null)
         _gameInput = new MainActions();

      if (forceRefresh || _controller == null)
         _controller = GetComponentInChildren<BaseVehicleController>(true);
   }
}
