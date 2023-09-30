using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSettingsUI : MonoBehaviour
{
    [SerializeField] private Toggle speedrunToggle;
    [SerializeField] private Toggle deathCountToggle;
    [SerializeField] private TMP_InputField playerNameInput;

    private void Start()
    {
        // Update current states
        speedrunToggle.isOn = GameManager.instance.SpeedrunEnabled();
        deathCountToggle.isOn = GameManager.instance.DeathCounterEnabled();
        playerNameInput.text = GameManager.instance.GetPlayerName();
    }

    public void EnableSpeedrun(bool state)
    {
        GameManager.instance.EnableSpeedrunTimer(state);
    }

    public void EnableDeathCounter(bool state)
    {
        GameManager.instance.EnableDeathCounter(state);
    }

    public void ChangePlayerName(string name)
    {
        GameManager.instance.SetPlayerName(name);
    }
}
