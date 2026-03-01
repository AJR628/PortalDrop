using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MainSceneTests
{
    [UnityTest]
    public IEnumerator MainScene_HasRequiredObjects_AndSerializedRefs()
    {
        SceneManager.LoadScene("Main");
        yield return null;

        Assert.That(GameObject.Find("Main Camera"), Is.Not.Null);
        Assert.That(GameObject.Find("Arena"), Is.Not.Null);
        Assert.That(GameObject.Find("Ball"), Is.Not.Null);
        Assert.That(GameObject.Find("PortalEntry"), Is.Not.Null);
        Assert.That(GameObject.Find("PortalExit"), Is.Not.Null);
        Assert.That(GameObject.Find("Canvas"), Is.Not.Null);
        Assert.That(GameObject.Find("EventSystem"), Is.Not.Null);
        Assert.That(GameObject.Find("GameManager"), Is.Not.Null);

        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        InputPortalPlacer inputPortalPlacer = GameObject.Find("GameManager").GetComponent<InputPortalPlacer>();
        PortalManager portalManager = GameObject.Find("PortalEntry").GetComponent<PortalManager>();

        Assert.That(gameManager, Is.Not.Null);
        Assert.That(inputPortalPlacer, Is.Not.Null);
        Assert.That(portalManager, Is.Not.Null);

        Assert.That(gameManager.Ball, Is.Not.Null);
        Assert.That(gameManager.PortalEntry, Is.Not.Null);
        Assert.That(gameManager.PortalExit, Is.Not.Null);
        Assert.That(gameManager.StartButton, Is.Not.Null);
        Assert.That(gameManager.ResetButton, Is.Not.Null);

        Assert.That(inputPortalPlacer.ExitPortalTransform, Is.Not.Null);
        Assert.That(inputPortalPlacer.ExitPortalPortal, Is.Not.Null);
        Assert.That(inputPortalPlacer.GameManager, Is.Not.Null);
        Assert.That(inputPortalPlacer.MainCamera, Is.Not.Null);

        Assert.That(portalManager.ExitPortalTransform, Is.Not.Null);
        Assert.That(portalManager.ExitPortalPortal, Is.Not.Null);
        Assert.That(portalManager.BallController, Is.Not.Null);
        Assert.That(portalManager.GameManager, Is.Not.Null);
    }
}
