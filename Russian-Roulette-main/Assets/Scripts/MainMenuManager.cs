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
    public GameObject BackgroundPanel;  // �ɼ� ���
    public GameObject OptionsPanel;     // �ɼ� �޴�

    [Header("Audio")]
    public AudioMixer masterMixer;

    // �� GameScene���� UI�� �ѱ� ���� ���� ����
    public static bool gameStartedFromMainMenu = false;

    // �� Start ��ư Ŭ�� �� ȣ��
    public void OnStartGame()
    {
        gameStartedFromMainMenu = true;
        SceneManager.LoadScene("GameScene");
    }

    // �� �ɼ� ��ư Ŭ�� �� ȣ��
    public void OnOptions()
    {
        BackgroundPanel.SetActive(true);
        OptionsPanel.SetActive(true);
    }

    // �� �ɼ� �� �ڷΰ��� ��ư Ŭ�� �� ȣ��
    public void OnBackFromOptions()
    {
        OptionsPanel.SetActive(false);
        BackgroundPanel.SetActive(false);
    }

    // �� ���� ���� �����̴� ���� �� ȣ��
    public void OnVolumeChange(float sliderValue)
    {
        float dB = Mathf.Log10(sliderValue) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }

    // �� ���� ��ư Ŭ�� �� ȣ��
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
