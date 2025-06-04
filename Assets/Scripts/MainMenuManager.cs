#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject BackgroundPanel;  // Hierarchy의 BackgroundPanel 할당
    public GameObject OptionsPanel;     // Hierarchy의 OptionsPanel 할당

    [Header("Audio")]
    public AudioMixer masterMixer;

    public void OnVolumeChange(float sliderValue)
    {
        // 로그 스케일(데시벨)로 변환해 자연스런 볼륨 조절
        float dB = Mathf.Log10(sliderValue) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }

    // START 버튼에 연결
    public void OnStartGame()
    {
        SceneManager.LoadScene("GameScene");  // Build Settings에 등록된 씬 이름 확인
    }

    // OPTIONS 버튼에 연결
    public void OnOptions()
    {
        BackgroundPanel.SetActive(true);
        OptionsPanel.SetActive(true);
    }
    // BACK 버튼에 연결
    public void OnBackFromOptions()
    {
        OptionsPanel.SetActive(false);
        BackgroundPanel.SetActive(false);
    }

    // QUIT 버튼에 연결
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
