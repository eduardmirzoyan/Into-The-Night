using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Security.Cryptography;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemachine;

    private Coroutine coroutine;
    public static CameraShake instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        cinemachine = GetComponent<CinemachineVirtualCamera>();
    }

    public void ScreenShake(float magnitude, float duration)
    {
        // Start shaking screen
        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(Shake(magnitude, duration));
    }

    private IEnumerator Shake(float magnitude, float duration)
    {
        CinemachineBasicMultiChannelPerlin perlin = cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = magnitude;

        // Start timer
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float intensity = Mathf.Lerp(magnitude, 0f, elapsed / duration);
            perlin.m_AmplitudeGain = intensity;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore position and rotation
        perlin.m_AmplitudeGain = 0f;
        transform.rotation = Quaternion.identity;
    }
}
