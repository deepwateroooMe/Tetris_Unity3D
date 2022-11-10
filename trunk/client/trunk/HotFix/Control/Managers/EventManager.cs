using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using HotFix.Control;

namespace HotFix.Control {

    public class EventManager : SingletonMono<EventManager> { 
        private const string TAG = "EventManager";

// 普通代理: 其实是说,如果触发了el事件,回调这个EventListener(EventInfo el)方法,是触发事件之后的回调;是以参数和返回类型来区分代理之间的不同的
        public delegate void EventListener(EventInfo el); // 普通代理: 给出普通代理的定义               

// 泛型代理，带一个EventInfo参数,这样就可以处理所有自定义事件了继承子类了
        public delegate void EventListener<T>(T el) where T : EventInfo; // 转化为泛型代理

// 字典:它的銉是以热更新工程中定义过的EventInfo的子类类型相区分;这个字典的键的个数会比较少(因为整个项目中一个类型只占一个键与条款)
        public Dictionary<System.Type, EventListener> delegatesMap;

// 字典:是对不同类型事件代理的管理;热更新项目中定义过的不同事件代理作键(EventListener<T>)(那是这个字典的键是以代理相区分,就是不同代理方法的参数与返回类型相区分,应该还有其它),
// 值为一个这样的代理EventListener<T>;这个字典键的个数会很多,基本每注册一个回调就填加一个键
        public Dictionary<System.Delegate, EventListener> delegateLookupMap;

        private Vector3 delta;

        private TetrominoSpawnedEventInfo spawnedInfo;
        private TetrominoMoveEventInfo moveInfo;
        private TetrominoRotateEventInfo rotateInfo;
        private TetrominoValidMMInfo validInfo; // valid Move Rotate
        private TetrominoLandEventInfo landInfo;
        private TetrominoChallLandInfo challLandInfo;
        private UndoLastTetrominoInfo undoInfo;
        
        private GameEnterEventInfo enterInfo;
        private GamePauseEventInfo pauseInfo;
        private GameResumeEventInfo resumeInfo;
        private CanvasToggledEventInfo canvasInfo;
        private CubesMaterialEventInfo cubeMatInfo;
        
        public void Awake() {
            // Debug.Log(TAG + " Awake");
            delegatesMap = new Dictionary<System.Type, EventListener>();  
            delegateLookupMap = new Dictionary<System.Delegate, EventListener>();
            delta = Vector3.zero;

            undoInfo = new UndoLastTetrominoInfo();
            spawnedInfo = new TetrominoSpawnedEventInfo();
            moveInfo = new TetrominoMoveEventInfo();
            rotateInfo = new TetrominoRotateEventInfo();
            landInfo = new TetrominoLandEventInfo();
            challLandInfo = new TetrominoChallLandInfo();
            
            canvasInfo = new CanvasToggledEventInfo();
            enterInfo = new GameEnterEventInfo();
            pauseInfo = new GamePauseEventInfo();
            resumeInfo = new GameResumeEventInfo();
            cubeMatInfo = new CubesMaterialEventInfo();
            
            validInfo = new TetrominoValidMMInfo();
        }

        public void RegisterListener<T>(EventListener<T> listener) where T : EventInfo { 
            // Debug.Log(TAG + ": RegisterListener(): T: " + typeof(T)); 
            EventListener internalDelegate = (el) => { // 注意: 即便不同类(不同对象)对所感兴趣的同类事件的监听回调方法同名,这里仍然会被定义为不同的键
                // Debug.Log(TAG + " RegisterListener<T>:　(typeof(T)): " + typeof(T)); // EventManager (typeof(T)): HotFix.Control.CanvasToggledEventInfo
                listener((T)el);
            };

            // 代理类型(EventListener<T>)已经存在，且值(所有注册过的回调方法,唯一一个)相等，那就不需要再做什么,直接返回
// TODO:我认为我写得很有条理很干净的游戏逻辑可以跳过这一步,可以晚点儿再测一下            
            if (delegateLookupMap.ContainsKey(listener) && delegateLookupMap[listener] == internalDelegate) {
                Debug.Log(TAG + " RegisterListener<T> CONTAINED & RETURNS T.TAG: " + typeof(T)); // 可是为什么我这个消息没有打印出来呢?
                return;
            }
            delegateLookupMap[listener] = internalDelegate; 

            EventListener  tmpDelegate;
            if (delegatesMap.TryGetValue(typeof(T), out tmpDelegate)) {    // 如果存在这个类型T的键，返回true，并且值的内容写入在tmpDelegate里面
                delegatesMap[typeof(T)] = tmpDelegate += internalDelegate; // 那么对于当前键，其值的内容再添加一个新的代理监听监听回调(注册回调方法的时候也是这么写的)
            } else
                delegatesMap[typeof(T)] = internalDelegate;
            Debug.Log(TAG + " RegisterListener() delegatesMap.Count after: " + delegatesMap.Count
                      + "; delegateLookupMap.Count after: " + delegateLookupMap.Count); 
        }
        
