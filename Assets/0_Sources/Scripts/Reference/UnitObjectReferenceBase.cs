using UnityEngine;

public abstract class UnitObjectReferenceBase : MonoBehaviour
{
   public abstract void RestoreSettings();
   public virtual void RestorePosition(Vector3 position) => transform.position = position;
}
