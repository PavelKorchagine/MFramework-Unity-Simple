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
    public class CoroutineTaskManager : MonoBehaviour
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

        private static CoroutineTaskManager singleton;

        public static CoroutineTaskManager Instance
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
                var g = new GameObject("CoroutineTaskManager_RenderHeads.Media.AVProVideo");
                singleton = g.AddComponent<CoroutineTaskManager>();
            }
        }
        private Dictionary<IEnumerator, TaskState> taskStateDict = new Dictionary<IEnumerator, TaskState>();
        public TaskState CreateTask(IEnumerator coroutine)
        {
            if (singleton == null)
            {
                GameObject go = new GameObject("CoroutineTaskManager_RenderHeads.Media.AVProVideo");
                singleton = go.AddComponent<CoroutineTaskManager>();
            }
            if (taskStateDict.TryGet(coroutine) == null)
            {
                taskStateDict.Add(coroutine, new TaskState(coroutine));
            }

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
            foreach (var item in taskStateDict.Values)
            {
                item.Stop();

            }
            Instance.StopAllCoroutines();
            taskStateDict.Clear();
        }
        public void StopTask(IEnumerator coroutine)
        {
            if (taskStateDict.TryGet(coroutine) == null)
                return;
            else
            {
                taskStateDict.TryGet(coroutine).Stop();
                taskStateDict.Remove(coroutine);
            }
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

            Instance.StopCoroutine(coroutine);
        }
        private void TaskFinished(bool manual)
        {

        }

        #region UNITY_EDITOR
#if UNITY_EDITOR
        [CustomEditor(typeof(CoroutineTaskManager))]
        public class CoroutineTaskManagerEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();
                CoroutineTaskManager manager = target as CoroutineTaskManager;

                if (manager == null)
                {
                    return;
                }
                if (!EditorApplication.isPlaying)
                {
                    return;
                }
                EditorGUILayout.LabelField("coroutines：" + manager.taskStateDict.Count.ToString());
                foreach (var item in manager.taskStateDict)
                {
                   // EditorGUILayout.LabelField("-" + item.ToString());
                }
             
                this.Repaint();
            }
        }
#endif
        #endregion

    }

}