using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour
{
    public Light targetLight;

    public float normalIntensity = 2f;
    public float minWait = 2f;
    public float maxWait = 7f;

    void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minWait, maxWait)
            );

            // A few rapid flashes
            int flashes = Random.Range(1, 5);

            for (int i = 0; i < flashes; i++)
            {
                targetLight.enabled = false;
                yield return new WaitForSeconds(
                    Random.Range(0.04f, 0.15f)
                );

                targetLight.enabled = true;
                yield return new WaitForSeconds(
                    Random.Range(0.04f, 0.2f)
                );
            }

            targetLight.intensity = normalIntensity;
        }
    }
}
