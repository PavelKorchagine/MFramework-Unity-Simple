///////////////////////////////////////////////////////////////////////////////
// Copyright 2017-2019  Vrtist Technology Co., Ltd. All Rights Reserved.
// File:  
// Author: 
// Date:  
// Discription:  
/////////////////////////////////////////////////////////////////////////////// 

#define HTCVIVE
#define PC
#define OCULUS_QUEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
#region UNITY_EDITOR
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion
using MFramework_Unity.InputSystem;

namespace MFramework_Unity
{

    /*
     * 在代码中使用时如何开启某个Layers？
     * LayerMask mask = 1 << 你需要开启的Layers层。
     * LayerMask mask = 0 << 你需要关闭的Layers层。
     * 举几个个栗子：
     * LayerMask mask = 1 << 2; 表示开启Layer2。
     * LayerMask mask = 0 << 5;表示关闭Layer5。
     * LayerMask mask = 1<<2|1<<8;表示开启Layer2和Layer8。
     * LayerMask mask = 0<<3|0<<7;表示关闭Layer3和Layer7。
     * 上面也可以写成：
     * LayerMask mask = ~（1<<3|1<<7）;表示关闭Layer3和Layer7。
     * LayerMask mask = 1<<2|0<<4;表示开启Layer2并且同时关闭Layer4.
     */

    /// <summary>
    /// abstract
    /// </summary>
    public class PCMouseInputModule : BaseStandaloneInputModule
    {

        protected override void Start()
        {
            base.Start();
            _Ray.isDebugDrawLine = true;
        }

        protected override void ExecuteHandler(GameObject go)
        {
            base.ExecuteHandler(go);
        }

        /// <summary>
        /// Start
        /// </summary>
        protected override void Update()
        {
            if (GetMainCamera() == null)
                return;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WEBGL || UNITY_ANDROID

            // 真实物体交互
            rayCast = _Ray.GetMouseRayCast(mainCamera, rayDistance, realRayPointLayerMask);
            if (rayCast.RealRay.direction != Vector3.zero && inputStick) inputStick.transform.forward = rayCast.RealRay.direction;

            //inputStick.transform.forward = rayCast.RealRay.direction;
            ExecuteHandler(rayCast.GameObjRayCast);
            objRayCast = rayCast.GameObjRayCast;
            // 获取虚拟坐标
            virtualRayCast = _Ray.GetMouseVirtualRayCast(mainCamera, 100, virtualPointLayerMask);
            if (virtualRayCast.IsRayCast)
            {
                virtualPoint = virtualRayCast.Vector3RayCastPoint;
                if (inputStick) inputStick.SetCursorPostion(virtualPoint);
            }

#endif
        }

    }

}