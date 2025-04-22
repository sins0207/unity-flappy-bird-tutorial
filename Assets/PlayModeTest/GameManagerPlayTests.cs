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
    public IEnumerator GameStartsPausedAndPlayerDisabled()
    {
        Assert.IsNotNull(gameManager, "GameManager reference is null at test start.");
        Assert.IsNotNull(player, "Player reference is null at test start.");
        Assert.IsNotNull(gameManager.playButton, "GameManager.playButton reference is null.");

        Assert.AreEqual(0f, Time.timeScale, "Game should start with Time.timeScale = 0.");
        Assert.IsFalse(player.enabled, "Player should be disabled initially.");
        Assert.IsTrue(gameManager.playButton.activeSelf, "Play button should be active initially.");

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
}
