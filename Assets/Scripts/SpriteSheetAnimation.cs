/**
 * Created by: Victoria Shenkevich
 * Created on: 15/12/2019
 */

using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSheetAnimation : MonoBehaviour
{
    #region Private Fields
    [Range(0.1f, 1f)]
	[SerializeField] private float speed = 1;

	[SerializeField] private bool isInLoop = false;
	private bool isPlayed = false;
	private bool isEnded = false;
    
    private int currentFrame = 0;
    private int currentSprite = 0;

    private Image imageComponent;
	#endregion


    #region Links    
    [SerializeField] private Sprite[] sprites;    
    #endregion


    #region Unity Methods
    void Awake ()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.enabled = false;
    }

    void Update()
    {
        imageComponent.enabled = isPlayed;

        if (isPlayed)
        {
            UpdateFrame();
        }
    }
    #endregion
    

    #region Private Methods
    private void UpdateFrame()
    {
        if (isInLoop || (!isInLoop && !isEnded))
        {
            currentFrame++;

            float delta = currentFrame * speed;

            if (delta > currentSprite)
            {
                UpdateSprite();
            }

            if (delta >= sprites.Length)
            {
                currentFrame = 0;
            }
        }
    }
    
    private void UpdateSprite()
    {
        imageComponent.sprite = sprites[currentSprite++];

        if (currentSprite == sprites.Length)
        {
            if (isInLoop)
            {
                currentSprite = 0;
            }
            else
            {
                isEnded = true;
                isPlayed = false;
            }
        }
            
    }
    #endregion


    #region Public Methods
    public void Play ()
    {
        isPlayed = true;
    }
    #endregion
}