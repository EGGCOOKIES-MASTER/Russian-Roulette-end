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
        ShowMessage("게임 시작! 당신의 턴입니다.", 2f);
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
        Debug.Log("총알을 재장전했습니다!");
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
            string message = $"[턴 {turnCount}] 플레이어가 자기 자신에게 발사!";
            if (bullet == "fakeBullet")
            {
                message += "\n공포탄입니다! 당신의 턴이 유지됩니다.";
                ShowMessage(message, 2f);
                return;
            }
            else
            {
                playerHP--;
                message += $"\n실탄입니다! 당신의 체력이 {playerHP}로 감소했습니다.";
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
            string message = $"[턴 {turnCount}] 플레이어가 적에게 발사!";
            if (bullet == "fakeBullet")
            {
                message += "\n공포탄입니다! 적의 턴으로 넘어갑니다.";
                ShowMessage(message, 2f);
                isPlayerTurn = false;
                EnablePlayerInput(false);
                Invoke(nameof(EnermyTurn), 2f);
            }
            else
            {
                enermyHP--;
                message += $"\n실탄입니다! 적의 체력이 {enermyHP}로 감소했습니다.";
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
            message = $"[턴 {turnCount}] 에너미가 자신에게 발사!";
            if (bullet == "fakeBullet")
            {
                message += "\n공포탄입니다! 에너미 턴 유지.";
                ShowMessage(message, 2f);
                Invoke(nameof(EnermyTurn), 2f);
                return;
            }
            else
            {
                enermyHP--;
                message += $"\n실탄입니다! 적의 체력이 {enermyHP}로 감소했습니다.";
                ShowMessage(message, 2f);
                CheckGameOver();
                if (!isGameOver)
                    Invoke(nameof(SetPlayerTurn), 2f);
            }
        }
        else
        {
            message = $"[턴 {turnCount}] 에너미가 플레이어에게 발사!";
            if (bullet == "fakeBullet")
            {
                message += "\n공포탄입니다! 당신의 턴입니다.";
                ShowMessage(message, 2f);
                Invoke(nameof(SetPlayerTurn), 2f);
            }
            else
            {
                playerHP--;
                message += $"\n실탄입니다! 당신의 체력이 {playerHP}로 감소했습니다.";
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
        ShowMessage("당신의 턴입니다. 누구를 겨누시겠습니까?", 2f);
        EnablePlayerInput(true);
        UpdateUI();
    }

    private void CheckGameOver()
    {
        if (playerHP <= 0)
        {
            ShowMessage($"게임 오버! 당신은 패배했습니다. 총 턴 수: {turnCount}", 3f);
            isGameOver = true;
        }
        else if (enermyHP <= 0)
        {
            ShowMessage($"게임 클리어! 당신의 승리입니다! 총 턴 수: {turnCount}", 3f);
            isGameOver = true;
        }

        if (isGameOver)
            EnablePlayerInput(false);

        UpdateUI();
    }

    private void UpdateUI()
    {
        playerHPText.text = $"플레이어 HP: {playerHP}";
        enemyHPText.text = $"적 HP: {enermyHP}";
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
