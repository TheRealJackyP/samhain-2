using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour
{
    [SerializeField] Slider _slider;

    void Start()
    {
        AudioManager.Instance.ChangeMasterVolume(_slider.value);
        _slider.onValueChanged.AddListener(Val => AudioManager.Instance.ChangeMasterVolume(Val));
    }
}
