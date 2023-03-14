// copyright (c) https:// github.com/Bian-Sh
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.LowLevel;
namespace zFramework.Misc {
    // 是一个对Unity 多线程进行管理的库。管理的原理也很简单：需要回调到主线程的本地缓存，异步线程执行的本地缓存，向主线程的投放同步更新等。因为是多线程环境，很多地主要上锁
    public static class Loom { 

        static SynchronizationContext context; // 标记，记住主线程 
        static readonly ConcurrentQueue<Action> tasks = new ConcurrentQueue<Action>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install() {
            context = SynchronizationContext.Current;
            
#region 使用 PlayerLoop 在 Unity 主线程的 Update 中更新本任务同步器
            var playerloop = PlayerLoop.GetCurrentPlayerLoop();
            var loop = new PlayerLoopSystem { // 这个东西，里面有很多层嵌套循环，像个树，只是是数组嵌套而成的树
                type = typeof(Loom),
                updateDelegate = Update
            };
            // 1. 找到 Update Loop System, 具体的是，找到它所在的数组中的下标
            int index = Array.FindIndex(playerloop.subSystemList, v => v.type == typeof(UnityEngine.PlayerLoop.Update));
            // 2.  将咱们的 loop 插入到 Update loop 中
            var updateloop = playerloop.subSystemList[index]; // 这个东西，仍然是嵌套循环的，在它的子系统中，通过数组＝》链表＝》数组，添加一个元素
            var temp = updateloop.subSystemList.ToList(); // 通过数组＝》链表＝》数组，添加一个元素
            temp.Add(loop);
            updateloop.subSystemList = temp.ToArray();
            playerloop.subSystemList[index] = updateloop; // 并重新，把更新后的值赋回去
            // 3. 设置自定义的 Loop 到 Unity 引擎。设置成自己自定义的
            PlayerLoop.SetPlayerLoop(playerloop);
#if UNITY_EDITOR
            // 4. 已知：编辑器停止 Play 我们自己插入的 loop 依旧会触发，进入或退出Play 模式先清空 tasks
            EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged; 
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
            static void EditorApplication_playModeStateChanged(PlayModeStateChange obj) { 
                if (obj == PlayModeStateChange.ExitingEditMode ||
                    obj == PlayModeStateChange.ExitingPlayMode) {
                    // 清空任务列表
                    while (tasks.TryDequeue(out _)) { }
                }
            }
#endif
#endregion
        }

#if UNITY_EDITOR
        // 5. 确保编辑器下推送的事件也能被执行
        [InitializeOnLoadMethod]
        static void EditorForceUpdate() {
            Install();
            EditorApplication.update -= ForceEditorPlayerLoopUpdate;
            EditorApplication.update += ForceEditorPlayerLoopUpdate;
            void ForceEditorPlayerLoopUpdate() {
                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) {
                    // Not in Edit mode, don't interfere
                    return;
                }
                Update();
            }
        }
#endif
//  在主线程中执行
        public static void Post(Action task) {
            if (SynchronizationContext.Current == context) {
                task?.Invoke(); // 如果是主线程，就可以直接调用
            } else { // 否则，加入缓存同步队列中
                tasks.Enqueue(task);
                if (tasks.Count > 50) {
                    Debug.LogWarning($"{nameof(Loom)}:请控制消息推送速率，消息队列中未处理的数据量已超 50 个 {tasks.Count} ！");
                }
            }
        }
        public static void PostNext(Action action) => tasks.Enqueue(action); // 加入到了主线程的缓存同步队列，FIFO
        static void Update() { // 这里，就确定，这一定是主线程了？
            while (tasks.TryDequeue(out var task)) {
                try {
                    task?.Invoke();
                }
                catch (Exception e) {
                    Debug.Log($"{nameof(Loom)}:  封送的任务执行过程中发现异常，请确认: {e}");
                }
            }
        }
    }
}