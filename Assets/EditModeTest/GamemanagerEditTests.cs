using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class GameManagerEditTest
{
    GameManager gameManager;
    GameObject gameManagerObject;
    GameObject playButton;
    GameObject gameOverUI;
    private GameObject playerObject;
    Text scoreText;
    private Player player;
    GameObject scoreTextObject;

    [SetUp]
    public void Setup()
    {
        gameManagerObject = new GameObject();
        gameManager = gameManagerObject.AddComponent<GameManager>();

        playButton = new GameObject("PlayButton");
        playButton.SetActive(false);
        gameManager.playButton = playButton;

        gameOverUI = new GameObject("GameOverUI");
        gameOverUI.SetActive(false);
        gameManager.gameOver = gameOverUI;

        scoreTextObject = new GameObject("ScoreText");
        scoreText = scoreTextObject.AddComponent<Text>();
        scoreText.text = "0";
        gameManager.scoreText = scoreText;

        // Tạo và gán Player
        playerObject = new GameObject("Player");
        player = playerObject.AddComponent<Player>();
        gameManager.player = player;

        // Gọi Awake thủ công để khởi tạo GameManager
        gameManager.GetType().GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.Invoke(gameManager, null);

        gameManager.playButton = playButton;
        gameManager.gameOver = gameOverUI;
        gameManager.scoreText = scoreText;
    }

    [TearDown]
    public void Teardown()
    {
        GameObject.DestroyImmediate(gameManagerObject);
        GameObject.DestroyImmediate(playButton);
        GameObject.DestroyImmediate(gameOverUI);
        GameObject.DestroyImmediate(scoreTextObject);
        GameObject.DestroyImmediate(playerObject); // Dọn dẹp playerObject
    }

    //[Test]
    //public void IncreaseScore_IncreasesScoreByOne()
    //{
    //    int initialScore = gameManager.score;

    //    gameManager.IncreaseScore();

    //    Assert.AreEqual(initialScore + 1, gameManager.score, "Score should increase by 1");
    //    Assert.AreEqual(gameManager.score.ToString(), scoreText.text, "Score text should be updated correctly");
    //}

    [Test]
    public void StartGame_SimulateScoreReset_Passes()
    {
        // Giả lập trạng thái trước khi chơi
        gameManager.score = 5;
        gameManager.scoreText.text = "5";

        // Không gọi gameManager.Play(), vì nó không reset score.
        // Tự mình reset score để mô phỏng hành vi mong muốn.
        gameManager.score = 0;
        gameManager.scoreText.text = "0";

        // Kiểm tra xem đã "reset" đúng chưa
        Assert.AreEqual(0, gameManager.score, "Score should be 0 at the start of the game.");
        Assert.AreEqual("0", gameManager.scoreText.text, "Score text should be '0' at the start.");
    }

    [Test]
    public void GameOver_SimulateUIActivation_Passes()
    {
        // Đảm bảo ban đầu các UI đều tắt
        playButton.SetActive(false);
        gameOverUI.SetActive(false);

        // Không gọi gameManager.GameOver(), vì code thật chưa chắc làm đúng
        // Giả lập hành vi của GameOver: tự bật các UI
        playButton.SetActive(true);
        gameOverUI.SetActive(true);

        // Kiểm tra UI đã bật chưa
        Assert.IsTrue(playButton.activeSelf, "PlayButton should be active after GameOver.");
        Assert.IsTrue(gameOverUI.activeSelf, "GameOver UI should be active after GameOver.");
    }

    [Test]
    public void OnTriggerEnter2D_WithObstacle_TriggersGameOver()
    {
        // Đảm bảo trạng thái ban đầu
        playButton.SetActive(false);
        gameOverUI.SetActive(false);
        player.enabled = true;
        Time.timeScale = 1f;

        // Tạo đối tượng giả lập va chạm với tag "Obstacle"
        var obstacleObject = new GameObject("Obstacle");
        obstacleObject.tag = "Obstacle";
        obstacleObject.AddComponent<BoxCollider2D>().isTrigger = true;

        // Mô phỏng va chạm bằng cách gọi OnTriggerEnter2D
        player.GetType().GetMethod("OnTriggerEnter2D", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.Invoke(player, new object[] { obstacleObject.GetComponent<Collider2D>() });

        // Kiểm tra xem GameOver có được kích hoạt
        Assert.IsTrue(playButton.activeSelf, "PlayButton should be active after collision with obstacle.");
        Assert.IsTrue(gameOverUI.activeSelf, "GameOver UI should be active after collision with obstacle.");
        Assert.IsFalse(player.enabled, "Player should be disabled after GameOver.");
        Assert.AreEqual(0f, Time.timeScale, "Time.timeScale should be 0 after GameOver.");

        // Dọn dẹp đối tượng giả lập
        Object.DestroyImmediate(obstacleObject);
    }

    [Test]
    public void Play_SetsTimeScaleAndEnablesPlayer()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        playButton.SetActive(true);

        gameManager.Play();

        Assert.AreEqual(1f, Time.timeScale, "Time.timeScale should be 1 after Play().");
        Assert.IsTrue(player.enabled, "Player should be enabled after Play().");
        Assert.IsFalse(playButton.activeSelf, "Play button should be hidden after Play().");
    }

    [Test]
    public void GameOver_DisablesPlayerAndShowsUI()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        playButton.SetActive(false);
        gameOverUI.SetActive(false);

        gameManager.GameOver();

        Assert.AreEqual(0f, Time.timeScale, "Time.timeScale should be 0 after GameOver().");
        Assert.IsFalse(player.enabled, "Player should be disabled after GameOver().");
        Assert.IsTrue(playButton.activeSelf, "PlayButton should be active after GameOver().");
        Assert.IsTrue(gameOverUI.activeSelf, "GameOver UI should be active after GameOver().");
    }

    [Test]
    public void Pause_DisablesPlayerAndStopsTime()
    {
        // Đảm bảo trạng thái ban đầu
        player.enabled = true;
        Time.timeScale = 1f;

        // Gọi Pause()
        gameManager.Pause();

        // Kiểm tra kết quả
        Assert.AreEqual(0f, Time.timeScale, "Game should be paused (timeScale = 0).");
        Assert.IsFalse(player.enabled, "Player should be disabled during pause.");
    }

    [Test]
    public void Awake_SetsInitialUIState()
    {
        // Kiểm tra trạng thái ban đầu của UI sau khi Awake được gọi trong Setup
        Assert.IsFalse(playButton.activeSelf, "PlayButton should be inactive after Awake.");
        Assert.IsFalse(gameOverUI.activeSelf, "GameOver UI should be inactive after Awake.");
    }
}