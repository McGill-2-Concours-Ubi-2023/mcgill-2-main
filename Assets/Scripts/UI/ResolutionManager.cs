using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResolutionManager : MonoBehaviour
{
    Resolution[] resolutions;
    int currentResolutionIndex;
    private const string RESOLUTION_KEY = "resolution";
    [SerializeField]
    private TextMeshProUGUI resolutionText;
    [SerializeField]
    private bool windowed;

    private void Start()
    {
        resolutions = Screen.resolutions;
        currentResolutionIndex = PlayerPrefs.GetInt(RESOLUTION_KEY, 0);
        SetText();

    }

    public void NextResolution()
    {
        if (resolutions.Length == 0) return;

        if (currentResolutionIndex >= resolutions.Length - 1)
        {
            currentResolutionIndex = 0;
            while (resolutions[currentResolutionIndex].refreshRate != 60)
            {
                currentResolutionIndex++;
            }
        }
        else { 
            currentResolutionIndex++;
            while (resolutions[currentResolutionIndex].refreshRate != 60)
            {
                currentResolutionIndex++;
            }
        }
        SetText();
    }

    public void PreviousResolution()
    {
        if (resolutions.Length == 0) return;
        if (currentResolutionIndex <= 0)
        {
            currentResolutionIndex = resolutions.Length - 1;
            while (resolutions[currentResolutionIndex].refreshRate != 60)
            {
                currentResolutionIndex--;
            }
        }
        else { 
            currentResolutionIndex--;
            while (resolutions[currentResolutionIndex].refreshRate != 60)
            {
                currentResolutionIndex--;
            }
        }

        SetText();
    }

    public void SetText()
    {
        resolutionText.text = resolutions[currentResolutionIndex].width + " x " + resolutions[currentResolutionIndex].height;
    }

    public void Apply()
    {
        Screen.SetResolution(resolutions[currentResolutionIndex].width, resolutions[currentResolutionIndex].height, !windowed);
        PlayerPrefs.SetInt(RESOLUTION_KEY, currentResolutionIndex);
    }

    public void SetWindowed(bool tog)
    {
        windowed = tog;
    }


}
