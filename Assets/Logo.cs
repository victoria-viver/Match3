/**
 * Created by: Victoria Shenkevich
 * Created on: 29/12/2019
 */

using System.Collections;
using UnityEngine;

public class Logo : MonoBehaviour
{
    #region Constants
    private const string IDLE_ANIMATION = "PlayIdleAnimation";
    private const float PARTS_DELAY = 0.1f;
    private const int ANIMATION_DELAY = 5;
    #endregion


    #region Private Fields
    private Animator animator;
    #endregion


    #region Links
    [SerializeField] private Animator[] parts;
    #endregion


    #region Unity Methods
    void Start()
    {
        InvokeRepeating(IDLE_ANIMATION, 0, parts.Length * PARTS_DELAY * ANIMATION_DELAY);
    }

    private void PlayIdleAnimation()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            StartCoroutine(PlayAnimationCo(parts[i], "Logo", i * PARTS_DELAY));
        }        
    }
    #endregion


    #region Private Methods
    IEnumerator PlayAnimationCo (Animator animator, string animatonName, float delay)
    {   
        yield return new WaitForSeconds(delay);

        animator.PlayInFixedTime(animatonName, 0);
    }
    #endregion
}
