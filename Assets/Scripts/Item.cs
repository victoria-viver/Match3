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
    private Vector2 coordinates;
    
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
        gameObject.name = "[" + coordinates.x + "," + coordinates.y + "]";
    }

    private void UpdatePosition ()
    {
        //0.5f is added because items pivot is in the center
        transform.position = new Vector2((coordinates.x + 0.5f) * Model.CellSize + Model.BORDER_GAP, (coordinates.y + 0.5f) * Model.CellSize + Model.BORDER_GAP);
    }
    #endregion


    #region Public Methods
    public void Init (Vector2 coordinates, Action onDestroyCallback)
    {
        UpdateCoordinates (coordinates);

        OnDestroyCallback = onDestroyCallback;
    }

    public void UpdateCoordinates (Vector2 coordinates)
    {
        this.coordinates = coordinates;

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