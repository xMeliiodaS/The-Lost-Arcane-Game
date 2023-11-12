using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;           // Reference to the AudioMixer to control audio settings
    [SerializeField] private TMP_Dropdown resolutionDropdown; // Dropdown UI element for selecting resolution
    [SerializeField] private Slider volumeSlider;             // Slider UI element for adjusting main volume
    [SerializeField] private Slider volumeSliderEffect;       // Slider UI element for adjusting sound effects volume
    [SerializeField] private TMP_Dropdown qualityDropdown;    // Dropdown UI element for selecting graphics quality
    [SerializeField] private Toggle fullscreenToggle;         // Toggle UI element for toggling fullscreen mode

    private Resolution[] resolutions;

    private void Start()
    {
        // Get available screen resolutions
        resolutions = Screen.resolutions;

        // Clear resolution dropdown options
        resolutionDropdown.ClearOptions();
        List<string> options = new();

        int currentResolutionIndex = 0;

        // Populate dropdown options and find the current resolution index
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        // Add resolution options to the dropdown
        resolutionDropdown.AddOptions(options);

        // Load the saved resolution index
        int savedResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", currentResolutionIndex);
        resolutionDropdown.value = savedResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Load the saved volume
        float savedVolume = PlayerPrefs.GetFloat("BackgroundMusic", 0f);
        audioMixer.SetFloat("BackgroundMusic", savedVolume);
        volumeSlider.value = savedVolume;

        // Load and set the saved volume for sound effects
        float savedVolumesavedVolumeEffect = PlayerPrefs.GetFloat("SoundsEffect", 0f);
        audioMixer.SetFloat("SoundsEffect", savedVolumesavedVolumeEffect);
        volumeSliderEffect.value = savedVolumesavedVolumeEffect;

        // Load the saved quality level
        int savedQualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(savedQualityLevel);
        qualityDropdown.value = savedQualityLevel;

        // Load the saved fullscreen mode
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        Screen.fullScreen = savedFullscreen;
        fullscreenToggle.isOn = savedFullscreen;
    }


    /// <summary>
    /// Sets the background music volume using the specified value.
    /// </summary>
    /// <param name="volume">The new volume value.</param>
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("BackgroundMusic", volume);

        // Save the volume
        PlayerPrefs.SetFloat("BackgroundMusic", volume);
    }


    /// <summary>
    /// Sets the sound effects volume using the specified value.
    /// </summary>
    /// <param name="volume">The new volume value.</param>
    public void SetVolumeEffect(float volume)
    {
        audioMixer.SetFloat("SoundsEffect", volume);

        // Save the volume
        PlayerPrefs.SetFloat("SoundsEffect", volume);
    }


    /// <summary>
    /// Sets the graphics quality level using the specified index.
    /// </summary>
    /// <param name="qualityIndex">The index of the quality level to set.</param>
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        // Save the quality level
        PlayerPrefs.SetInt("QualityLevel", qualityIndex);
    }


    /// <summary>
    /// Toggles fullscreen mode based on the specified value.
    /// </summary>
    /// <param name="isFullscreen">Whether to enable fullscreen mode or not.</param>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // Save the fullscreen mode
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }


    /// <summary>
    /// Sets the screen resolution based on the specified index.
    /// </summary>
    /// <param name="resolutionIndex">The index of the resolution to set.</param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Save the resolution index
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
    }
}
