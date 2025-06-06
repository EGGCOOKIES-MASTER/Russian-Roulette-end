#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using static System.Net.Mime.MediaTypeNames;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject BackgroundPanel;  // 옵션 배경
    public GameObject OptionsPanel;     // 옵션 메뉴

    [Header("Audio")]
    public AudioMixer masterMixer;

    // ▶ GameScene에서 UI를 켜기 위한 상태 변수
    public static bool gameStartedFromMainMenu = false;

    // ▶ Start 버튼 클릭 시 호출
    public void OnStartGame()
    {
        gameStartedFromMainMenu = true;
        SceneManager.LoadScene("GameScene");
    }

    // ▶ 옵션 버튼 클릭 시 호출
    public void OnOptions()
    {
        BackgroundPanel.SetActive(true);
        OptionsPanel.SetActive(true);
    }

    // ▶ 옵션 → 뒤로가기 버튼 클릭 시 호출
    public void OnBackFromOptions()
    {
        OptionsPanel.SetActive(false);
        BackgroundPanel.SetActive(false);
    }

    // ▶ 볼륨 조절 슬라이더 변경 시 호출
    public void OnVolumeChange(float sliderValue)
    {
        float dB = Mathf.Log10(sliderValue) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }

    // ▶ 종료 버튼 클릭 시 호출
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
