using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFramework_Unity;
using MFramework_Unity.Tools;
using MFramework_Unity.InputSystem;

namespace MFramework_Unity_Simple
{
    public class GameFacade : Facade
    {
        public static GameFacade Facade {
            get { return Instance as GameFacade; }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}