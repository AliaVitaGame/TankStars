using System.Collections;
using UnityEngine;

public class VehicleWreckage : MonoBehaviour
{
    public void BeginFade(float visibleDuration, float fadeDuration)
    {
        StartCoroutine(FadeAndDestroy(visibleDuration, fadeDuration));
    }

    private IEnumerator FadeAndDestroy(float visibleDuration, float fadeDuration)
    {
        yield return new WaitForSeconds(visibleDuration);

        var renderers = GetComponentsInChildren<MeshRenderer>(true);
        var materials = new Material[renderers.Length][];
        var startColors = new Color[renderers.Length][];

        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].materials;
            startColors[i] = new Color[materials[i].Length];
            for (int m = 0; m < materials[i].Length; m++)
            {
                VehicleMaterialUtil.MakeTransparent(materials[i][m]);
                startColors[i][m] = VehicleMaterialUtil.GetColor(materials[i][m]);
            }
        }

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(time / fadeDuration);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null)
                    continue;

                for (int m = 0; m < materials[i].Length; m++)
                {
                    Color color = startColors[i][m];
                    color.a *= alpha;
                    VehicleMaterialUtil.ApplyColor(materials[i][m], color);
                }
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
