using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioToggleUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Image slashIcon;

    [Header("Settings")]
    [SerializeField] private string source;
    [SerializeField] private float defaultVolume = 0f;

    const float MUTE_VOLUME = -80f;

    private void Start()
    {
        if (defaultVolume == MUTE_VOLUME)
            throw new System.Exception($"Default volume cannot be {defaultVolume}!");

        bool found = audioMixer.GetFloat(source, out float volume);
        if (!found)
            throw new System.Exception($"Parameter '{source}' does not exit!");

        // Hide mute based on state
        bool muted = volume != defaultVolume;
        slashIcon.enabled = muted;
    }

    public void Toggle()
    {
        bool found = audioMixer.GetFloat(source, out float volume);
        if (!found) throw new System.Exception($"Parameter '{source}' does not exit!");

        // Toggle current state
        if (volume == defaultVolume) // Not mute
        {
            audioMixer.SetFloat(source, MUTE_VOLUME);
            slashIcon.enabled = true;
        }
        else
        {
            audioMixer.SetFloat(source, defaultVolume);
            slashIcon.enabled = false;
        }
    }
}
