using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField] private AudioSource[] bgmSources;
    [SerializeField] private Transform seParent;
    [SerializeField] private Transform settingWindow;
    [SerializeField] private PlayerController playerController;
    private List<AudioSource> seSources = new List<AudioSource>();
    private float bgmVol = 0.1f, seVol = 1f, sens = 150f;
    private float minSens = 10f;

    private void Awake()
    {
        LoadSetting();
        for (int i = 0; i < seParent.childCount; i++)
        {
            var aud = seParent.GetChild(i).GetComponent<AudioSource>();
            seSources.Add(aud);
        }
        foreach (AudioSource bgm in bgmSources)
            bgm.volume = bgmVol;
        foreach (AudioSource se in seSources)
            se.volume = seVol;
        playerController.SensitivitySetting(sens);
    }

    public void ChangeBGMVolume(Slider _slider)
    {
        bgmVol = _slider.value;
        foreach (AudioSource bgm in bgmSources)
            bgm.volume = bgmVol;
    }

    public void ChangeSEVolume(Slider _slider)
    {
        seVol = _slider.value;
        foreach (AudioSource se in seSources)
            se.volume = seVol;
    }

    public void ChangeSensitivity(Slider _slider)
    {
        sens = _slider.value;
        if (sens < minSens) sens = minSens;
        playerController.SensitivitySetting(sens);
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetFloat("BGM",bgmVol);
        PlayerPrefs.SetFloat("SE",seVol);
        PlayerPrefs.SetFloat("Sens",sens);
    }

    private void LoadSetting()
    {
        bgmVol = PlayerPrefs.GetFloat("BGM",bgmVol);
        settingWindow.GetChild(0).GetComponent<Slider>().value = bgmVol;
        seVol = PlayerPrefs.GetFloat("SE",seVol);
        settingWindow.GetChild(1).GetComponent<Slider>().value = seVol;
        sens = PlayerPrefs.GetFloat("Sens",sens);
        settingWindow.GetChild(2).GetComponent<Slider>().value = sens;
    }
}
