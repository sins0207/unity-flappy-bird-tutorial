using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameManagerPlayTests
{
    const string MainSceneName = "Flappy Bird";

    GameManager gameManager;
    Player player;

    [UnitySetUp]
    public IEnumerator UnitySetUp()
    {
        Debug.Log($"-- Test UnitySetUp START: Loading scene '{MainSceneName}' ----");

        AsyncOperation sceneLoadOperation = SceneManager.LoadSceneAsync(MainSceneName, LoadSceneMode.Single);

        if (sceneLoadOperation == null)
        {
            Assert.Fail($"Failed to start loading scene '{MainSceneName}'. Check scene name and build settings.");
            yield break;
        }

        while (!sceneLoadOperation.isDone)
        {
            yield return null;
        }

        gameManager = GameManager.Instance;
        Assert.IsNotNull(gameManager, $"GameManager.Instance is null after loading scene '{MainSceneName}' and waiting.");

        player = gameManager.player;
        Assert.IsNotNull(player, "Player object/component not found in the scene after setup.");

        Debug.Log("--- Test UnitySetUp END: Setup complete");
    }

    [UnityTearDown]
    public IEnumerator UnityTearDown()
    {
        Debug.Log("-- Test UnityTearDown START");

        Time.timeScale = 1.0f;
        gameManager = null;
        player = null;

        Debug.Log("Test UnityTearDown END");
        yield return null;
    }

    [UnityTest]
    public IEnumerator GameStartsOnPlayButtonClick()
    {
        Assert.IsNotNull(gameManager, "GameManager reference is null at test start.");
        Assert.IsNotNull(player, "Player reference is null at test start.");
        Assert.IsNotNull(gameManager.playButton, "Play button reference is null.");

        gameManager.Play();

        yield return null;

        Assert.AreEqual(1f, Time.timeScale, "Game should resume (Time.timeScale = 1) after clicking Play.");
        Assert.IsTrue(player.enabled, "Player should be enabled after starting the game.");
        Assert.IsFalse(gameManager.playButton.activeSelf, "Play button should be hidden after game starts.");
    }

    [UnityTest]
    public IEnumerator ScoreIncreasesDuringGameplay()
    {
        Assert.IsNotNull(gameManager, "GameManager reference is null at test start.");
        Assert.IsNotNull(player, "Player reference is null at test start.");
        Assert.IsNotNull(gameManager.scoreText, "ScoreText reference is null.");

   
        gameManager.Play();
        yield return null;

       
        int initialScore = gameManager.score;
        string initialScoreText = gameManager.scoreText.text;

       
        gameManager.IncreaseScore();
        yield return null;

     
        Assert.AreEqual(initialScore + 1, gameManager.score, "Score should increase by 1 after calling IncreaseScore.");
        Assert.AreEqual((initialScore + 1).ToString(), gameManager.scoreText.text, "Score text should reflect the updated score.");
    }

    [UnityTest]
    public IEnumerator ObstacleCollisionTriggersGameOver()
    {
        Assert.IsNotNull(gameManager, "GameManager reference is null at test start.");
        Assert.IsNotNull(player, "Player reference is null at test start.");
        Assert.IsNotNull(gameManager.playButton, "PlayButton reference is null.");
        Assert.IsNotNull(gameManager.gameOver, "GameOver UI reference is null.");

        
        gameManager.Play();
        yield return null;

    
        Assert.AreEqual(1f, Time.timeScale, "Game should be running before collision.");
        Assert.IsTrue(player.enabled, "Player should be enabled before collision.");

        
        GameObject obstacle = new GameObject("Obstacle");
        obstacle.tag = "Obstacle";
        BoxCollider2D collider = obstacle.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;

        player.GetType()
            .GetMethod("OnTriggerEnter2D", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
            ?.Invoke(player, new object[] { collider });

        yield return null;
        Assert.AreEqual(0f, Time.timeScale, "Time.timeScale should be 0 after game over.");
        Assert.IsFalse(player.enabled, "Player should be disabled after game over.");
        Assert.IsTrue(gameManager.playButton.activeSelf, "Play button should be visible after game over.");
        Assert.IsTrue(gameManager.gameOver.activeSelf, "GameOver UI should be visible after game over.");
        Object.Destroy(obstacle);
    }
}