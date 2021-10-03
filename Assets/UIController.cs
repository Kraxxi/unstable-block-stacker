using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public float currentHeight;
    public float lastHeight;
    public float targetHeight;
    
    public float smoothDuration;
    public float smoothProgress;
    public float endSmoothTime;
    
    public float diff;
    public float diffProgress;

    public TextMeshProUGUI scoreText;
    public AnimationCurve smoothCurve;

    public float highScore;

    public TextMeshProUGUI ingameHighScoreDisplay;
    public Canvas ingameScoreCanvas;
    private void Start()
    {
        highScore = PlayerPrefs.GetFloat("Highscore");

        if (highScore != 0)
        {
            ingameHighScoreDisplay.text = $"{highScore:0.00}m ->";
            ingameScoreCanvas.transform.position = new Vector3(ingameScoreCanvas.transform.position.x, highScore,
                ingameScoreCanvas.transform.position.z);
        }

    }

    private void Update()
    {
        smoothProgress = smoothCurve.Evaluate(1 - (endSmoothTime - Time.time));
        diffProgress = diff * smoothProgress;
        currentHeight = lastHeight + diffProgress;
        scoreText.text = $"Height: {currentHeight:0.00}m";

        if (currentHeight > highScore)
        {
            ingameHighScoreDisplay.text = $"{currentHeight:0.00}m ->";
            ingameScoreCanvas.transform.position = new Vector3(ingameScoreCanvas.transform.position.x, currentHeight + 0.5f,
                ingameScoreCanvas.transform.position.z);
        }

    }

    public void UpdateHeight(float newHeight)
    {
        if (newHeight > highScore)
        {
            PlayerPrefs.SetFloat("Highscore", newHeight);
        }
        
        targetHeight = newHeight;
        smoothProgress = 0f;
        endSmoothTime = Time.time + smoothDuration;
        diff = targetHeight - currentHeight;
        lastHeight = currentHeight;
    }

    
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
