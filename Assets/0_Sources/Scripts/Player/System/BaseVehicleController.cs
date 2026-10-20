using UnityEngine;

public abstract class BaseVehicleController : MonoBehaviour
{
  protected Vector2 MoveInput;
  protected Vector2 AimDirection;
  protected bool IsInvertedZ;
  protected bool IsInvertedX;

  public virtual void SetMove(Vector2  move) => MoveInput =  move;
  public virtual void SetAim(Vector2 aim) => AimDirection = aim;

  public virtual void SetInverted(bool invertedZ, bool isInvertedX)
  {
    IsInvertedZ = invertedZ;
    IsInvertedX = isInvertedX;
  }
}
