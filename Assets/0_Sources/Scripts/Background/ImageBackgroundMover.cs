using UnityEngine;

public class ImageBackgroundMover : MonoBehaviour
{
  [SerializeField] private Vector2 scrollDirection;
  [SerializeField] private Material material;
  [SerializeField] private float smoothnessFactor = 100;

  private void Update() 
    => material.mainTextureOffset = -scrollDirection * Time.time / smoothnessFactor;
}
