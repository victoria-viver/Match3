/**
* Created by: Victoria Shenkevich
* Created on: 15/12/2019
*/

using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour
{
    #region Private Fields
	private int currentScore = 0;
	private int startScore = 0;
	private int targetScore = 0;

    private int animLengthInFrames = 10;
    private int currentFrame = 0;
    #endregion


    #region Links
    [SerializeField] private Text score;
    #endregion


    #region Unity Methods
    void Awake ()
    {
        score = GetComponent<Text>();
    }

    void Start()
    {
        score.text = currentScore.ToString();
    }

    void Update()
    {
        if (currentScore < targetScore)
        {
            currentFrame++;

            currentScore = startScore + (int) ((targetScore - startScore) * (float) currentFrame / (float) animLengthInFrames);

            score.text = currentScore.ToString();
        }
    }
    #endregion


    #region Public Methods
    public void AddPoints (int points)
    {
        startScore = targetScore;

        targetScore += points;

        currentFrame = 0;
    }
    #endregion
}