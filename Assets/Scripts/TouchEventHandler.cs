using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchEventHandler : SingletonMonoBehaviour<TouchEventHandler>,
    IPointerDownHandler,
    IPointerUpHandler,
    IBeginDragHandler,
    IEndDragHandler,
    IDragHandler
{

    //タップ中
    private bool _isPressing = false;
    public bool IsPressing
    {
        get { return _isPressing; }
    }

    //ドラッグ中か
    private bool _isDragging = false;
    public bool IsDragging
    {
        get { return _isDragging; }
    }

    //ピンチ中かのフラグ
    private bool _isPinching = false;
    public bool IsPinching
    {
        get { return _isPinching; }
    }

    //全フレームでのドラッグ位置（ワールド座標）
    private Vector3 _beforeTapWorldPoint;

    //ピンチ開始時の指の距離
    private float _beforeDistanceOfPinch;

    //タップ
    public event Action<bool> onPress = delegate { };
    public event Action onBeginPress = delegate { };
    public event Action onEndPress = delegate { };
    public event Action onTap = delegate { };

    //ドラッグ
    public event Action<Vector2> onDrag = delegate { };
    public event Action<Vector3> onDragIn3D = delegate { };
    public event Action onBeginDrag = delegate { };
    public event Action onEndDrag = delegate { };

    //ピンチ
    public event Action<float> onPinch = delegate { };
    public event Action onBeginPinch = delegate { };
    public event Action onEndPinch = delegate { };

    //ドラッグしてる指のデータ
    private Dictionary<int, PointerEventData> _draggingDataDict = new Dictionary<int, PointerEventData>();

    System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

    //
    //タップ
    //
    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressing = true;

        onBeginPress();
        onPress(true);
        sw.Start();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressing = false;

        onEndPress();
        onPress(false);

        //ピンチ終了イベント実行
        if (_isPinching && _draggingDataDict.Count < 2)
        {
            _isPinching = false;
            onEndPinch();
        }
        OnTap(eventData);
        sw.Reset();
    }

    //
    //タップ（画面タッチ後、0.5秒に指を離す）
    //
    public void OnTap(PointerEventData eventData)
    {
        if (!_isDragging && sw.ElapsedMilliseconds < 500f) 
        {
            onTap();
        }
    }

    //
    //ドラッグ
    //
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            _beforeTapWorldPoint = GetTapWorldPoint();
        }

        _isDragging = true;
        onBeginDrag();

        //ドラッグデータの追加
        _draggingDataDict[eventData.pointerId] = eventData;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        onEndDrag();

        if (_draggingDataDict.ContainsKey(eventData.pointerId))
        {
            _draggingDataDict.Remove(eventData.pointerId);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging)
        {
            OnBeginDrag(eventData);
            return;
        }

        _draggingDataDict[eventData.pointerId] = eventData;

        if (_draggingDataDict.Count >= 2)
        {
            if (_isDragging)
            {
                _isDragging = false;
                onEndDrag();
            }

            OnPinch();
        }
        else if (Input.touches.Length <= 1)
        {
            if (_isPinching)
            {
                _isDragging = false;
                _isPinching = false;
                onEndPinch();
                OnBeginDrag(eventData);
            }

            onDrag(eventData.delta);
            OnDragInWorldPoint();
        }
    }

    //
    //ドラッグ中（座標をワールド座標で返す）
    //
    public void OnDragInWorldPoint()
    {
        Vector3 tapWorldPoint = GetTapWorldPoint();
        onDragIn3D(tapWorldPoint - _beforeTapWorldPoint);
        _beforeTapWorldPoint = tapWorldPoint;
    }

    //タップしている場所をワールド座標で取得
    private Vector3 GetTapWorldPoint()
    {
        //タップ位置を画面内のワールド座標に変換
        Vector2 screenPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(),
        new Vector2(Input.mousePosition.x, Input.mousePosition.y),
        null,
        out screenPos
        );

        //ワールド座標に変換
        Vector3 tapWorldPoint = Camera.main.ScreenToWorldPoint(
        new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z)
        );

        return tapWorldPoint;
    }

    //
    //ピンチ
    //
    public void OnPinch()
    {
        Vector2 firstTouch = Vector2.zero;
        Vector2 secondTouch = Vector2.zero;
        int count = 0;

        foreach(var draggingData in _draggingDataDict)
        {
            if(count == 0)
            {
                firstTouch = draggingData.Value.position;
                break;
            }
            else if(count == 1)
            {
                secondTouch = draggingData.Value.position;
                break;
            }
        }

        //ピンチ中の幅を取得
        float distanceOfPinch = Vector2.Distance(firstTouch, secondTouch);

        if (!_isPinching)
        {
            _isPinching = true;
            _beforeDistanceOfPinch = distanceOfPinch;

            onBeginPinch();
            return;
        }

        float pinchDiff = distanceOfPinch - _beforeDistanceOfPinch;
        onPinch(pinchDiff);

        _beforeDistanceOfPinch = distanceOfPinch;
    }
}

