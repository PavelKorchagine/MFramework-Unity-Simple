using System;
using UnityEngine;
using Random = UnityEngine.Random;
#region using UNITY_EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Rigidbody))]
public class FirstPerspPlayer : BasePerspPlayer
{
    private bool m_IsWalking;
    private Transform m_Player;
    [Range(0, 1.5f)]
    [SerializeField]
    private float stepOffset = 0.65f;
    [Range(0, 50)]
    [SerializeField]
    private float speedMove = 1;
    [Range(0.1f, 5.0f)]
    [SerializeField]
    private float speedMoveRatioRate = 1.5f;
    [SerializeField]
    private KeyCode moveRatioKey = KeyCode.LeftShift;

    public Vector2 m_MinimumXMaximumX = new Vector2(-90f, 90f);
    public Vector2 m_Sensitivity = new Vector2(-2f, 2f);

    private float m_StickToGroundForce = 10;
    private float m_GravityMultiplier = 2;
    private PerspPlayerMouseLook m_MouseLook;

    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_Character;
    private Rigidbody m_Rigid;
    [SerializeField] private CollisionFlags m_CollisionFlags;
    [SerializeField] private bool m_PreviouslyGrounded;

    public Vector3 defaultlPos = Vector3.zero;
    public Vector3 defaultlEuler = Vector3.zero;
    public Vector3 defaultlSca = Vector3.one;

    [SerializeField] protected bool isMouseLock = true;
    public KeyCode InternalLockUpdateK = KeyCode.Space;

    protected override void Reset()
    {
        m_Rigid = GetComponent<Rigidbody>();
        m_Rigid.isKinematic = true;

        m_Character = GetComponent<CharacterController>();
        m_Character.stepOffset = stepOffset;

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

        m_Rigid = GetComponent<Rigidbody>();
        m_Rigid.isKinematic = true;
        m_Character = GetComponent<CharacterController>();
        if (m_Character == null)
        {
            m_Character = gameObject.AddComponent<CharacterController>();
        }

    }

    public override void OnInit()
    {
        base.OnInit();

        m_Player = transform;
        m_Character.stepOffset = stepOffset;

        m_MouseLook = new PerspPlayerMouseLook(transform, m_Camera.transform, updateType);
        m_MouseLook.smooth = false;
        m_MouseLook.InternalLockUpdateK = InternalLockUpdateK;
        m_MouseLook.MinimumX = m_MinimumXMaximumX.x;
        m_MouseLook.MaximumX = m_MinimumXMaximumX.y;
        m_MouseLook.XSensitivity = m_Sensitivity.x;
        m_MouseLook.YSensitivity = m_Sensitivity.y;
        m_MouseLook.SetCursorLock(true);

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

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

        m_MouseLook.UpdateCursorLock();

        if (m_PreviouslyGrounded != m_Character.isGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_Character.isGrounded;

        Move();
        Rotate();
        IsPlayerTrafering = true;

        return;
    }

    protected override void Update()
    {
        CallBack();

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

        m_MouseLook.UpdateCursorLock();
        if (m_PreviouslyGrounded != m_Character.isGrounded) m_MoveDir.y = 0f;
        m_PreviouslyGrounded = m_Character.isGrounded;

        Move();
        Rotate();
        IsPlayerTrafering = true;

        return;
    }

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

        m_MouseLook.UpdateCursorLock();
        if (m_PreviouslyGrounded != m_Character.isGrounded) m_MoveDir.y = 0f;
        m_PreviouslyGrounded = m_Character.isGrounded;

        Move();
        Rotate();
        IsPlayerTrafering = true;

        return;
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

    protected override void CallBack()
    {
        if (IsPlayerTrafering != lastPlayerTrafer && IsPlayerTrafering == true)
        {
            if (onPerspStartTranEvent != null) onPerspStartTranEvent.Invoke(new Transform[] { m_Player, m_Camera.transform });
        }
        else if (IsPlayerTrafering != lastPlayerTrafer && IsPlayerTrafering == false)
        {
            if (onPerspEndTranEvent != null) onPerspEndTranEvent.Invoke(new Transform[] { m_Player, m_Camera.transform });
        }
        else if (IsPlayerTrafering == lastPlayerTrafer && IsPlayerTrafering == true)
        {
            if (onPerspTraningEvent != null) onPerspTraningEvent.Invoke(new Transform[] { m_Player, m_Camera.transform });
        }

        lastPlayerTrafer = IsPlayerTrafering;
    }

    protected override void OnApplicationPause(bool pause)
    {
        this.pause = pause;
    }

    public override void OnReset()
    {
        base.OnReset();
    }

    private void Rotate()
    {
        //m_MouseLook.UpdateCursorLock();
        if (m_MouseLook.GetCurrentCursor())
        {
            m_MouseLook.LookRotation(m_Player, m_Camera.transform);
        }

        if (m_PreviouslyGrounded != m_Character.isGrounded)
        {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = m_Character.isGrounded;
    }

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

        Vector2 m_Input;
        GetInput(out m_Input);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        if (Physics.SphereCast(transform.position, m_Character.radius, Vector3.down, out hitInfo,
                           m_Character.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
        }
        
        m_MoveDir.x = desiredMove.x * _tempSpeedMove;
        m_MoveDir.z = desiredMove.z * _tempSpeedMove;

        var _deltaTime = Time.deltaTime;
        switch (this.updateType)
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

        if (m_Character.isGrounded)
        {
            m_MoveDir.y = -m_StickToGroundForce;
        }
        else
        {
            m_MoveDir += Physics.gravity * m_GravityMultiplier * _deltaTime;
        }

        m_CollisionFlags = m_Character.Move(m_MoveDir * _deltaTime);
    }

    private void GetInput(out Vector2 m_Input)
    {
        m_Input = Vector2.zero;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        m_Input.x = UnityEngine.Input.GetAxis("Horizontal");
        m_Input.y = UnityEngine.Input.GetAxis("Vertical");

#elif UNITY_ANDROID || UNITY_IOS
       
#endif
        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }

        //bool waswalking = false;
        //waswalking = m_IsWalking;
        //m_IsWalking = !UnityEngine.Input.GetKey(KeyCode.LeftShift);
        //speed = m_IsWalking ? speedMove : speedMove * speedMoveRatioRate;
       
    }


    #region CustomEditor(typeof(FirstPerspPlayer)
#if UNITY_EDITOR

    protected bool _showParameter = true;

    [CanEditMultipleObjects]
    [CustomEditor(typeof(FirstPerspPlayer))]
    public class FirstPerspPlayerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FirstPerspPlayer manager = (FirstPerspPlayer)target;
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

