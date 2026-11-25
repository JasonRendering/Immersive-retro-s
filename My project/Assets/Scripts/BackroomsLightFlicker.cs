using UnityEngine;

public class BackroomsLightFlicker : MonoBehaviour
{
    private Light targetLight;
    private float originalIntensity;
    private float timer;

    private void Start()
    {
        targetLight = GetComponent<Light>();

        if (targetLight != null)
        {
            originalIntensity = targetLight.intensity;
        }

        timer = Random.Range(1f, 5f);
    }

    private void Update()
    {
        if (targetLight == null)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Occasional brief fluorescent flicker
            if (Random.value < 0.35f)
            {
                targetLight.intensity =
                    originalIntensity * Random.Range(0.1f, 0.5f);
            }
            else
            {
                targetLight.intensity = originalIntensity;
            }

            timer = Random.Range(0.05f, 0.2f);
        }
    }
}