using UnityEngine;

public class CameraScreenToWorldPoint : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask aimLayer;
    
    private Vector3 _lastAimPosition;
    public static CameraScreenToWorldPoint Instance { get; private set; }

    private void Start()
    {
        if(Instance) Destroy(gameObject);
        else Instance = this;
    }

    public Vector3 GetMousePosition()
    {
        var input = PlayerInputManager.Instance.GetInput();
        
        if(input == null)  return Vector3.zero;
        
        var mousePos = input.Gameplay.AimDirection.ReadValue<Vector2>();
       
        Ray ray = playerCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimLayer))
            _lastAimPosition = hit.point;
        
        return _lastAimPosition;
    }
}
