using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenuManager : MonoBehaviour
{
    public Slider musicVol, sfxVol;
    public AudioMixer mainAudioMixer;
    public Toggle musicToggle, sfxToggle;

    private float defaultMusicVolume;
    private float defaultSfxVolume;
    private float lastMusicVolume; // Переменная для хранения последнего значения громкости музыки
    private float lastSfxVolume;   // Переменная для хранения последнего значения громкости SFX

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