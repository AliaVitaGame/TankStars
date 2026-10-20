using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleBushCamouflage : MonoBehaviour
{
    [SerializeField] private LayerMask shelterMask;
    [SerializeField] private float checkRadius = 1.1f;
    [SerializeField] [Range(0.3f, 1f)] private float darken = 0.72f;
    [SerializeField] [Range(0.2f, 1f)] private float hiddenAlpha = 0.62f;

    private static List<Renderer> _bushRenderers;
    private static bool _cacheReady;

    private IUnitHealth _health;
    private readonly Dictionary<MeshRenderer, Material[]> _originalMaterials = new();
    private readonly List<MeshRenderer> _bodyRenderers = new();
    private bool _applied;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetCache()
    {
        _bushRenderers = null;
        _cacheReady = false;
    }

    private void OnEnable()
    {
        _health = GetComponent<IUnitHealth>();
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        Restore();
    }

    private void Start()
    {
        CacheBodyRenderers();
        if (shelterMask.value == 0)
        {
            int shelter = LayerMask.NameToLayer("Shelter");
            if (shelter >= 0)
                shelterMask = 1 << shelter;
        }
    }

    private void Update()
    {
        if (_health != null && _health.Health <= 0)
            return;

        bool hidden = IsInsideBush();
        if (hidden == _applied)
            return;

        if (hidden)
            Apply();
        else
            Restore();
    }

    private bool IsInsideBush()
    {
        Vector3 probe = transform.position + Vector3.up * 0.45f;
        if (shelterMask.value != 0 && Physics.CheckSphere(probe, checkRadius, shelterMask, QueryTriggerInteraction.Collide))
            return true;

        EnsureBushCache();
        if (_bushRenderers == null)
            return false;

        for (int i = 0; i < _bushRenderers.Count; i++)
        {
            var renderer = _bushRenderers[i];
            if (renderer == null)
                continue;

            Bounds bounds = renderer.bounds;
            bounds.Expand(0.55f);
            if (bounds.Contains(probe))
                return true;
        }

        return false;
    }

    private void Apply()
    {
        CacheBodyRenderers();
        foreach (var renderer in _bodyRenderers)
        {
            if (renderer == null)
                continue;

            if (!_originalMaterials.ContainsKey(renderer))
                _originalMaterials[renderer] = renderer.sharedMaterials;

            var instances = renderer.materials;
            foreach (var material in instances)
            {
                VehicleMaterialUtil.MakeTransparent(material);
                Color original = VehicleMaterialUtil.GetColor(material);
                original.a = 1f;
                VehicleMaterialUtil.ApplyTint(material, original, darken, hiddenAlpha);
            }
        }

        _applied = true;
    }

    private void Restore()
    {
        foreach (var pair in _originalMaterials)
        {
            if (pair.Key != null)
                pair.Key.sharedMaterials = pair.Value;
        }

        _applied = false;
    }

    private void CacheBodyRenderers()
    {
        if (_bodyRenderers.Count > 0)
            return;

        foreach (var renderer in GetComponentsInChildren<MeshRenderer>(true))
        {
            if (renderer.GetComponent<ParticleSystem>() != null)
                continue;
            _bodyRenderers.Add(renderer);
        }
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _cacheReady = false;
        _bushRenderers = null;
    }

    private static void EnsureBushCache()
    {
        if (_cacheReady && _bushRenderers != null)
            return;

        _bushRenderers = new List<Renderer>();
        var renderers = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach (var renderer in renderers)
        {
            if (IsBushName(renderer.name) || (renderer.transform.parent != null && IsBushName(renderer.transform.parent.name)))
                _bushRenderers.Add(renderer);
        }

        _cacheReady = true;
    }

    private static bool IsBushName(string objectName)
    {
        if (string.IsNullOrEmpty(objectName))
            return false;

        return objectName.IndexOf("Bush", System.StringComparison.OrdinalIgnoreCase) >= 0
               || objectName.IndexOf("Clump", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
