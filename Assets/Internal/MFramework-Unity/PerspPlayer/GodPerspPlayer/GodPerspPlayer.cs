using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region using UNITY_EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

public class GodPerspPlayer : BasePerspPlayer
{
    protected Transform centerY, centerX, player;

    public Vector2 sensitivity = new Vector2(2f, 2f);
    public bool clampVerticalRotation = true;
    public Vector2 MinMaximum = new Vector2(-90, 90);
    public float ZoomSpeed = 10;
    public Vector2 NFDistance = new Vector2(-100, -900);

    private Quater m_Quater;
    private Vector3 lpos;
    private Vector3 pivet;

    protected Vector3 oriTrasPos = Vector3.zero;
    protected Vector3 oriTrasEuler = Vector3.zero;
    [SerializeField] protected bool enableSmooth = true;
    [SerializeField] protected float smoothDur = 0.75f;
    [SerializeField] protected float lerpRate = 10;
    protected float _smoother = 0;

    [SerializeField] protected bool isTouchPanelActive = false;

    protected override void Reset()
    {

    }

    protected override void Awake()
    {
        if (isInitedSelf)
        {
            OnInit(1, 1, 1);
        }

    }

    // Start is called before the first frame update
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

        if (centerY == null) centerY = transform;
        if (centerX == null) centerX = transform.Find("CenterX");
        if (player == null) player = transform.Find("CenterX/Camera_01");

    }

    public override void OnInit()
    {
        base.OnInit();

        m_Quater = new Quater(centerY.localRotation, centerX.localRotation);
        lpos = player.localPosition;
        pivet = transform.localPosition;
    }

    public override void UpdateParams()
    {
        m_Quater = new Quater(centerY.localRotation, centerX.localRotation);
        lpos = player.localPosition;
        pivet = transform.localPosition;
    }

    private bool outInput = false;
    /// <summary>
    /// 上帝视角，视角切换
    /// </summary>
    /// <param name="godPlayerEulur">godPlayer方向</param>
    /// <param name="centerXEulur">centerX方向</param>
    /// <param name="cameraPos">camera位置</param>
    public void TurnGodPer(Vector3 godPlayerEulur, Vector3 centerXEulur, Vector3 cameraPos)
    {
        ToDOCalOutInput();

        m_Quater.m_CharacterTargetRot = Quaternion.Euler(godPlayerEulur);
        m_Quater.m_CameraTargetRot = Quaternion.Euler(centerXEulur);

        lpos = cameraPos;

        if (clampVerticalRotation)
            m_Quater.m_CameraTargetRot = ClampRotationAroundXAxis(m_Quater.m_CameraTargetRot);

    }

    public void TurnGodPer(Vector3 pivet, Vector3 godPlayerEulur, Vector3 centerXEulur, Vector3 cameraPos)
    {
        ToDOCalOutInput();

        m_Quater.m_CharacterTargetRot = Quaternion.Euler(godPlayerEulur);
        m_Quater.m_CameraTargetRot = Quaternion.Euler(centerXEulur);

        this.pivet = pivet;
        lpos = cameraPos;

        if (clampVerticalRotation)
            m_Quater.m_CameraTargetRot = ClampRotationAroundXAxis(m_Quater.m_CameraTargetRot);

    }

    /// <summary>
    /// 上帝视角，视角切换
    /// </summary>
    /// <param name="godPlayerEulur">godPlayer方向</param>
    /// <param name="centerXEulur">centerX方向</param>
    /// <param name="cameraPos">camera位置</param>
    public void TurnGodPer(Vector3 pivet)
    {
        //ToDOCalOutInput();
        this.pivet = pivet;

        IsPlayerTrafering = true;
        _smoother = smoothDur;
    }

    public void TurnGodPer(Vector3 pivet, Vector3 cameraPos)
    {
        ToDOCalOutInput();

        this.pivet = pivet;
        lpos = cameraPos;

        IsPlayerTrafering = true;
        _smoother = smoothDur;

    }

    public override void OnReset()
    {
        base.OnReset();

    }

    private int _countCalOutInput = 0;
    private int countCalOutInput = 256;

    private void ToDOCalOutInput()
    {
        outInput = true;
        _countCalOutInput = 0;
    }

    private void CalOutInput()
    {
        if (outInput)
        {
            _countCalOutInput++;
            if (_countCalOutInput >= countCalOutInput)
            {
                _countCalOutInput = 0;
                outInput = false;
            }
        }
    }

    public override void OnUpdate()
    {
        if (isTouchPanelActive)
        {
            if (_getRotateEK())
                GetInput();
            //GetInputMoveForward();
        }

    }


    public void OnUpdateMoveForward()
    {
        if (isTouchPanelActive)
        {
            //GetInput();
            if (_getMouseScrollWheel() != 0)
                GetInputMoveForward();
        }

    }

    protected override void Update()
    {
        base.OnUpdate();
        CalOutInput();

        CallBack();

        if (updateType != UpdateType.Update) // 判断是不是Update更新
        {
            return;
        }

        if (CheckError())
        {
            IsPlayerTrafering = false;
            return;
        }

        if (!isTouchPanelActive)
        {
            if (_getRotateEK())
                GetInput();
            if (_getMouseScrollWheel() != 0)
                GetInputMoveForward();
        }
        MoveAndRotate();
        SpaceMove();
    }

    protected bool CheckError()
    {
        bool result = false;

        if ((this.intervalPause == true || this.stop == true || this.pause == true) /*&& !_mouseLook.lockCursor*/)
        {
            result = true;
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
#elif UNITY_ANDROID || UNITY_IOS
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

    // Update is called once per frame
    protected override void LateUpdate()
    {
        if (updateType != UpdateType.LateUpdate) // 判断是不是Update更新
        {
            return;
        }

        if (CheckError())
        {
            IsPlayerTrafering = false;
            return;
        }

        MoveAndRotate();

    }

    protected override void FixedUpdate()
    {
        if (updateType != UpdateType.FixedUpdate) // 判断是不是Update更新
        {
            return;
        }

        if (CheckError())
        {
            IsPlayerTrafering = false;
            return;
        }

        MoveAndRotate();
    }

    public float speedSpaceMove = 100;
    private void SpaceMove()
    {
        if (UnityEngine.Input.GetKey(KeyCode.W))
        {
            transform.Translate(transform.forward * speedSpaceMove * Time.deltaTime, Space.World);
            this.pivet = transform.localPosition;

        }
        else if (UnityEngine.Input.GetKey(KeyCode.S))
        {
            transform.Translate(-transform.forward * speedSpaceMove * Time.deltaTime, Space.World);
            this.pivet = transform.localPosition;

        }


        if (UnityEngine.Input.GetKey(KeyCode.A))
        {
            transform.Translate(-transform.right * speedSpaceMove * Time.deltaTime, Space.World);
            this.pivet = transform.localPosition;

        }
        else if (UnityEngine.Input.GetKey(KeyCode.D))
        {
            transform.Translate(transform.right * speedSpaceMove * Time.deltaTime, Space.World);
            this.pivet = transform.localPosition;

        }

    }

    private Quater GetInput()
    {
        float yRot = 0;
        float xRot = 0;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        if (_getRotateEK())
        {
            yRot = UnityEngine.Input.GetAxis("Mouse X") * sensitivity.x;
            xRot = UnityEngine.Input.GetAxis("Mouse Y") * sensitivity.y;
            //yRot = UnityEngine.Input.GetAxis("Horizontal") * sensitivity.x;
            //xRot = UnityEngine.Input.GetAxis("Vertical") * sensitivity.y;
        }
#elif UNITY_ANDROID || UNITY_IOS
        // Read input
        yRot = UnityEngine.Input.GetTouch(0).deltaPosition.x * sensitivity.x;
        xRot = UnityEngine.Input.GetTouch(0).deltaPosition.y * sensitivity.y;
#endif
        m_Quater.m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_Quater.m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);
        if (clampVerticalRotation) m_Quater.m_CameraTargetRot = ClampRotationAroundXAxis(m_Quater.m_CameraTargetRot);

        return m_Quater;
    }

    private Vector3 GetInputMoveForward()
    {
        float m_MouseScrollWheel = 0;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
       
        if (_getMouseScrollWheel() != 0)
        {
            m_MouseScrollWheel = _getMouseScrollWheel();
        }
#elif UNITY_ANDROID || UNITY_IOS
        // Read input
#endif
        lpos = player.localPosition;
        lpos.z += m_MouseScrollWheel * ZoomSpeed;
        lpos.z = Mathf.Clamp(lpos.z, NFDistance.y, NFDistance.x);

        return lpos;
    }


    private void GetInputMobile()
    {
     

    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinMaximum.x, MinMaximum.y);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }

    private void MoveAndRotate()
    {
        var _deltaTime = _getDeltaTime();

        if (outInput || _getMouseScrollWheel() != 0 || _getRotateEK())
        {
            IsPlayerTrafering = true;

            _smoother = smoothDur;

            if (enableSmooth)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, pivet, _deltaTime * lerpRate);
                player.localPosition = Vector3.Lerp(player.localPosition, lpos, _deltaTime * lerpRate);

                centerY.localRotation = Quaternion.Lerp(centerY.localRotation, m_Quater.m_CharacterTargetRot, _deltaTime * lerpRate);
                centerX.localRotation = Quaternion.Lerp(centerX.localRotation, m_Quater.m_CameraTargetRot, _deltaTime * lerpRate);
            }
            else
            {
                transform.localPosition = pivet;
                player.localPosition = lpos;

                centerY.localRotation = m_Quater.m_CharacterTargetRot;
                centerX.localRotation = m_Quater.m_CameraTargetRot;

            }
        }
        else
        {
            LatedMoveAndRotate();
        }

    }

    private void LatedMoveAndRotate()
    {
        if (!enableSmooth || !IsPlayerTrafering)
            return;

        var _deltaTime = _getDeltaTime();

        if (_smoother > 0)
        {
            _smoother -= _deltaTime;
            centerY.localRotation = Quaternion.Lerp(centerY.localRotation, m_Quater.m_CharacterTargetRot, _deltaTime * lerpRate);
            centerX.localRotation = Quaternion.Lerp(centerX.localRotation, m_Quater.m_CameraTargetRot, _deltaTime * lerpRate);
            player.localPosition = Vector3.Lerp(player.localPosition, lpos, _deltaTime * lerpRate);
            transform.localPosition = Vector3.Lerp(transform.localPosition, pivet, _deltaTime * lerpRate);

            transform.localPosition = Vector3.Lerp(transform.localPosition, pivet, _deltaTime * lerpRate);

        }
        else if (_smoother <= 0 && IsPlayerTrafering)
        {
            _smoother = 0;
            IsPlayerTrafering = false;

        }


    }

    private void RotateOnMobile()
    {
        if ((UnityEngine.Input.touchCount == 1 && UnityEngine.Input.GetTouch(0).phase == TouchPhase.Moved))
        {
            IsPlayerTrafering = true;
            if (onPerspTraningEvent != null) onPerspTraningEvent.Invoke(transform);
            _smoother = smoothDur;

            GetInputMobile();
        }
        else
        {
            if (_smoother > 0)
            {
                _smoother -= Time.deltaTime;
                if (onPerspTraningEvent != null) onPerspTraningEvent.Invoke(transform);
            }
            else if (_smoother <= 0 && IsPlayerTrafering)
            {
                _smoother = 0;
                IsPlayerTrafering = false;
            }
        }

        centerY.localRotation = Quaternion.Lerp(centerY.localRotation, m_Quater.m_CharacterTargetRot, Time.deltaTime * 10);
        centerX.localRotation = Quaternion.Lerp(centerX.localRotation, m_Quater.m_CameraTargetRot, Time.deltaTime * 10);
    }
    private void MoveOnMobile()
    {
        if (UnityEngine.Input.touchCount < 2)
        {
            return;
        }

        var  m_MouseScrollWheel = Vector2.Distance(UnityEngine.Input.GetTouch(0).position, UnityEngine.Input.GetTouch(1).position);
        if (m_MouseScrollWheel != 0)
        {
            lpos = player.localPosition;
            lpos.z += m_MouseScrollWheel * ZoomSpeed;
            lpos.z = Mathf.Clamp(lpos.z, NFDistance.y, NFDistance.x);
            //player.localPosition = lpos;

            if (onPerspTraningEvent != null) onPerspTraningEvent.Invoke(transform);
        }

        player.localPosition = Vector3.Lerp(player.localPosition, lpos, Time.deltaTime * 10);
    }


    #region CustomEditor(typeof(GodPerspPlayer)
#if UNITY_EDITOR

    protected bool _showParameter = true;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(GodPerspPlayer))]
    public class GodPerspPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GodPerspPlayer manager = (GodPerspPlayer)target;
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
