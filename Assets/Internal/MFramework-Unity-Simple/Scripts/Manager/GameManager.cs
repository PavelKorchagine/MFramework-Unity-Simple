using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFramework_Unity;
using MFramework_Unity.Tools;
using MFramework_Unity.InputSystem;

namespace MFramework_Unity_Simple
{
    public class GameManager : BaseManager
    {
        protected GameFacade gameFacade;
        public BaseGameModule[] modules;

        public override void OnInit(params object[] paras)
        {
            base.OnInit(paras);
            gameFacade = facade as GameFacade;
            modules = gameObject.GetComponentsRealInChildren<BaseGameModule>(true, true, true);

            foreach (var item in modules)
            {
                item.OnInit(gameFacade, this);
            }
        }

        public override void OnInit()
        {
            base.OnInit();
            foreach (var item in modules)
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