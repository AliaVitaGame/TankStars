using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShakeController : MonoBehaviour
{
   [SerializeField] private CinemachineCamera playerCamera;
   
   private bool _isShaking;
   
   private CinemachineBasicMultiChannelPerlin _perlin;
   public static CameraShakeController Instance { get; private set; }

   private void Start()
   {
      Instance = this;
      _perlin = playerCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
   }

   public void Shake(float amplitude = 1.3f, float duration = 0.3f)
   {
      if(_isShaking) return;
      StartCoroutine(ShakeRoutine(amplitude, duration));
   }

   private IEnumerator ShakeRoutine(float amplitude, float duration)
   {
      _isShaking = true;
      _perlin.AmplitudeGain = amplitude;
      yield return new WaitForSeconds(duration);
      _isShaking = false;
      _perlin.AmplitudeGain = 0;
   }
}
