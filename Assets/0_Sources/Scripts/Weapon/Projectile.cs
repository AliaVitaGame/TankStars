using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem collisionEffect;
    
    private int _damage;
    private float _speedProjectile;
    private float _liveTimeProjectile;
    
    private GameObject _shooter;
    private Rigidbody _rigidbody;
    private Vector3 _directionMove;
    private Collider[] _colliders;

    private bool _isInitialized;

    private void Start()
    {
        collisionEffect.transform.SetParent(null);
    }

    public void SetStats(WeaponData data, Vector3 directionMove, Collider shooter)
    {
        _damage = data.Damage;
        _speedProjectile = data.SpeedProjectile;
        _liveTimeProjectile = data.LiveTimeProjectile;
        _directionMove =  directionMove;
        _shooter =  shooter.gameObject;
        
        StopAllCoroutines();
        StartCoroutine(DeactivateObjectTimer());

        if (_isInitialized == false)
        {
            _rigidbody = GetComponent<Rigidbody>();
            _colliders = GetComponentsInChildren<Collider>();

            if(_colliders.Length == 0) return;
            
            foreach (var t in _colliders)
                Physics.IgnoreCollision(t, shooter);
            
            _isInitialized = true;
        }
    }
    
    private void FixedUpdate()
    {
        if(_isInitialized == false) return;
        _rigidbody.linearVelocity = _directionMove * _speedProjectile * Time.fixedDeltaTime;
    }

    private void Damage(Transform target)
    {
        if (!target) return;
        
        if (target.TryGetComponent(out IUnitHealth health))
            health.TakeDamage(_damage, _shooter);
        
        gameObject.SetActive(false);
    }

    private void PlayEffect(Vector3 position, Vector3 normal)
    {
        collisionEffect.transform.position = position;
        collisionEffect.transform.rotation = Quaternion.LookRotation(normal);
        collisionEffect.Play();
    }

    private IEnumerator DeactivateObjectTimer()
    {
        yield return new WaitForSeconds(_liveTimeProjectile);
        gameObject.SetActive(false);
    }


    private void OnCollisionEnter(Collision other)
    {
        var contact = other.GetContact(0);
        PlayEffect(contact.point, contact.normal);
        Damage(other.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayEffect(transform.position, -_directionMove);
        Damage(other.transform);
    }
}
