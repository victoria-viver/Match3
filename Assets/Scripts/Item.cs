/**
 * Created by: Victoria Shenkevich
 * Created on: 15/12/2019
 */

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    #region Private Fields
    private Image image;
    [SerializeField] private GameObject popAnimation;
    private SpriteSheetAnimation popAnimationScript;
    private Action OnDestroyCallback;
    #endregion


    #region Unity Methods
    void Awake()
    {
        image = GetComponent<Image>();
        popAnimationScript = popAnimation.GetComponent<SpriteSheetAnimation>();
    }

    void Start()
    {
        popAnimationScript.Init(OnPopAnimationEnd);
    }

    void Update()
    {
        
    }

    void OnDestroy()
    {
        OnDestroyCallback();
    }
    #endregion


    #region Public Methods
    public void Init (Action onDestroyCallback)
    {
        OnDestroyCallback = onDestroyCallback;
    }

    public void Pop ()
    {
        popAnimationScript.Play();
        image.enabled = false;
    }

    public void OnPopAnimationEnd()
    {
        Destroy(gameObject);
    }
    #endregion
}