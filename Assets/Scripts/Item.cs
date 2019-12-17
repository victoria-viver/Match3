/**
 * Created by: Victoria Shenkevich
 * Created on: 15/12/2019
 */

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IDragHandler, IEndDragHandler
{
    #region Private Fields
    private Vector2 coordinates;
    
    private Image image;
    private RectTransform rectTransform;
    
    [SerializeField] private GameObject popAnimation;
    private SpriteSheetAnimation popAnimationScript;

    private Action<Vector2, Vector2> OnMovementCallback;
    private Action OnMovementStopCallback;
    private Action OnDestroyCallback;

    private Vector2 startPosition;
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

    void OnDestroy()
    {
        OnDestroyCallback();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - startPosition.x;
        float deltaY = eventData.position.y - startPosition.y;
        float delta = Math.Abs(deltaX) > Math.Abs(deltaY) ? deltaX : deltaY;

        if (Math.Abs(delta) > Model.CellSize/2)
            OnMovementCallback(coordinates, new Vector2(deltaX, deltaY));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnMovementStopCallback ();
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
        startPosition = transform.position;
    }
    #endregion


    #region Public Methods
    public void Init (Vector2 coordinates, Action<Vector2, Vector2> onMovementCallback, Action onMovementStopCallback, Action onDestroyCallback)
    {
        UpdateCoordinates (coordinates);

        OnMovementCallback = onMovementCallback;
        OnMovementStopCallback = onMovementStopCallback;
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