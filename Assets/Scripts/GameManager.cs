/**
* Created by: Victoria Shenkevich
* Created on: 15/12/2019
*/

using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Links
    [SerializeField] private ScoreCounter score;
    [SerializeField] private BoardController board;
    [SerializeField] private GameObject screenBlocker;
    #endregion


	#region Singleton
    private static GameManager instance = null;

    internal static GameManager Instance 
	{
		get 
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<GameManager>();
				
				if (instance == null)
				{
					instance = new GameObject ("GameManager").AddComponent<GameManager>();
				}
			}
			
			return instance;
		}
   	}
    #endregion Singleton


	#region Unity Methods
	void Awake () 
	{
		//Singleton
		if (instance)
			DestroyImmediate(gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		//
	}

	void Start() 
	{
		BlockScreen (false);
		board.Init();	
	}
	#endregion


    #region Public Methods
    public void UpdateScore(int points)
    {
        score.AddPoints(points);
    }

	public void BlockScreen (bool isBlocked)
	{
		screenBlocker.SetActive (isBlocked);
	}
    #endregion
}
