using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFramework_Unity;
using MFramework_Unity.Tools;
using MFramework_Unity.InputSystem;

namespace MFramework_Unity_Simple
{
    public class UIManager : BaseManager
    {
        protected GameFacade gameFacade;
        public BaseUIModule[] panels;

        public override void OnInit(params object[] paras)
        {
            base.OnInit(paras);
            gameFacade = facade as GameFacade;
            panels = gameObject.GetComponentsRealInChildren<BaseUIModule>(true, true, true);

            foreach (var item in panels)
            {
                item.OnInit(gameFacade, this);
            }
        }

        public override void OnInit()
        {
            base.OnInit();
            foreach (var item in panels)
            {
                item.OnInit();
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();

        }
    }
}