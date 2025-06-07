// RussianRouletteGame.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RussianRouletteGame : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public Button shootSelfButton;
    public Button shootEnemyButton;
    public UnityEngine.UI.Image damageOverlay;

    [Header("Audio")]
    public AudioSource gunshotAudio;
    public AudioSource dryfireAudio;
    public AudioSource hurtAudio;
    public AudioSource winAudio;
    public AudioSource loseAudio;

    public int playerHP = 5;
    public int enermyHP = 5;
    public int turnCount = 0;

    private List<string> bulletChamber = new List<string>();
    private int currentBulletIndex = 0;
    private bool isPlayerTurn = true;
    private bool isGameOver = false;

    private Coroutine damageFlashCoroutine;
    private int selfShootStreak = 0;
    private int enemySelfShootStreak = 0;

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
        selfShootStreak = 0;
        enemySelfShootStreak = 0;

        UpdateUI();
        ShowMessage("게임 시작! 당신의 턴입니다.", 2f);
        EnablePlayerInput(true);
    }

    private void LoadBullets()
    {
        bulletChamber.Clear();
        bulletChamber.Add("realBullet");
        for (int i = 0; i < 5; i++) bulletChamber.Add("fakeBullet");

        for (int i = 0; i < bulletChamber.Count; i++)
        {
            string temp = bulletChamber[i];
            int randomIndex = UnityEngine.Random.Range(i, bulletChamber.Count);
            bulletChamber[i] = bulletChamber[randomIndex];
            bulletChamber[randomIndex] = temp;
        }

        currentBulletIndex = 0;
    }

    public void FireAt(bool shootSelf)
    {
        if (isGameOver || !isPlayerTurn) return;
        if (currentBulletIndex >= bulletChamber.Count) LoadBullets();

        string bullet = bulletChamber[currentBulletIndex];
        currentBulletIndex++;
        turnCount++;

        string message = shootSelf ? $"[턴 {turnCount}] 플레이어가 자기 자신에게 발사!" : $"[턴 {turnCount}] 플레이어가 적에게 발사!";

        if (shootSelf)
        {
            if (bullet == "fakeBullet")
            {
                dryfireAudio?.Play();
                selfShootStreak++;

                if (selfShootStreak == 3)
                    message += "\n⚠️ 경고: 다음 자기공격 시 턴이 넘어갑니다!";
                else if (selfShootStreak >= 4)
                {
                    message += "\n4회 연속 자기공격으로 인해 턴이 넘어갑니다!";
                    ShowMessage(message, 2f);
                    selfShootStreak = 0;
                    isPlayerTurn = false;
                    EnablePlayerInput(false);
                    Invoke(nameof(EnermyTurn), 2f);
                    UpdateUI();
                    return;
                }

                message += "\n공포탄입니다! 당신의 턴이 유지됩니다.";
                ShowMessage(message, 2f);
                return;
            }
            else
            {
                gunshotAudio?.Play();
                playerHP--;

                if (damageFlashCoroutine != null) StopCoroutine(damageFlashCoroutine);
                damageFlashCoroutine = StartCoroutine(FlashDamageOverlay());

                if (!CheckGameOver())
                {
                    selfShootStreak = 0;
                    hurtAudio?.Play();
                    message += $"\n실탄입니다! 당신의 체력이 {playerHP}로 감소했습니다.";
                    ShowMessage(message, 2f);
                    isPlayerTurn = false;
                    EnablePlayerInput(false);
                    Invoke(nameof(EnermyTurn), 2f);
                }
                else
                {
                    message += "\n실탄입니다! 당신의 체력이 0이 되었습니다.";
                    ShowMessage(message, 2f);
                }
            }
        }
        else
        {
            selfShootStreak = 0;
            if (bullet == "fakeBullet")
            {
                dryfireAudio?.Play();
                message += "\n공포탄입니다! 적의 턴으로 넘어갑니다.";
                ShowMessage(message, 2f);
                isPlayerTurn = false;
                EnablePlayerInput(false);
                Invoke(nameof(EnermyTurn), 2f);
            }
            else
            {
                gunshotAudio?.Play();
                enermyHP--;
                if (!CheckGameOver())
                {
                    hurtAudio?.Play();
                    message += $"\n실탄입니다! 적의 체력이 {enermyHP}로 감소했습니다.";
                    ShowMessage(message, 2f);
                    isPlayerTurn = false;
                    EnablePlayerInput(false);
                    Invoke(nameof(EnermyTurn), 2f);
                }
                else
                {
                    message += "\n실탄입니다! 적의 체력이 0이 되었습니다.";
                    ShowMessage(message, 2f);
                }
            }
        }
        UpdateUI();
    }

    private void EnermyTurn()
    {
        if (isGameOver) return;
        if (currentBulletIndex >= bulletChamber.Count) LoadBullets();

        int remainingRealBullets = 0;
        for (int i = currentBulletIndex; i < bulletChamber.Count; i++)
            if (bulletChamber[i] == "realBullet") remainingRealBullets++;

        float chance = (float)remainingRealBullets / (bulletChamber.Count - currentBulletIndex);
        float healthFactor = (float)enermyHP / 5f;
        bool shootSelf = (enermyHP > 2 && chance * healthFactor < 0.3f);

        string bullet = bulletChamber[currentBulletIndex];
        currentBulletIndex++;
        turnCount++;
        string message = shootSelf ? $"[턴 {turnCount}] 에너미가 자신에게 발사!" : $"[턴 {turnCount}] 에너미가 플레이어에게 발사!";

        if (shootSelf)
        {
            if (bullet == "fakeBullet")
            {
                dryfireAudio?.Play();
                enemySelfShootStreak++;

                if (enemySelfShootStreak == 3)
                    message += "\n⚠️ 에너미 경고: 다음 자기공격 시 턴이 넘어갑니다!";
                else if (enemySelfShootStreak >= 4)
                {
                    message += "\n에너미가 4회 연속 자기공격하여 턴이 넘어갑니다!";
                    ShowMessage(message, 2f);
                    enemySelfShootStreak = 0;
                    Invoke(nameof(SetPlayerTurn), 2f);
                    UpdateUI();
                    return;
                }

                message += "\n공포탄입니다! 에너미 턴 유지.";
                ShowMessage(message, 2f);
                Invoke(nameof(EnermyTurn), 2f);
                return;
            }
            else
            {
                gunshotAudio?.Play();
                enermyHP--;
                enemySelfShootStreak = 0;
                if (!CheckGameOver())
                {
                    hurtAudio?.Play();
                    message += $"\n실탄입니다! 적의 체력이 {enermyHP}로 감소했습니다.";
                    ShowMessage(message, 2f);
                    Invoke(nameof(SetPlayerTurn), 2f);
                }
                else
                {
                    message += "\n실탄입니다! 적의 체력이 0이 되었습니다.";
                    ShowMessage(message, 2f);
                }
            }
        }
        else
        {
            enemySelfShootStreak = 0;
            if (bullet == "fakeBullet")
            {
                dryfireAudio?.Play();
                message += "\n공포탄입니다! 당신의 턴입니다.";
                ShowMessage(message, 2f);
                Invoke(nameof(SetPlayerTurn), 2f);
            }
            else
            {
                gunshotAudio?.Play();
                playerHP--;

                if (damageFlashCoroutine != null) StopCoroutine(damageFlashCoroutine);
                damageFlashCoroutine = StartCoroutine(FlashDamageOverlay());

                if (!CheckGameOver())
                {
                    hurtAudio?.Play();
                    message += $"\n실탄입니다! 당신의 체력이 {playerHP}로 감소했습니다.";
                    ShowMessage(message, 2f);
                    Invoke(nameof(SetPlayerTurn), 2f);
                }
                else
                {
                    message += "\n실탄입니다! 당신의 체력이 0이 되었습니다.";
                    ShowMessage(message, 2f);
                }
            }
        }
        UpdateUI();
    }

    private IEnumerator FlashDamageOverlay()
    {
        if (damageOverlay == null) yield break;
        damageOverlay.color = new Color(1f, 0f, 0f, 0.5f);
        yield return new WaitForSeconds(0.2f);
        damageOverlay.color = new Color(1f, 0f, 0f, 0f);
    }

    private void SetPlayerTurn()
    {
        isPlayerTurn = true;
        ShowMessage("당신의 턴입니다. 누구를 겨누시겠습니까?", 2f);
        EnablePlayerInput(true);
        UpdateUI();
    }

    private bool CheckGameOver()
    {
        if (playerHP <= 0)
        {
            if (!isGameOver) loseAudio?.Play();
            ShowMessage($"게임 오버! 당신은 패배했습니다. 총 턴 수: {turnCount}", 3f);
            isGameOver = true;
            EnablePlayerInput(false);
            UpdateUI();
            return true;
        }
        else if (enermyHP <= 0)
        {
            if (!isGameOver) winAudio?.Play();
            ShowMessage($"게임 클리어! 당신의 승리입니다! 총 턴 수: {turnCount}", 3f);
            isGameOver = true;
            EnablePlayerInput(false);
            UpdateUI();
            return true;
        }
        return false;
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