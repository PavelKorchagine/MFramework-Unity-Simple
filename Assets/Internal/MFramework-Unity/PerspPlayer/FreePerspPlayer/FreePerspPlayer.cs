using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
#region using UNITY_EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[RequireComponent(typeof(CharacterController))]
public class FreePerspPlayer : BasePerspPlayer
{
    private Transform m_Player;
    [Range(0, 50)]
    [SerializeField]
    private float speedMove = 1;
    [Range(0.1f, 5.0f)]
    [SerializeField]
    private float speedMoveRatioRate = 1.5f;
    [SerializeField]
    private KeyCode moveRatioKey = KeyCode.LeftShift;

    [Range(0, 20)]
    [SerializeField]
    private float rotateMove = 10;
    [Range(0.1f, 5.0f)]
    [SerializeField]
    private float rotateMoveRatioRate = 1.5f;
    [SerializeField] private Vector2 m_MinimumXMaximumX = new Vector2(-90f, 90f);
    private CharacterController m_Character;

    private Transform simlateTra;
    private bool isRotate;
    private PerspPlayerMouseLook m_MouseLook;


    protected override void Reset()
    {
        var rigi = GetComponent<Rigidbody>();
        if (rigi != null && rigi.useGravity == true)
        {
            rigi.useGravity = false;
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL

#elif UNITY_ANDROID || UNITY_IOS
       
#endif
    }

    protected override void Awake()
    {
        if (isInitedSelf)
        {
            OnInit(1, 1, 1);
        }

    }

    protected override void Start()
    {
        if (isInitedSelf)
        {
            OnInit();
        }
       
    }

    public override void OnInit(params object[] paras)
    {
        base.OnInit(paras);
        m_Character = GetComponent<CharacterController>();
        if (m_Character == null)
        {
            m_Character = gameObject.AddComponent<CharacterController>();
        }
    }

    public override void OnInit()
    {
        base.OnInit();

        var rigi = GetComponent<Rigidbody>();
        if (rigi != null && rigi.useGravity == true)
        {
            rigi.useGravity = false;
        }

        m_Player = transform;

        simlateTra = new GameObject("simlateTra To Rotate").transform;
        simlateTra.parent = transform;
        simlateTra.localPosition = simlateTra.localEulerAngles = Vector3.zero;
        simlateTra.localScale = Vector3.one;

        simlateTra.eulerAngles = new Vector3(m_Camera.transform.eulerAngles.x, m_Player.eulerAngles.y, 0);

        m_MouseLook = new PerspPlayerMouseLook(m_Player, m_Camera.transform, updateType);
        m_MouseLook.smooth = false;
        m_MouseLook.smoothTime = 15;
        m_MouseLook.MinimumX = m_MinimumXMaximumX.x;
        m_MouseLook.MaximumX = m_MinimumXMaximumX.y;

        lastPlayerTrafer = IsPlayerTrafering = false;
    }

    public override void OnReset()
    {
        base.OnReset();
    }

    protected override void Update()
    {
        CallBack();

        if (updateType != UpdateType.Update) // 判断是不是Update更新
        {
            IsPlayerTrafering = false;
            return;
        }

        if (CheckError())
        {
            IsPlayerTrafering = false;
            return;
        }

        #region IsClickUI UnActived
        //if (IsClickUI())
        //{
        //    IsPlayerTrafering = false;
        //    return;
        //} 
        #endregion

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
#elif UNITY_ANDROID || UNITY_IOS
#endif

        Move();
        Rotate();
        IsPlayerTrafering = true;
       
        return;

        #region Rotate 2
        /*
        if (UnityEngine.Input.GetMouseButton(1) || UnityEngine.Input.GetMouseButton(2) || isRotate)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            Vector2 input = new Vector2
            {
                x = UnityEngine.Input.GetAxis("Horizontal") * Time.deltaTime * rotateMove,
                y = UnityEngine.Input.GetAxis("Vertical") * Time.deltaTime * rotateMove
            };

            // 得到鼠标当前位置
            float mouseX = Input.GetAxis("Mouse X") * rotateMove;
            float mouseY = Input.GetAxis("Mouse Y") * rotateMove;

            // 设置照相机和Player的旋转角度，X,Y值需要更具情况变化位置
            m_CameraTra.localRotation = m_CameraTra.localRotation * Quaternion.Euler(-mouseY, 0, 0);
            m_Player.localRotation = m_Player.localRotation * Quaternion.Euler(0, mouseX, 0);

            simlateTra.eulerAngles = new Vector3(m_CameraTra.eulerAngles.x, m_Player.eulerAngles.y, 0);
#elif UNITY_ANDROID || UNITY_IOS

#endif

        }
        */
        #endregion

    }

    protected override void CallBack()
    {
        if (IsPlayerTrafering != lastPlayerTrafer && IsPlayerTrafering == true)
        {
            if (onPerspStartTranEvent != null) onPerspStartTranEvent.Invoke(simlateTra);
        }
        else if (IsPlayerTrafering != lastPlayerTrafer && IsPlayerTrafering == false)
        {
            if (onPerspEndTranEvent != null) onPerspEndTranEvent.Invoke(simlateTra);
        }
        else if (IsPlayerTrafering == lastPlayerTrafer && IsPlayerTrafering == true)
        {
            if (onPerspTraningEvent != null) onPerspTraningEvent.Invoke(simlateTra);
        }

        lastPlayerTrafer = IsPlayerTrafering;
    }

    protected override void FixedUpdate()
    {
        if (updateType != UpdateType.FixedUpdate) // 判断是不是FixedUpdate更新
        {
            IsPlayerTrafering = false;
            return;
        }

        if (CheckError())
        {
            IsPlayerTrafering = false;
            return;
        }

        Move();
        Rotate();
        IsPlayerTrafering = true;

        return;
    }

    protected override void LateUpdate()
    {
        if (updateType != UpdateType.LateUpdate) // 判断是不是LateUpdate更新
        {
            IsPlayerTrafering = false;
            return;
        }

        if (CheckError())
        {
            IsPlayerTrafering = false;
            return;
        }

        Move();
        Rotate();
        IsPlayerTrafering = true;

    }

    protected bool CheckError()
    {
        bool result = false;

        if ((this.intervalPause == true || this.stop == true || this.pause == true) /*&& !_mouseLook.lockCursor*/)
        {
            result = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        if ((!UnityEngine.Input.anyKey))
        {
            result = true;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if ((UnityEngine.Input.touchCount == 0))
        {
            result = true;
        }
#endif

        #region IsClickUI UnActived
        //if (IsClickUI())
        //{
        //    IsPlayerTrafering = false;
        //    return;
        //} 
        #endregion

        return result;
    }

    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="ratio">是否加速</param>
    private void Move()
    {
        bool ratio = false;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        if (UnityEngine.Input.GetKey(moveRatioKey))
        {
            ratio = true;
        }
#elif UNITY_ANDROID || UNITY_IOS
#endif
        var _tempSpeedMove = ratio ? speedMove * speedMoveRatioRate : speedMove;
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

        Vector2 m_Input;
        GetInput(out m_Input);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        if (UnityEngine.Input.GetKey(KeyCode.E))
        {
            m_Character.Move(Vector3.up * _deltaTime * _tempSpeedMove);
        }
        else if (UnityEngine.Input.GetKey(KeyCode.Q))
        {
            m_Character.Move(-Vector3.up * _deltaTime * _tempSpeedMove);
        }
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = simlateTra.forward * m_Input.y + simlateTra.right * m_Input.x;
#elif UNITY_ANDROID || UNITY_IOS
#endif

        m_Character.Move(desiredMove * _deltaTime * _tempSpeedMove);
        simlateTra.eulerAngles = new Vector3(m_Camera.transform.eulerAngles.x, m_Player.eulerAngles.y, 0);

        // Finily TODO
        ratio = false;
    }

    private void Rotate()
    {
        bool enabled = false;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL

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

#elif UNITY_ANDROID || UNITY_IOS

#endif
        if (enabled)
        {
            m_MouseLook.LookRotation(m_Player, m_Camera.transform);
        }

    }

    private void GetInput(out Vector2 m_Input)
    {
        m_Input = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        // Read input
        m_Input.x = UnityEngine.Input.GetAxis("Horizontal");
        m_Input.y = UnityEngine.Input.GetAxis("Vertical");
#elif UNITY_ANDROID || UNITY_IOS
       
#endif

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

    }

    protected override void OnApplicationPause(bool pause)
    {
        this.pause = pause;
    }


    #region CustomEditor(typeof(FreePerspPlayer)
#if UNITY_EDITOR

    protected bool _showParameter = true;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(FreePerspPlayer))]
    public class FreePerspPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FreePerspPlayer manager = (FreePerspPlayer)target;
            GUI.changed = false;
            serializedObject.Update();
            //GUI style 设置
            GUIStyle firstLevelStyle = new GUIStyle(GUI.skin.label);
            firstLevelStyle.alignment = TextAnchor.UpperLeft;
            firstLevelStyle.fontStyle = FontStyle.Normal;
            firstLevelStyle.fontSize = 11;
            firstLevelStyle.wordWrap = true;

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;

            #region ShowParameter
            GUILayout.BeginVertical("", boxStyle);
            manager._showParameter = EditorGUILayout.BeginToggleGroup(string.Format("ShowParameter"), manager._showParameter);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndVertical();

            if (manager._showParameter)
            {
                GUILayout.BeginVertical("", boxStyle);
                base.OnInspectorGUI();
                //GUILayout.BeginHorizontal("", boxStyle);
                //GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            #endregion

        }
    }

#endif
    #endregion

}
