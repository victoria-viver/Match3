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
    private enum Direction {None, Column, Row};
    private Vector2 coordinates;
    
    private Image image;
    private RectTransform rectTransform;
    
    [SerializeField] private GameObject popAnimation;
    private SpriteSheetAnimation popAnimationScript;

    private Action<Vector2, Vector2> OnMovementCallback;
    private Action OnMovementStopCallback;
    private Action OnDestroyCallback;

    private Vector2 startPosition;
    private Vector2 movementStartDelta;
    private Vector2 targetDelta;

    private int currentFrame = 0;
    private int animLengthInFrames = 5;
    private bool isMovableToTarget = false;
    private Direction direction = Direction.None;
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

    void Update ()
    {
        if (isMovableToTarget)
        {
            if (currentFrame < animLengthInFrames)
            {
                currentFrame++;

                Vector2 delta = movementStartDelta + (targetDelta - movementStartDelta) * currentFrame / animLengthInFrames;
                
                OnMovementCallback(coordinates, delta);
            }
            else
            {
                isMovableToTarget = false;
                currentFrame = 0;

                OnMovementStopCallback ();
            }
        }
    }

    void OnDestroy()
    {
        OnDestroyCallback();
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - startPosition.x;
        float deltaY = eventData.position.y - startPosition.y;
        float maxDelta = Math.Abs(deltaX) > Math.Abs(deltaY) ? deltaX : deltaY;

        if (direction == Direction.None)
        {
            if (maxDelta == deltaX)
                direction = Direction.Row;
            else
                direction = Direction.Column;
        }

        if (direction == Direction.Row)
            OnMovementCallback(coordinates, new Vector2(deltaX, 0));
        else
            OnMovementCallback(coordinates, new Vector2(0, deltaY));
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UpdateTargetPosition(eventData.position);

        direction = Direction.None;
    }
    #endregion


    #region Private Methods
    private void UpdateName ()
    {
        gameObject.name = "[" + coordinates.x + "," + coordinates.y + "]";
    }

    private void UpdatePosition ()
    {
        // 0.5f is added because items pivot is in the center
        transform.position = new Vector2((coordinates.x + 0.5f) * Model.CellSize + Model.BORDER_GAP, (coordinates.y + 0.5f) * Model.CellSize + Model.BORDER_GAP);
        startPosition = transform.position;
    }

    private void UpdateTargetPosition(Vector2 position)
    {
        isMovableToTarget = true;
        
        float deltaX = (position.x - startPosition.x) % Model.CellSize;
        float deltaY = (position.y - startPosition.y) % Model.CellSize;

        if (direction == Direction.Row)
        {
            movementStartDelta = new Vector2(deltaX, 0);

            if (Math.Abs(deltaX) >= Model.CellSize / 2)
            {
                int p = deltaX > 0 ? 1 : -1;
                targetDelta = new Vector2(Model.CellSize * p, 0);
            }
            else
            {
                targetDelta = new Vector2(0, 0);
            }
        }
        else if (direction == Direction.Column)
        {
            movementStartDelta = new Vector2(0, deltaY);

            if (Math.Abs(deltaY) >= Model.CellSize / 2)
            {
                int p = deltaY > 0 ? 1 : -1;
                targetDelta = new Vector2(0, Model.CellSize * p);
            }
            else
            {
                targetDelta = new Vector2(0, 0);
            }
        }
    }
    #endregion


    #region Public Methods
    public void Init (Vector2 coordinates, Action<Vector2, Vector2> onMovementCallback, Action onMovementStopCallback, Action onDestroyCallback)
    {
        UpdateCoordinates (coordinates, true);

        OnMovementCallback = onMovementCallback;
        OnMovementStopCallback = onMovementStopCallback;
        OnDestroyCallback = onDestroyCallback;
    }

    public void UpdateCoordinates (Vector2 coordinates, bool isInit = false)
    {
        this.coordinates = coordinates;

        UpdateName ();
        UpdatePosition();
    }

    public void MovePartially (Vector2 delta)
    {
        transform.position = startPosition + delta;
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