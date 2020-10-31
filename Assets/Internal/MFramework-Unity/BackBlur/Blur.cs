using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MFramework_Unity.Tools;
using DG.Tweening;
#region using UNITY_EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

/* 
**************************************************************************************************************
Copyright/版权 (C) 2020 The arHop Studio. All Rights Reserved. 保留所有权利。
File Name/文件名:           Blur.cs
Discription/描述:     		Be fully careful of  Code modification!
Author/作者:             	Korchagin
CreateTime/创建时间:        2020/7/14 11:18:33
**************************************************************************************************************
*/

/// <summary>
/// Blur
/// </summary>
[ExecuteInEditMode]
public class Blur : MonoBehaviour
{
    public Material blurMaterial;
    [Range(0.0f, 2.0f)]
    public float blurValue = 0.5f;
    private Transform simlateObj;

    private void OnEnable()
    {
        if (Application.isPlaying)
        {
            blurMaterial = GetComponent<UnityEngine.UI.Image>().material;
            if (simlateObj == null)
            {
                simlateObj = new GameObject("simlateObj").transform;
                simlateObj.SetParent(transform);
                simlateObj.Centralizate();
            }
        }

        if (blurMaterial && simlateObj && Application.isPlaying)
        {
            //ExcuteToTarget();
        }
    }

    public void ExcuteToTarget(float targetVa = 0.5f, float speed = 1)
    {
        if (simlateObj == null) return;
        simlateObj.DOKill();
        float dur = Mathf.Abs(targetVa - simlateObj.localPosition.y) / speed;
        simlateObj.DOLocalMoveY(targetVa, dur).OnUpdate(()=> {
            float runtimeV = simlateObj.localPosition.y;
            blurValue = runtimeV;
            if (blurMaterial) blurMaterial.SetFloat("_Size", blurValue);
        });
    }

    public void ExcuteFocusToTarget(float targetVa = 0.5f, float dur = 0.5f)
    {
        if (simlateObj == null) return;
        simlateObj.DOKill();
        simlateObj.Centralizate();
        simlateObj.DOLocalMoveY(targetVa, dur).OnUpdate(() => {
            float runtimeV = simlateObj.localPosition.y;
            blurValue = runtimeV;
            if (blurMaterial) blurMaterial.SetFloat("_Size", blurValue);
        });
    }

    public void ExcuteToZero(float speed = 1)
    {
        if (simlateObj == null) return;
        simlateObj.DOKill();
        float dur = Mathf.Abs(simlateObj.localPosition.y) / speed;
        simlateObj.DOLocalMoveY(0, dur).OnUpdate(() => {
            float runtimeV = simlateObj.localPosition.y;
            blurValue = runtimeV;
            if (blurMaterial) blurMaterial.SetFloat("_Size", blurValue);
        });
    }

    public void ExcuteFocusToZero(float targetVa = 0.5f, float dur = 0.5f)
    {
        if (simlateObj == null) return;
        simlateObj.DOKill();
        simlateObj.Centralizate();
        simlateObj.localPosition = Vector3.up * targetVa;
        simlateObj.DOLocalMoveY(0, dur).OnUpdate(() => {
            float runtimeV = simlateObj.localPosition.y;
            blurValue = runtimeV;
            if (blurMaterial) blurMaterial.SetFloat("_Size", blurValue);
        });
    }

    private void Update()
    {
        if (blurMaterial == null || Application.isPlaying) return;

        blurMaterial.SetFloat("_Size", blurValue);
    }

    private void OnDestroy()
    {
        blurValue = 0;
    }
}

