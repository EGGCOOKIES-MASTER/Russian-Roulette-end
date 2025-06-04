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
    public GameObject BackgroundPanel;  // Hierarchy�� BackgroundPanel �Ҵ�
    public GameObject OptionsPanel;     // Hierarchy�� OptionsPanel �Ҵ�

    [Header("Audio")]
    public AudioMixer masterMixer;

    public void OnVolumeChange(float sliderValue)
    {
        // �α� ������(���ú�)�� ��ȯ�� �ڿ����� ���� ����
        float dB = Mathf.Log10(sliderValue) * 20f;
        masterMixer.SetFloat("MasterVolume", dB);
    }

    // START ��ư�� ����
    public void OnStartGame()
    {
        SceneManager.LoadScene("GameScene");  // Build Settings�� ��ϵ� �� �̸� Ȯ��
    }

    // OPTIONS ��ư�� ����
    public void OnOptions()
    {
        BackgroundPanel.SetActive(true);
        OptionsPanel.SetActive(true);
    }
    // BACK ��ư�� ����
    public void OnBackFromOptions()
    {
        OptionsPanel.SetActive(false);
        BackgroundPanel.SetActive(false);
    }

    // QUIT ��ư�� ����
    public void OnQuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
