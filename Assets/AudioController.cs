using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    //Audioミキサーを入れるとこ
    [SerializeField] AudioMixer audioMixer;

    //それぞれのスライダーを入れるとこ
    [SerializeField] Slider BGMSlider;
    [SerializeField] Slider SESlider;

    private void Start()
    {
        if (BGMSlider != null)
        {
            BGMSlider.onValueChanged.AddListener((value) =>
            {
                value = Mathf.Clamp01(value);

                float decibel = 20f * Mathf.Log10(value);
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                audioMixer.SetFloat("BGM", decibel);
            });
        }

        if (SESlider != null)
        {
            SESlider.onValueChanged.AddListener((value) =>
            {
                value = Mathf.Clamp01(value);

                float decibel = 20f * Mathf.Log10(value);
                decibel = Mathf.Clamp(decibel, -80f, 0f);
                audioMixer.SetFloat("SE", decibel);
            });
        }
    }
}
