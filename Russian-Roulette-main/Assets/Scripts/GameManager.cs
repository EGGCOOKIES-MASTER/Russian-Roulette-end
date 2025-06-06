using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI infoText;   // 상단 텍스트
    public Button fireButton;          // "방아쇠 당기기" 버튼
    public GameObject gameUI;          // GameUI 전체 묶음 오브젝트

    private int bulletSlot = 6;        // 총 슬롯 수 (예: 6발 중 하나만 장전)
    private int bulletIndex;           // 실제 총알이 들어간 슬롯 인덱스
    private int currentIndex = 0;      // 현재 턴에서 발사 시도할 인덱스
    private bool isPlayerTurn = true;  // 현재 턴이 플레이어인가?

    void Start()
    {
        // Start 버튼에서 진입한 경우에만 UI 표시
        if (MainMenuManager.gameStartedFromMainMenu)
        {
            EnableGameUI();
        }
        else
        {
            if (gameUI != null)
                gameUI.SetActive(false); // 에디터에서 바로 실행하면 UI 숨김
        }

        bulletIndex = UnityEngine.Random.Range(0, bulletSlot);
    }

    public void EnableGameUI()
    {
        if (gameUI != null)
            gameUI.SetActive(true);

        infoText.text = "당신의 턴입니다.";
        fireButton.interactable = true;
    }

    public void Fire()
    {
        bool fired = (currentIndex == bulletIndex);

        if (fired)
        {
            if (isPlayerTurn)
            {
                infoText.text = "당신이 죽었습니다.";
            }
            else
            {
                infoText.text = "NPC가 죽었습니다. 당신의 승리입니다!";
            }

            fireButton.interactable = false;
        }
        else
        {
            currentIndex = (currentIndex + 1) % bulletSlot;
            isPlayerTurn = !isPlayerTurn;

            if (isPlayerTurn)
            {
                infoText.text = "당신의 턴입니다.";
                fireButton.interactable = true;
            }
            else
            {
                infoText.text = "NPC의 턴입니다...";
                fireButton.interactable = false;
                Invoke("NPCTurn", 2f); // NPC는 2초 후 자동 발사
            }
        }
    }

    private void NPCTurn()
    {
        Fire();
    }
}
