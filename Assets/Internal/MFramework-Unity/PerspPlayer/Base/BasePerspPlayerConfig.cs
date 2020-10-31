using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct Quater
{
    public Quaternion m_CharacterTargetRot;
    public Quaternion m_CameraTargetRot;

    public Quater(Quaternion m_CharacterTargetRot, Quaternion m_CameraTargetRot)
    {
        this.m_CharacterTargetRot = m_CharacterTargetRot;
        this.m_CameraTargetRot = m_CameraTargetRot;
    }
}

public class OnPerspStartTranEvent : UnityEvent<object>
{
    public OnPerspStartTranEvent()
    {

    }
}

public class OnPerspEndTranEvent : UnityEvent<object>
{
    public OnPerspEndTranEvent()
    {

    }
}

public class OnPerspTraningEvent : UnityEvent<object>
{
    public OnPerspTraningEvent()
    {

    }
}

public enum UpdateType
{
    Update,
    FixedUpdate,
    LateUpdate
}

public enum RotateKey
{
    LeftKey,
    RightKey,
    LeftAndRightKey,
}

[Serializable]
public class PerspPlayerMouseLook
{
    public float XSensitivity = 2f;
    public float YSensitivity = 2f;
    public bool clampVerticalRotation = true;
    public float MinimumX = -90F;
    public float MaximumX = 90F;
    public bool smooth;
    public float smoothTime = 5f;
    public bool lockCursor = true;
    public UpdateType updateType = UpdateType.Update;
    public KeyCode InternalLockUpdateK = KeyCode.Space;

    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    private bool m_cursorIsLocked = true;

    public PerspPlayerMouseLook()
    {

    }

    public PerspPlayerMouseLook(Transform character, Transform camera, UpdateType updateType = UpdateType.Update)
    {
        m_CharacterTargetRot = character.localRotation;
        m_CameraTargetRot = camera.localRotation;
        this.updateType = updateType;
    }

    public void LookRotation(Transform character, Transform camera)
    {
        float yRot = 0;
        float xRot = 0;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        yRot = UnityEngine.Input.GetAxis("Mouse X") * XSensitivity;
        xRot = UnityEngine.Input.GetAxis("Mouse Y") * YSensitivity;
#elif UNITY_ANDROID || UNITY_IOS
#endif

        m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

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

        if (smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                smoothTime * _deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                smoothTime * _deltaTime);
        }
        else
        {
            character.localRotation = m_CharacterTargetRot;
            camera.localRotation = m_CameraTargetRot;
        }

        //UpdateCursorLock();
    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            //we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public bool GetCurrentCursor()
    {
        return m_cursorIsLocked;
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL
        if (UnityEngine.Input.GetKeyDown(InternalLockUpdateK)) 
        {
            m_cursorIsLocked = !m_cursorIsLocked;
            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
#endif


       
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;
        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);
        return q;
    }

}
