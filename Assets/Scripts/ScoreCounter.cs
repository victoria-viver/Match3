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
	private int targetScore = 0;

    private int animLengthInFrames = 20;
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
            currentScore += (targetScore - currentScore) * currentFrame / animLengthInFrames;
            currentFrame = (currentFrame == animLengthInFrames) ? 0 : currentFrame;

            score.text = currentScore.ToString();
        }
    }
    #endregion


    #region Public Methods
    public void UpdateScore (int newPoints)
    {
        targetScore += newPoints;
    }
    #endregion
}
