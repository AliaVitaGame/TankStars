using System.Collections.Generic;
using UnityEngine;

public class VehicleDeathVfx : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private GameObject smokePrefab;
    [SerializeField] private float explosionScale = 0.55f;
    [SerializeField] private float explosionForce = 14f;
    [SerializeField] private float explosionRadius = 4.5f;
    [SerializeField] private float explosionUpward = 1.15f;
    [SerializeField] private float wreckVisibleDuration = 2.4f;
    [SerializeField] private float wreckFadeDuration = 1.6f;
    [SerializeField] private float cameraShakeAmplitude = 2.4f;
    [SerializeField] private float cameraShakeDuration = 0.45f;

    private IUnitHealth _health;
    private GameObject _wreckage;

    private void OnEnable()
    {
        _health = GetComponent<IUnitHealth>();
        if (_health != null)
            _health.OnDeathEvent += HandleDeath;
    }

    private void OnDisable()
    {
        if (_health != null)
            _health.OnDeathEvent -= HandleDeath;
    }

    private void LateUpdate()
    {
        if (_wreckage != null && _health != null && _health.Health > 0)
        {
            Destroy(_wreckage);
            _wreckage = null;
        }
    }

    private void HandleDeath(GameObject victim)
    {
        SpawnExplosion();
        SpawnWreckage();

        if (CameraShakeController.Instance != null)
            CameraShakeController.Instance.Shake(cameraShakeAmplitude, cameraShakeDuration);
    }

    private void SpawnExplosion()
    {
        Vector3 position = GetVisualCenter();
        PlayPrefab(explosionPrefab, position);
        PlayPrefab(smokePrefab, position);
    }

    private void PlayPrefab(GameObject prefab, Vector3 position)
    {
        if (prefab == null)
            return;

        var instance = Instantiate(prefab, position, prefab.transform.rotation);
        instance.transform.localScale *= explosionScale;
        Destroy(instance, 8f);
    }

    private void SpawnWreckage()
    {
        if (_wreckage != null)
            Destroy(_wreckage);

        Transform visual = FindVisualRoot();
        if (visual == null || visual == transform)
            return;

        _wreckage = Instantiate(visual.gameObject, visual.position, visual.rotation);
        _wreckage.name = name + "_Wreckage";
        _wreckage.SetActive(true);
        StripClone(_wreckage);

        foreach (var collider in GetComponentsInChildren<Collider>(true))
            collider.enabled = false;

        Vector3 explosionPoint = GetBounds(_wreckage).center;
        var pieces = CollectPieceTransforms(_wreckage);

        for (int i = 0; i < pieces.Count; i++)
            pieces[i].SetParent(_wreckage.transform, true);

        bool splitLargeParts = pieces.Count < 5;
        foreach (var piece in pieces)
        {
            var filter = piece.GetComponent<MeshFilter>();
            var renderer = piece.GetComponent<MeshRenderer>();
            if (filter == null || renderer == null || filter.sharedMesh == null)
                continue;

            if (splitLargeParts && CanSplit(filter.sharedMesh))
            {
                var shards = SplitMesh(filter.sharedMesh);
                if (shards.Count > 1)
                {
                    foreach (var shard in shards)
                        CreatePhysicsPiece(piece, shard, renderer.sharedMaterials, explosionPoint);

                    Destroy(piece.gameObject);
                    continue;
                }
            }

            AddPhysics(piece, filter.sharedMesh, explosionPoint);
        }

        var wreckage = _wreckage.AddComponent<VehicleWreckage>();
        wreckage.BeginFade(wreckVisibleDuration, wreckFadeDuration);
    }

    private Transform FindVisualRoot()
    {
        var named = transform.Find("Model");
        if (named != null)
            return named;

        foreach (Transform child in transform)
        {
            if (HasSolidMesh(child))
                return child;
        }

        return HasSolidMesh(transform) ? transform : null;
    }

    private static bool HasSolidMesh(Transform root)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        foreach (var renderer in renderers)
        {
            if (IsSolidBodyRenderer(renderer))
                return true;
        }

        return false;
    }

    private static void StripClone(GameObject clone)
    {
        foreach (var particle in clone.GetComponentsInChildren<ParticleSystem>(true))
        {
            if (particle != null)
                particle.gameObject.SetActive(false);
        }

        foreach (var behaviour in clone.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (behaviour != null)
                Destroy(behaviour);
        }

        foreach (var animator in clone.GetComponentsInChildren<Animator>(true))
            Destroy(animator);

        foreach (var audio in clone.GetComponentsInChildren<AudioSource>(true))
            Destroy(audio);

        foreach (var collider in clone.GetComponentsInChildren<Collider>(true))
            Destroy(collider);

        foreach (var rigidbody in clone.GetComponentsInChildren<Rigidbody>(true))
            Destroy(rigidbody);

        foreach (var renderer in clone.GetComponentsInChildren<MeshRenderer>(true))
            renderer.enabled = true;
    }

    private static List<Transform> CollectPieceTransforms(GameObject root)
    {
        var pieces = new List<Transform>();
        var filters = root.GetComponentsInChildren<MeshFilter>(true);
        foreach (var filter in filters)
        {
            if (filter.sharedMesh == null)
                continue;
            if (!IsSolidBodyRenderer(filter.GetComponent<MeshRenderer>()))
                continue;
            pieces.Add(filter.transform);
        }

        pieces.Sort((a, b) => GetDepth(b).CompareTo(GetDepth(a)));
        return pieces;
    }

    private static int GetDepth(Transform transform)
    {
        int depth = 0;
        while (transform.parent != null)
        {
            depth++;
            transform = transform.parent;
        }

        return depth;
    }

    private void CreatePhysicsPiece(Transform source, Mesh mesh, Material[] materials, Vector3 explosionPoint)
    {
        var piece = new GameObject(source.name + "_Shard");
        piece.transform.SetPositionAndRotation(source.position, source.rotation);
        piece.transform.localScale = source.lossyScale;
        piece.transform.SetParent(_wreckage.transform, true);

        var filter = piece.AddComponent<MeshFilter>();
        filter.sharedMesh = mesh;

        var renderer = piece.AddComponent<MeshRenderer>();
        renderer.sharedMaterials = materials;
        renderer.enabled = true;

        AddPhysics(piece.transform, mesh, explosionPoint);
    }

    private void AddPhysics(Transform piece, Mesh mesh, Vector3 explosionPoint)
    {
        var collider = piece.gameObject.GetComponent<BoxCollider>();
        if (collider == null)
            collider = piece.gameObject.AddComponent<BoxCollider>();

        collider.center = mesh.bounds.center;
        collider.size = Vector3.Max(mesh.bounds.size, Vector3.one * 0.08f);

        var rigidbody = piece.gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null)
            rigidbody = piece.gameObject.AddComponent<Rigidbody>();

        float volume = Mathf.Max(0.05f, mesh.bounds.size.x * mesh.bounds.size.y * mesh.bounds.size.z);
        rigidbody.mass = Mathf.Clamp(volume * 0.35f, 0.15f, 8f);
        rigidbody.linearDamping = 0.35f;
        rigidbody.angularDamping = 0.55f;
        rigidbody.AddExplosionForce(explosionForce, explosionPoint, explosionRadius, explosionUpward, ForceMode.Impulse);
        rigidbody.AddTorque(Random.insideUnitSphere * explosionForce * 0.25f, ForceMode.Impulse);
    }

    private static bool CanSplit(Mesh mesh)
    {
        return mesh.isReadable && mesh.vertexCount > 24 && mesh.triangles.Length >= 72;
    }

    private static List<Mesh> SplitMesh(Mesh source)
    {
        var result = new List<Mesh>();
        if (!source.isReadable)
            return result;

        var vertices = source.vertices;
        var normals = source.normals;
        var uvs = source.uv;
        var triangles = source.triangles;
        Bounds bounds = source.bounds;
        Vector3 size = bounds.size;
        size.x = Mathf.Max(size.x, 0.01f);
        size.y = Mathf.Max(size.y, 0.01f);
        size.z = Mathf.Max(size.z, 0.01f);

        const int cells = 2;
        var buckets = new List<int>[cells * cells * cells];
        for (int i = 0; i < buckets.Length; i++)
            buckets[i] = new List<int>();

        for (int t = 0; t < triangles.Length; t += 3)
        {
            Vector3 centroid = (vertices[triangles[t]] + vertices[triangles[t + 1]] + vertices[triangles[t + 2]]) / 3f;
            Vector3 normalized = centroid - bounds.min;
            int x = Mathf.Clamp(Mathf.FloorToInt(normalized.x / size.x * cells), 0, cells - 1);
            int y = Mathf.Clamp(Mathf.FloorToInt(normalized.y / size.y * cells), 0, cells - 1);
            int z = Mathf.Clamp(Mathf.FloorToInt(normalized.z / size.z * cells), 0, cells - 1);
            buckets[x + y * cells + z * cells * cells].Add(t);
        }

        for (int i = 0; i < buckets.Length; i++)
        {
            if (buckets[i].Count == 0)
                continue;

            var map = new Dictionary<int, int>();
            var newVerts = new List<Vector3>();
            var newNormals = new List<Vector3>();
            var newUvs = new List<Vector2>();
            var newTris = new List<int>();

            foreach (int t in buckets[i])
            {
                for (int k = 0; k < 3; k++)
                {
                    int index = triangles[t + k];
                    if (!map.TryGetValue(index, out int mapped))
                    {
                        mapped = newVerts.Count;
                        map[index] = mapped;
                        newVerts.Add(vertices[index]);
                        if (normals != null && normals.Length > index)
                            newNormals.Add(normals[index]);
                        if (uvs != null && uvs.Length > index)
                            newUvs.Add(uvs[index]);
                    }

                    newTris.Add(mapped);
                }
            }

            var mesh = new Mesh { name = source.name + "_Shard" };
            mesh.SetVertices(newVerts);
            if (newNormals.Count == newVerts.Count)
                mesh.SetNormals(newNormals);
            if (newUvs.Count == newVerts.Count)
                mesh.SetUVs(0, newUvs);
            mesh.SetTriangles(newTris, 0);
            mesh.RecalculateBounds();
            if (newNormals.Count != newVerts.Count)
                mesh.RecalculateNormals();
            result.Add(mesh);
        }

        return result;
    }

    private Vector3 GetVisualCenter()
    {
        Transform visual = FindVisualRoot();
        if (visual != null)
            return GetBounds(visual.gameObject).center;

        return transform.position + Vector3.up * 0.6f;
    }

    private static Bounds GetBounds(GameObject root)
    {
        var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
        bool hasBounds = false;
        Bounds bounds = new Bounds(root.transform.position + Vector3.up * 0.6f, Vector3.one);

        foreach (var renderer in renderers)
        {
            if (!IsSolidBodyRenderer(renderer))
                continue;

            if (!hasBounds)
            {
                bounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        return bounds;
    }

    private static bool IsSolidBodyRenderer(MeshRenderer renderer)
    {
        if (renderer == null)
            return false;
        if (renderer.GetComponent<ParticleSystem>() != null)
            return false;

        return true;
    }
}
