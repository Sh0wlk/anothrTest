using EasyTextEffects;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class score : MonoBehaviour
{
    public TextMeshProUGUI scoree;
    public float scoreCou=0f;
    public float prevScore=0f;
    public TextEffect TextEffect;
    public TextMeshProUGUI timer;
    public float time;
    public Volume postProcessVolume;
    private Vignette vignette;
    public Color color = Color.yellow;
    void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            // Try to get the Vignette component from the VolumeProfile
            if (!postProcessVolume.profile.TryGet(out vignette))
            {
                Debug.LogError("Vignette effect not found in the Volume Profile!");
            }
        }
        else
        {
            Debug.LogError("Post Process Volume or Profile not assigned/found!");
        }
    }
    void Update()
    {
        time = Time.time;
        timer.text=time.ToString();
        if (prevScore != scoreCou)
        {
            prevScore = scoreCou;
            scoree.text=scoreCou.ToString()+"$";
            TextEffect.Refresh();
            if (vignette != null)
            {
                // Temporarily change vignette color to red and increase intensity
                vignette.color.value = color;
                vignette.intensity.value = 0.6f; // Adjust as needed

                // Start a coroutine to fade the effect back
                StartCoroutine(FadeVignetteBack());
            }
        }
    }
    System.Collections.IEnumerator FadeVignetteBack()
    {
        float duration = 0.5f; // Duration of the fade
        float timer = 0f;
        Color startColor = vignette.color.value;
        float startIntensity = vignette.intensity.value;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;

            vignette.color.value = Color.Lerp(startColor, Color.black, progress); // Fade back to black
            vignette.intensity.value = Mathf.Lerp(startIntensity, 0.4f, progress); // Fade back to base intensity (adjust as needed)

            yield return null;
        }

        // Ensure it's fully reset
        vignette.color.value = Color.black;
        vignette.intensity.value = 0.4f;
    }
}
