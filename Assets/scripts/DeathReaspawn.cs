using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class death : MonoBehaviour
{
    public Animator animator;
    private bool shouldWait=false;
    public float timer;
    public Volume postProcessVolume;
    private Vignette vignette;
    // Update is called once per frame
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
        if (shouldWait)
        { 
            float time= Time.deltaTime;
            timer-=time;
        }
        if (timer < 0)
        {
            SceneManager.LoadScene(1);
        }
    }
    public void deaath()
    {
        animator.SetTrigger("end");
        shouldWait = true;
        if (vignette != null)
        {
            // Temporarily change vignette color to red and increase intensity
            vignette.color.value = Color.red;
            vignette.intensity.value = 0.6f; // Adjust as needed

            // Start a coroutine to fade the effect back
            StartCoroutine(FadeVignetteBack());
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
