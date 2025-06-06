using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RussianRouletteGame : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public Button shootSelfButton;
    public Button shootEnemyButton;

    public int playerHP = 5;
    public int enermyHP = 5;
    public int turnCount = 0;

    private List<string> bulletChamber = new List<string>();
    private int currentBulletIndex = 0;
    private bool isPlayerTurn = true;
    private bool isGameOver = false;

    private void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        playerHP = 5;
        enermyHP = 5;
        turnCount = 0;
        LoadBullets();
        isPlayerTurn = true;
        isGameOver = false;

        UpdateUI();
        ShowMessage("���� ����! ����� ���Դϴ�.", 2f);
        EnablePlayerInput(true);
    }

    private void LoadBullets()
    {
        bulletChamber.Clear();
        bulletChamber.Add("realBullet");
        for (int i = 0; i < 5; i++)
        {
            bulletChamber.Add("fakeBullet");
        }

        for (int i = 0; i < bulletChamber.Count; i++)
        {
            string temp = bulletChamber[i];
            int randomIndex = Random.Range(i, bulletChamber.Count);
            bulletChamber[i] = bulletChamber[randomIndex];
            bulletChamber[randomIndex] = temp;
        }

        currentBulletIndex = 0;
        Debug.Log("�Ѿ��� �������߽��ϴ�!");
    }

    public void FireAt(bool shootSelf)
    {
        if (isGameOver || !isPlayerTurn) return;

        if (currentBulletIndex >= bulletChamber.Count)
            LoadBullets();

        string bullet = bulletChamber[currentBulletIndex];
        currentBulletIndex++;

        turnCount++;

        if (shootSelf)
        {
            string message = $"[�� {turnCount}] �÷��̾ �ڱ� �ڽſ��� �߻�!";
            if (bullet == "fakeBullet")
            {
                message += "\n����ź�Դϴ�! ����� ���� �����˴ϴ�.";
                ShowMessage(message, 2f);
                return;
            }
            else
            {
                playerHP--;
                message += $"\n��ź�Դϴ�! ����� ü���� {playerHP}�� �����߽��ϴ�.";
                ShowMessage(message, 2f);
                CheckGameOver();
                if (!isGameOver)
                {
                    isPlayerTurn = false;
                    EnablePlayerInput(false);
                    Invoke(nameof(EnermyTurn), 2f);
                }
            }
        }
        else
        {
            string message = $"[�� {turnCount}] �÷��̾ ������ �߻�!";
            if (bullet == "fakeBullet")
            {
                message += "\n����ź�Դϴ�! ���� ������ �Ѿ�ϴ�.";
                ShowMessage(message, 2f);
                isPlayerTurn = false;
                EnablePlayerInput(false);
                Invoke(nameof(EnermyTurn), 2f);
            }
            else
            {
                enermyHP--;
                message += $"\n��ź�Դϴ�! ���� ü���� {enermyHP}�� �����߽��ϴ�.";
                ShowMessage(message, 2f);
                CheckGameOver();
                if (!isGameOver)
                {
                    isPlayerTurn = false;
                    EnablePlayerInput(false);
                    Invoke(nameof(EnermyTurn), 2f);
                }
            }
        }

        UpdateUI();
    }

    private void EnermyTurn()
    {
        if (isGameOver) return;

        if (currentBulletIndex >= bulletChamber.Count)
            LoadBullets();

        int remainingRealBullets = 0;
        for (int i = currentBulletIndex; i < bulletChamber.Count; i++)
        {
            if (bulletChamber[i] == "realBullet")
                remainingRealBullets++;
        }

        float chance = (float)remainingRealBullets / (bulletChamber.Count - currentBulletIndex);
        bool shootSelf = chance < 0.6f;

        string bullet = bulletChamber[currentBulletIndex];
        currentBulletIndex++;
        turnCount++;

        string message = "";

        if (shootSelf)
        {
            message = $"[�� {turnCount}] ���ʹ̰� �ڽſ��� �߻�!";
            if (bullet == "fakeBullet")
            {
                message += "\n����ź�Դϴ�! ���ʹ� �� ����.";
                ShowMessage(message, 2f);
                Invoke(nameof(EnermyTurn), 2f);
                return;
            }
            else
            {
                enermyHP--;
                message += $"\n��ź�Դϴ�! ���� ü���� {enermyHP}�� �����߽��ϴ�.";
                ShowMessage(message, 2f);
                CheckGameOver();
                if (!isGameOver)
                    Invoke(nameof(SetPlayerTurn), 2f);
            }
        }
        else
        {
            message = $"[�� {turnCount}] ���ʹ̰� �÷��̾�� �߻�!";
            if (bullet == "fakeBullet")
            {
                message += "\n����ź�Դϴ�! ����� ���Դϴ�.";
                ShowMessage(message, 2f);
                Invoke(nameof(SetPlayerTurn), 2f);
            }
            else
            {
                playerHP--;
                message += $"\n��ź�Դϴ�! ����� ü���� {playerHP}�� �����߽��ϴ�.";
                ShowMessage(message, 2f);
                CheckGameOver();
                if (!isGameOver)
                    Invoke(nameof(SetPlayerTurn), 2f);
            }
        }

        UpdateUI();
    }

    private void SetPlayerTurn()
    {
        isPlayerTurn = true;
        ShowMessage("����� ���Դϴ�. ������ �ܴ��ðڽ��ϱ�?", 2f);
        EnablePlayerInput(true);
        UpdateUI();
    }

    private void CheckGameOver()
    {
        if (playerHP <= 0)
        {
            ShowMessage($"���� ����! ����� �й��߽��ϴ�. �� �� ��: {turnCount}", 3f);
            isGameOver = true;
        }
        else if (enermyHP <= 0)
        {
            ShowMessage($"���� Ŭ����! ����� �¸��Դϴ�! �� �� ��: {turnCount}", 3f);
            isGameOver = true;
        }

        if (isGameOver)
            EnablePlayerInput(false);

        UpdateUI();
    }

    private void UpdateUI()
    {
        playerHPText.text = $"�÷��̾� HP: {playerHP}";
        enemyHPText.text = $"�� HP: {enermyHP}";
    }

    private void EnablePlayerInput(bool enable)
    {
        shootSelfButton.interactable = enable;
        shootEnemyButton.interactable = enable;
    }

    private void ShowMessage(string message, float duration)
    {
        infoText.text = message;
        CancelInvoke(nameof(ClearInfo));
        Invoke(nameof(ClearInfo), duration);
    }

    private void ClearInfo()
    {
        infoText.text = "";
    }
}
