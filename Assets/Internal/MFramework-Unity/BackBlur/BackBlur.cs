using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BackBlur : MonoBehaviour
{
    [SerializeField] private Image _image;
    private float speed = 10;
    [SerializeField] private Vector2 vector2 = new Vector2(0, 1);

    [Range(0, 100)]
    private float size;
    private bool Tranftering = false;
    private int dir = 0;

    public void ShowBlur()
    {
        if (gameObject.activeSelf && gameObject.activeInHierarchy)
        {
            Tranftering = true;
            dir = 1;
        }
        else
        {
            size = vector2.y;
            _image.material.SetFloat("_Size", size);
        }
    }

    public void HideBlur()
    {
        if (gameObject.activeSelf && gameObject.activeInHierarchy)
        {
            Tranftering = true;
            dir = -1;

            
        }
        else
        {
            Back();

        }
    }

    public void Back()
    {
        //size = vector2.x;
        //_image.material.SetFloat("_Size", size);
        ToExcuteInSs(new Vector2(0.7f, 0), 0.3f);
    }

    private void Update()
    {
        if (Tranftering && _image.material != null)
        {
            size += dir * Time.deltaTime * speed;

            if (dir == 1 && size >= vector2.y)
            {
                dir = 0;
                size = vector2.y;
                Tranftering = false;
            }
            else if (dir == -1 && size <= vector2.x)
            {
                dir = 0;
                size = vector2.x;
                Tranftering = false;
            }
            _image.material.SetFloat("_Size", size);
        }
    }

    private GameObject simlateObj;
    public void ToExcuteInSs(Vector2 rad, float dur)
    {
        if (simlateObj == null)
        {
            simlateObj = new GameObject("simlateObj_BackBlur");
            simlateObj.transform.parent = null;
        }
        simlateObj.transform.DOKill();
        simlateObj.transform.position = Vector3.up * rad.x;
        simlateObj.transform.eulerAngles = Vector3.zero;

        SimlateObj s = simlateObj.GetComponent<SimlateObj>();
        if (s == null) s = simlateObj.AddComponent<SimlateObj>();
        s.toSimlateObj = this.gameObject;

        simlateObj.transform.DOMoveY(rad.y, dur).OnUpdate(() =>
        {
            ToProcess(simlateObj.transform.position.y);
        }).OnComplete(() =>
        {
            DestroyImmediate(simlateObj);
            simlateObj = null;
        }).SetEase(Ease.Linear);
    }

    private void ToProcess(float y)
    {
        _image.material.SetFloat("_Size", y);
    }


    private void ToProcess(Material material, float y)
    {
        material.SetFloat("_Size", y);
    }

    internal void Show()
    {
        //size = vector2.y;
        //_image.material.SetFloat("_Size", size);
        ToExcuteInSs(new Vector2(0, 0.7f), 0.5f);
        //_image.ToExcuteInSs(new Vector2(0, 0.7f), 0.5f, ToProcess);
    }

    private void OnDestroy()
    {
        _image.material.SetFloat("_Size", 0);
    }
}

//public static class MaterialEx
//{
//    public static void ToExcuteInSs(this Material material, Vector2 rad, float dur, string key = "_Size", string _tag = "BackBlur")
//    {
//        var simlateObj = new GameObject("simlateObj_" + _tag);
//        SimlateObj s = simlateObj.AddComponent<SimlateObj>();
//        //s.toSimlateObj = material.gameObject;
//        simlateObj.transform.parent = null;
//        simlateObj.transform.position = simlateObj.transform.eulerAngles = Vector3.up * rad.x;
//        simlateObj.transform.DOMoveY(rad.y, dur).OnUpdate(() => {
//            material.SetFloat("_Size", simlateObj.transform.position.y);
//        }).OnComplete(() => {
//            UnityEngine.Object.DestroyImmediate(simlateObj);
//        }).SetEase(Ease.Linear);
//    }
//}

//public static class UIImageMatEx
//{
//    public static void ToExcuteInSs(this Image _image, Vector2 rad, float dur, Action<Material, float> callback, string key = "_Size", string _tag = "BackBlur")
//    {
//        var simlateObj = new GameObject("simlateObj_" + _tag);
//        SimlateObj s = simlateObj.AddComponent<SimlateObj>();
//        s.toSimlateObj = _image.gameObject;
//        simlateObj.transform.parent = null;
//        simlateObj.transform.position = simlateObj.transform.eulerAngles = Vector3.up * rad.x;
//        simlateObj.transform.DOMoveY(rad.y, dur).OnUpdate(() => {
//            callback(_image.material, simlateObj.transform.position.y);
//        }).OnComplete(() => {
//            UnityEngine.Object.DestroyImmediate(simlateObj);
//        }).SetEase(Ease.Linear);
//    }
//}

//public static class RenderMatEx
//{
//    public static void ToExcuteInSs(this Renderer _render, Vector2 rad, float dur, Action<Material, float> callback, string key = "_Size", string _tag = "BackBlur")
//    {
//        var simlateObj = new GameObject("simlateObj_" + _tag);
//        SimlateObj s = simlateObj.AddComponent<SimlateObj>();
//        s.toSimlateObj = _render.gameObject;
//        simlateObj.transform.parent = null;
//        simlateObj.transform.position = simlateObj.transform.eulerAngles = Vector3.up * rad.x;
//        simlateObj.transform.DOMoveY(rad.y, dur).OnUpdate(() => {
//            callback(_render.material, simlateObj.transform.position.y);
//        }).OnComplete(() => {
//            UnityEngine.Object.DestroyImmediate(simlateObj);
//        }).SetEase(Ease.Linear);
//    }
//}
