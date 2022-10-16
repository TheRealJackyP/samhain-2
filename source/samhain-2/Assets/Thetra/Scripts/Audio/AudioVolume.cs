using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioVolume : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] Slider _slider2;
    void Start()
    {
        if( _slider2 != null )
        _slider.value = _slider2.value;
        AudioManager.Instance.ChangeMasterVolume(_slider.value);
     
        _slider.onValueChanged.AddListener(Val => AudioManager.Instance.ChangeMasterVolume(Val));
    }
}
