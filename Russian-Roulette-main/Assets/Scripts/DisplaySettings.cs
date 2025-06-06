using UnityEngine;
using TMPro;      // TextMeshPro용
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplaySettings : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;

    Resolution[] resolutions;

    void Start()
    {
        // 1) 해상도 리스트 불러오기
        resolutions = Screen.resolutions;
        var options = new List<string>();

        int currentIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var r = resolutions[i];
            string option = $"{r.width} × {r.height}";
            options.Add(option);
            if (r.width == Screen.width && r.height == Screen.height)
                currentIndex = i;
        }

        // 2) 드롭다운 세팅
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();

        // 3) 이벤트 바인딩
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        fullScreenToggle.isOn = Screen.fullScreen;
        fullScreenToggle.onValueChanged.AddListener(SetFullScreen);
    }

    public void SetResolution(int index)
    {
        var r = resolutions[index];
        Screen.SetResolution(r.width, r.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFull)
    {
        Screen.fullScreen = isFull;
    }
}
