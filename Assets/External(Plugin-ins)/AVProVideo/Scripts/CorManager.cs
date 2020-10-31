using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region using UnityEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif
#endregion

namespace RenderHeads.Media.AVProVideo
{
    public class CorManager : MonoBehaviour
    {
        private static bool initialized;
        public class TaskState
        {
            public bool Running
            {
                get
                {
                    return running;
                }
            }

            public bool Paused
            {
                get
                {
                    return paused;
                }
            }
            public delegate void FinishedHandler(bool manual);
            public event FinishedHandler Finished;

            private IEnumerator coroutine;
            private bool running;
            private bool paused;
            private bool stopped;

            public TaskState(IEnumerator c)
            {
                coroutine = c;
            }

            public void Pause()
            {
                paused = true;
            }

            public void Unpause()
            {
                paused = false;
            }

            public Coroutine Start()
            {
                running = true;
                return singleton.StartCoroutine(CallWrapper());
            }

            public void Stop()
            {
                stopped = true;
                running = false;
            }

            private IEnumerator CallWrapper()
            {
                yield return null;
                IEnumerator e = coroutine;
                while (running)
                {
                    if (paused)
                        yield return null;
                    else
                    {
                        if (e != null && e.MoveNext())
                        {
                            yield return e.Current;
                        }
                        else
                        {
                            running = false;
                        }
                    }
                }

                FinishedHandler handler = Finished;
                if (handler != null)
                    handler(stopped);
            }
        }

        private static CorManager singleton;

        public static CorManager Instance
        {
            get
            {
                Initialize();
                return singleton;
            }
        }
        private void Awake()
        {
            if (gameObject.scene.name != "DontDestroyOnLoad")
            {
                DontDestroyOnLoad(this.gameObject);
            }
            initialized = true;
            singleton = this;
        }
        public void SayHello()
        {
            Debug.LogWarningFormat("[{0}] say: Hello，World !!", this);
        }
        private static void Initialize()
        {
            if (!initialized)
            {

                if (!Application.isPlaying)
                    return;
                initialized = true;
                var g = new GameObject("CorManager_RenderHeads.Media.AVProVideo");
                singleton = g.AddComponent<CorManager>();
            }
        }
        private Dictionary<IEnumerator, TaskState> taskStateDict = new Dictionary<IEnumerator, TaskState>();
        public TaskState CreateTask(IEnumerator coroutine)
        {
            if (singleton == null)
            {
                GameObject go = new GameObject("CorManager_RenderHeads.Media.AVProVideo");
                singleton = go.AddComponent<CorManager>();
            }
            if (taskStateDict.TryGet(coroutine) == null)
                taskStateDict.Add(coroutine, new TaskState(coroutine));

            return taskStateDict.TryGet(coroutine);
        }
        /// <summary>
        /// 功能未实现
        /// </summary>
        /// <param name="coroutine"></param>
        public Coroutine CreateAndExecuteTask(IEnumerator coroutine)
        {
            TaskState taskState = CreateTask(coroutine);
            return taskState.Start();
        }
        public void StopAllTasks()
        {

        }
        public void StopTask(IEnumerator coroutine)
        {
            if (taskStateDict.TryGet(coroutine) == null)
                return;
            else
                taskStateDict.TryGet(coroutine).Stop();
        }

        /// <summary>
        ///  功能未实现
        /// </summary>
        /// <param name="coroutine"></param>
        public void StopTask(Coroutine coroutine)
        {
            //if (taskStateDict.TryGet(coroutine) == null)
            //    return;
            //else
            //    taskStateDict.TryGet(coroutine).Stop();
        }
        private void TaskFinished(bool manual)
        {

        }

        #region UNITY_EDITOR
#if UNITY_EDITOR
        [CustomEditor(typeof(CorManager))]
        public class CorManagerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                CorManager manager = target as CorManager;

                if (manager == null)
                {
                    return;
                }
                if (!EditorApplication.isPlaying)
                {
                    return;
                }
                EditorGUILayout.LabelField("注册的 delayActionDict：" + manager.taskStateDict.Count.ToString());
             
                this.Repaint();
            }
        }
#endif
        #endregion

    }

}