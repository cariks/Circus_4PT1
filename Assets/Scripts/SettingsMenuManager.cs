using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using System.Collections.Generic;

public class SettingsMenuManager : MonoBehaviour
{
    public Slider musicVol, sfxVol;
    public AudioMixer mainAudioMixer;
    public Toggle musicToggle, sfxToggle;

    private float defaultMusicVolume;
    private float defaultSfxVolume;
    private float lastMusicVolume; // Переменная для хранения последнего значения громкости музыки
    private float lastSfxVolume;   // Переменная для хранения последнего значения громкости SFX

    public TMP_Dropdown ResDropDown;
    public Toggle ShaderToggle, FullScreenToggle;

    private const string ShaderPrefKey = "ShadersEnabled"; // Ключ для хранения

    Resolution[] AllResolutions;
    bool isShaderOn;
    bool IsFullScreen;
    int SelectedResolution;
    List<Resolution> SelectedResolutionList = new List<Resolution>();

    void Start()
    {
        // Получаем текущие значения громкости из Audio Mixer
        mainAudioMixer.GetFloat("MusicVol", out defaultMusicVolume);
        mainAudioMixer.GetFloat("SfxVol", out defaultSfxVolume);

        // Инициализируем переменные lastVolume текущими значениями громкости
        lastMusicVolume = defaultMusicVolume;
        lastSfxVolume = defaultSfxVolume;

        // Устанавливаем обработчики событий для Toggle
        musicToggle.onValueChanged.AddListener(OnMusicToggleValueChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxToggleValueChanged);

        // Инициализируем состояние Toggle в зависимости от громкости при старте
        musicToggle.isOn = !(defaultMusicVolume > -80f); // Инвертируем условие
        sfxToggle.isOn = !(defaultSfxVolume > -80f); // Инвертируем условие

        // Устанавливаем начальное состояние слайдеров
        musicVol.interactable = !musicToggle.isOn;
        sfxVol.interactable = !sfxToggle.isOn;



        // Загружаем сохранённое значение и применяем его к Toggle
        ShaderToggle.isOn = PlayerPrefs.GetInt(ShaderPrefKey, 1) == 1;

        // Подписываемся на изменение Toggle
        ShaderToggle.onValueChanged.AddListener(OnToggleChanged);


        IsFullScreen = true;
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach(Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();
            if(!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }
            resolutionStringList.Add(res.ToString());
        }

        ResDropDown.AddOptions(resolutionStringList);
    }

    private void OnToggleChanged(bool isOn)
    {
        PlayerPrefs.SetInt(ShaderPrefKey, isOn ? 1 : 0);
        PlayerPrefs.Save(); // Сохраняем значение в настройках

        // Передаём новое значение в GameManager
        GameManager.Instance.SetShaders(isOn);
    }

    public void ChangeResolution()
    {
        SelectedResolution = ResDropDown.value;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);
    }

    public void ChangeFullScreen()
    {
        IsFullScreen = FullScreenToggle.isOn;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);
    }

    public void ChangeMusicVolume()
    {
        if (!musicToggle.isOn) // Если музыка не выключена Toggle
        {
            mainAudioMixer.SetFloat("MusicVol", musicVol.value);
            lastMusicVolume = musicVol.value; // Сохраняем текущее значение громкости
        }
    }

    public void ChangeSfxVolume()
    {
        if (!sfxToggle.isOn) // Если SFX не выключены Toggle
        {
            mainAudioMixer.SetFloat("SfxVol", sfxVol.value);
            lastSfxVolume = sfxVol.value; // Сохраняем текущее значение громкости
        }
    }

    private void OnMusicToggleValueChanged(bool isOn)
    {
        musicVol.interactable = !isOn; // Инвертируем условие для interactable
        if (isOn)
        {
            mainAudioMixer.SetFloat("MusicVol", -80f); // Отключаем звук
        }
        else
        {
            mainAudioMixer.SetFloat("MusicVol", lastMusicVolume); // Восстанавливаем последнее значение громкости
        }
    }

    private void OnSfxToggleValueChanged(bool isOn)
    {
        sfxVol.interactable = !isOn; // Инвертируем условие для interactable
        if (isOn)
        {
            mainAudioMixer.SetFloat("SfxVol", -80f); // Отключаем звук
        }
        else
        {
            mainAudioMixer.SetFloat("SfxVol", lastSfxVolume); // Восстанавливаем последнее значение громкости
        }
    }
}