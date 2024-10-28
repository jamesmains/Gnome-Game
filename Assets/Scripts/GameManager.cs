using System;
using System.Collections;
using ParentHouse.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

// Todo: rework ALL of this nonsense...

public class GameManager : MonoBehaviour {
    public static UnityEvent OnGameStart = new();
    public static UnityEvent OnGameOver = new();
    public static UnityEvent<int> OnGainScore = new();
    public int gameState; // 0 -> Menu, 1 -> Gameplay, 2 -> Paused, 3 -> gameover

    [SerializeField] [FoldoutGroup("Hooks")]
    private GameObject playerObj;

    [SerializeField] private float timeSetSpeed;
    [SerializeField] private float alphaSetSpeed;
    [SerializeField] private CanvasGroup StartGameCanvas;
    [SerializeField] private TextMeshProUGUI ScoreText;
    private int score;

    private void Awake() {
        StartCoroutine(SetCanvasAlpha(StartGameCanvas, 1f));
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) TryStartGame();

        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void OnEnable() {
        OnGameOver.AddListener(EndGame);
        OnGameStart.AddListener(StartGame);
        OnGainScore.AddListener(delegate(int incomingScore) {
            score += incomingScore;
            ScoreText.text = score.ToString();
        });
    }

    private void OnDisable() {
        OnGameOver.RemoveListener(EndGame);
        OnGameStart.RemoveListener(StartGame);
        OnGainScore.RemoveListener(delegate(int incomingScore) {
            score += incomingScore;
            ScoreText.text = score.ToString();
        });
    }

    public void TryStartGame() {
        if (!(gameState == 3 || gameState == 0)) return;
        OnGameStart.Invoke();
    }

    private void StartGame() {
        Pooler.SpawnAt(playerObj, Vector3.zero);
        gameState = 1;
        score = 0;
        ScoreText.text = score.ToString();

        StopAllCoroutines();
        StartCoroutine(SetTimeScale(1f));
        StartCoroutine(SetCanvasAlpha(StartGameCanvas, 0f));
    }

    private void EndGame() {
        gameState = 3;
        StopAllCoroutines();
        StartCoroutine(SetTimeScale(.3f));
        StartCoroutine(SetCanvasAlpha(StartGameCanvas, 1f));
    }

    private void TogglePause() {
        StopCoroutine("SetTimeScale");
        if (gameState == 1)
            PauseGame();
        else if (gameState == 2)
            ResumeGame();
    }

    private void PauseGame() {
        StartCoroutine(SetTimeScale(0f));
    }

    private void ResumeGame() {
        StartCoroutine(SetTimeScale(.3f));
    }

    private IEnumerator SetTimeScale(float targetTime) {
        while (Math.Abs(Time.timeScale - targetTime) > 0.00095f) {
            Time.timeScale = Mathf.Lerp(Time.timeScale, targetTime, Time.unscaledDeltaTime * timeSetSpeed);
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator SetCanvasAlpha(CanvasGroup canvas, float targetAlpha) {
        while (Math.Abs(canvas.alpha - targetAlpha) > 0.00095f) {
            canvas.alpha = Mathf.Lerp(canvas.alpha, targetAlpha, Time.unscaledDeltaTime * alphaSetSpeed);
            yield return new WaitForEndOfFrame();
        }
    }
}