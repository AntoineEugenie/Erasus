using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("Volume Settings")]
    [SerializeField] private TMP_Text _volumeTextValue;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private float _defaultVolume = 1.0f;
    
    [Header("Gameplay Settings")]
    [SerializeField] private TMP_Text _controllerSenTextValue;
    [SerializeField] private Slider  _controllerSenSlider;
    [SerializeField] private int _defaultSen = 4;
    public int mainControllerSen = 4;
    
    [Header("Toggle Settings")]
    [SerializeField] private Toggle _invertYToggle;
    
    [Header("Graphics Settings")]
    [SerializeField] private Slider _brightnessSlider;
    [SerializeField] private TMP_Text _brightnessTextValue;
    [SerializeField] private float _defaultBrightness = 1;

    private int _qualityLevel;
    private bool _isFullScreen;
    private float _brightnessLevel;
    
    [Header("Confirmation")]
    [SerializeField] private GameObject _confirmPrompt;
    
    [Header("Levels to Load")]
    public string newGameLevel;
    private string _levelToLoad;
    [SerializeField] private GameObject noSavedGameDialog;
    
    [Header("Resolution Dropdowns")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] _resolutions;


    private void Start()
    {
        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        
        var options = new List<string>();
        int currentResolutionIndex = 0;
        
        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = _resolutions[i].width + " x " + _resolutions[i].height;
            options.Add(option);

            if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = _resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void NewGameDialogYes()
    {
        SceneManager.LoadScene(newGameLevel);
    }

    public void LoadGameDialogYes()
    {
        if (PlayerPrefs.HasKey("savedLevel"))
        {
            _levelToLoad = PlayerPrefs.GetString("savedLevel");
            SceneManager.LoadScene(_levelToLoad);
        }
        else
        {
            noSavedGameDialog.SetActive(true);
        }
    }
    
    public void ExitButton()
    {
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        _volumeTextValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("volume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        _controllerSenTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (_invertYToggle.isOn)
        {
            PlayerPrefs.SetInt("invertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("invertY", 0);
        }
        PlayerPrefs.SetFloat("sensitivity", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        _brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullScreen(bool isFullScreen)
    {
        _isFullScreen = isFullScreen;
    }

    public void SetQuality(int qualityLevel)
    {
        _qualityLevel = qualityLevel;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("brightness", _brightnessLevel);
        PlayerPrefs.SetInt("quality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel, true);
        PlayerPrefs.SetInt("fullscreen", _isFullScreen ? 1 : 0);
        Screen.fullScreen = _isFullScreen;
        StartCoroutine(ConfirmationBox());        
    }

    public void ResetButton(string menuType)
    {
        if (menuType == "Audio")
        {
            AudioListener.volume = _defaultVolume;
            _volumeSlider.value = _defaultVolume;
            _volumeTextValue.text = _defaultVolume.ToString("0.0");
            VolumeApply();
        }
        else if (menuType == "Gameplay")
        {
            _controllerSenTextValue.text = _defaultSen.ToString("0");
            _controllerSenSlider.value = _defaultSen;
            mainControllerSen = _defaultSen;
            _invertYToggle.isOn = false;
            GameplayApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        _confirmPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        _confirmPrompt.SetActive(false);
    }
}
