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
    private int x;
    private int y;
    
    private Image image;
    private RectTransform rectTransform;

    [SerializeField] private GameObject popAnimation;
    private SpriteSheetAnimation popAnimationScript;

    private Action OnDestroyCallback;
    #endregion


    #region Unity Methods
    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        popAnimationScript = popAnimation.GetComponent<SpriteSheetAnimation>();
    }

    void Start()
    {
        float cellSize = Model.ItemSize;
        rectTransform.sizeDelta = new Vector2(cellSize, cellSize);

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


    #region Private Methods
    private void UpdateName ()
    {
        gameObject.name = "[" + x + "," + y + "]";
    }

    private void UpdatePosition ()
    {
        transform.position = new Vector2((x + 0.5f) * Model.CellSize + Model.BORDER_GAP, (y + 0.5f) * Model.CellSize + Model.BORDER_GAP);
    }
    #endregion


    #region Public Methods
    public void Init (int x, int y, Action onDestroyCallback)
    {
        UpdateCoordinates (x, y);

        OnDestroyCallback = onDestroyCallback;
    }

    public void UpdateCoordinates (int x, int y)
    {
        this.x = x;
        this.y = y;

        UpdateName ();
        UpdatePosition();
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