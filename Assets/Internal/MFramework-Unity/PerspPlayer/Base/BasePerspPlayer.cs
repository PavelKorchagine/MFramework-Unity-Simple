using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
#region using UNITY_EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

/* 
**************************************************************************************************************
Copyright/版权 (C) 2020 The arHop Studio. All Rights Reserved. 保留所有权利。
File Name/文件名:           BasePerspPlayerController.cs
Discription/描述:     		Be fully careful of  Code modification!
Author/作者:             	Korchagin
CreateTime/创建时间:        2020/5/20 9:24:23
**************************************************************************************************************
*/

/// <summary>
/// BasePerspPlayerController
/// </summary>
public class BasePerspPlayer : MonoBehaviour
{
    [Tooltip("是否自身初始化，默认开启")]
    [SerializeField]
    protected bool isInitedSelf = true;
    /// <summary>
    /// 是否正在移动
    /// </summary>
    public bool IsPlayerTrafering = false;
    protected bool lastPlayerTrafer = false;
    public Transform m_Camera;
    protected bool pause = false;
    protected bool stop = false;

    protected bool intervalPause = false;
    [SerializeField]
    protected RotateKey rotateKey = RotateKey.RightKey;

    [Tooltip("刷新类型")]
    [SerializeField]
    protected UpdateType updateType;

    public OnPerspStartTranEvent onPerspStartTranEvent { get; set; } = new OnPerspStartTranEvent();
    public OnPerspEndTranEvent onPerspEndTranEvent { get; set; } = new OnPerspEndTranEvent();
    public OnPerspTraningEvent onPerspTraningEvent { get; set; } = new OnPerspTraningEvent();

    #region UNITY_API

    protected virtual void Reset()
    {
       
    }

    protected virtual void Awake()
    {
        if (isInitedSelf)
        {
            OnInit(1, 1, 1);
        }
    }
    protected virtual void OnEnable()
    {
		
    }
    protected virtual void OnDisable()
    {
		
    }
    protected virtual void Start()
    {
        if (isInitedSelf)
        {
            OnInit();
        }
    }
    protected virtual void Update()
    {
		
    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected virtual void LateUpdate()
    {
        
    }

    protected virtual void OnApplicationPause(bool pause)
    {
        this.pause = pause;
    }

    #endregion

    #region OTHER_API

    public virtual void OnInit(params object[] paras)
    {
        if (m_Camera == null)
        {
            m_Camera = GetComponentInChildren<Camera>().transform;
        }

        if (m_Camera == null)
        {
            try
            {
                m_Camera = GetComponentsInChildren<Camera>(true)[0].transform;
            }
            catch (Exception)
            {
            }
        }

        if (m_Camera == null)
        {
            stop = true;

            #region NET_35 区分

#if NET_35  //如果是net 3.5版本
            //Debug.LogErrorFormat("{0}:{1} 组件不存在，请检查组件结构！", this, nameof(m_Camera));
#elif NET_4X
            //Debug.LogErrorFormat("{0}:{1} 组件不存在，请检查组件结构！", this, nameof(m_Camera));
#else       //如果不是 net3.5版本

#endif
            #endregion

            #region NET_2_0_SUBSET NET_2_0 NET_4_6
#if NET_2_0_SUBSET || NET_2_0
            Debug.LogWarningFormat("{0}:{1} 组件不存在，请检查组件结构！", this, "m_Camera");
#elif NET_4_6
            Debug.LogWarningFormat("{0}:{1} 组件不存在，请检查组件结构！", this, nameof(m_Camera));
#else
            Debug.LogWarningFormat("{0}:{1} 组件不存在，请检查组件结构！", this, "m_Camera");
#endif
            #endregion

        }
    }

    public virtual void OnInit()
    {

    }

    public virtual void OnReset()
    {

    }

    public virtual void OnUpdate()
    {

    }

    /// <summary>
    /// 检测是否点击在UI上
    /// </summary>
    /// <returns></returns>
    protected virtual bool _getIsClickUI()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
#elif UNITY_ANDROID || UNITY_IOS
#endif
        if (EventSystem.current != null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
        return false;
    }

    public virtual void ToIntervalPause(bool intervalPause)
    {
        this.intervalPause = intervalPause;
    }

    protected virtual void CallBack()
    {
        if (IsPlayerTrafering != lastPlayerTrafer && IsPlayerTrafering == true)
        {
            if (onPerspStartTranEvent != null) onPerspStartTranEvent.Invoke(transform);
        }
        else if (IsPlayerTrafering != lastPlayerTrafer && IsPlayerTrafering == false)
        {
            if (onPerspEndTranEvent != null) onPerspEndTranEvent.Invoke(transform);
        }
        else if (IsPlayerTrafering == lastPlayerTrafer && IsPlayerTrafering == true)
        {
            if (onPerspTraningEvent != null) onPerspTraningEvent.Invoke(transform);
        }

        lastPlayerTrafer = IsPlayerTrafering;
    }

    public virtual void UpdateParams()
    {

    }

    protected virtual float _getDeltaTime()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
#elif UNITY_ANDROID || UNITY_IOS
#endif
        var _deltaTime = Time.deltaTime;

        switch (updateType)
        {
            case UpdateType.Update:
            case UpdateType.LateUpdate:
                _deltaTime = Time.deltaTime;
                break;
            case UpdateType.FixedUpdate:
                _deltaTime = Time.fixedDeltaTime;
                break;
            default:
                break;
        }
        return _deltaTime;
    }

    protected virtual bool _getRotateEK()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
#elif UNITY_ANDROID || UNITY_IOS
#endif
        bool enabled = false;
        switch (rotateKey)
        {
            case RotateKey.LeftKey:
                enabled = UnityEngine.Input.GetMouseButton(0);
                break;
            case RotateKey.RightKey:
                enabled = UnityEngine.Input.GetMouseButton(1);
                break;
            case RotateKey.LeftAndRightKey:
                enabled = UnityEngine.Input.GetMouseButton(0) || UnityEngine.Input.GetMouseButton(1);
                break;
            default:
                break;
        }
        return enabled;
    }

    protected virtual float _getMouseScrollWheel()
    {
        float sc = 0;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        sc = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(sc);
#elif UNITY_ANDROID || UNITY_IOS
#endif
        return sc;
    }

    #endregion

}