        public void UnregisterListener<T>(EventListener<T> listener) where T : EventInfo { // System.Action 这里并没有能真正移除掉监听，需要再理解、更改
            // Debug.Log(TAG + ": UnregisterListener()"); 
            EventListener internalDelegate;
            if (delegateLookupMap.TryGetValue(listener, out internalDelegate)) {
                EventListener tmpDelegate;
                if (delegatesMap.TryGetValue(typeof(T), out tmpDelegate)) {
                    tmpDelegate -= internalDelegate;
                    if (tmpDelegate == null)
                        delegatesMap.Remove(typeof(T));
                    else
                        delegatesMap[typeof(T)] = tmpDelegate;
                }
                delegateLookupMap.Remove(listener);
            }
            Debug.Log(TAG + " UnregisterListener() delegatesMap.Count after: " + delegatesMap.Count
                      + "; delegateLookupMap.Count after: " + delegateLookupMap.Count); 
        }
        
        public void FireEvent(string type) { // 事件:不带任何增量信息的
            Debug.Log(TAG + ": FireEvent() type: " + type); 
            switch (type) {
            case "entergame":
                FireEvent(enterInfo);
                return;
            case "pausegame":
                FireEvent(pauseInfo);
                return;
            case "resumegame":
                FireEvent(resumeInfo);
                return;
            case "undo":
                FireEvent(undoInfo);
                return;
            case "land":
                FireEvent(landInfo);
                return;
            case "challLand":
                FireEvent(challLandInfo);
                return;
            case "canvas":
                FireEvent(canvasInfo);
                return;
            case "cubesMat":
                FireEvent(cubeMatInfo);
                return;
            }
        }
        
        public void FireEvent(string type, string isMove, Vector3 delta) { 
            Debug.Log(TAG + "FireEvent() type: " + type + "; isMove: " + isMove);
            MathUtilP.print(delta);
            validInfo.type = isMove;
            validInfo.delta = delta;
            FireEvent(validInfo); // "validMR"
        }

        public void FireEvent(string type, Vector3 delta) {
            Debug.Log(TAG + ": FireEvent() type + delta. type: " + type); 
            MathUtilP.print(delta);
            switch (type) {
            case "spawned":
                spawnedInfo.delta = delta;
                FireEvent(spawnedInfo);
                return;
            case "move":
                moveInfo.delta = delta;
                FireEvent(moveInfo);
                return;
            case "rotate":
                rotateInfo.delta = delta;
                FireEvent(rotateInfo);
                return;
            }
        }
        
        public void FireEvent(EventInfo eventInfo) {
            EventListener tmpDelegate;
            if (delegatesMap.TryGetValue(eventInfo.GetType(), out tmpDelegate)) 
                tmpDelegate.Invoke(eventInfo);
        }
        
        public delegate void onSwapButtonClickedDelegate(); // for swapping preview Tetrominos
        public static event onSwapButtonClickedDelegate SwapButtonClicked;
        public void onSwapButtonClicked() { 
            // Debug.Log(TAG + ": onSwapButtonClicked()"); 
            if (SwapButtonClicked != null) {
                SwapButtonClicked();
            }
        }
        
        public delegate void onUndoButtonClickedDelegate(); // for testing undo feature      
        public static event onUndoButtonClickedDelegate UndoButtonClicked;
        public void onUndoButtonClicked() { 
            // Debug.Log(TAG + ": onUndoButtonClicked()"); 
            if (UndoButtonClicked != null) {
                UndoButtonClicked();
            }
        }

        public bool isCleanedUp() {
            return (delegatesMap.Count == 0 && delegateLookupMap.Count == 0);
        }
        
// TODO: 在必要的时候,作必要的清理,这里是不需要再作检查的            
        public void cleanUpLists() {
            if (delegatesMap != null && delegatesMap.Count > 0) 
                delegatesMap.Clear();
            if (delegateLookupMap != null && delegateLookupMap.Count > 0) 
                delegateLookupMap.Clear();
        }
    }
}