using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using HotFix.Control;

namespace HotFix.Control {

// TOGO: 对MoveCanvas RotateCanvas中两组按钮共十个的管理设计不够理想;
// 因热更新中的游戏框架依赖于这个管理器,暂且如此运行,过程中会试图优化对那两三组按钮的管理    
    public class EventManager : SingletonMono<EventManager> { 
        private const string TAG = "EventManager";

// 普通代理: 其实是说,如果触发了el事件,回调这个EventListener(EventInfo el)方法,是触发事件之后的回调;是以参数和返回类型来区分代理之间的不同的
        public delegate void EventListener(EventInfo el);                
// 泛型代理，带一个EventInfo参数,这样就可以处理所有自定义事件了继承子类了
        public delegate void EventListener<T>(T el) where T : EventInfo; 

// 这里上下两个字典的键,好像被我写反了.....        
// 字典:它的銉是以热更新工程中定义过的EventInfo的子类相区分;这个字典的键的个数会比较多,因为热更新工程中所自定义的类比较多?确认一下
        public Dictionary<System.Type, EventListener> delegatesMap;
// 字典:是对不同类型事件代理的管理;热更新项目中定义过的不同事件代理作键(EventListener<T>)(那是这个字典的键是以代理相区分,就是不同代理方法的参数与返回类型相区分), 值为一个这样的代理EventListener<T>;这个字典键的个数会略少
        public Dictionary<System.Delegate, EventListener> delegateLookupMap;

        private Vector3 delta;
        private TetrominoSpawnedEventInfo spawnedInfo;
        private TetrominoMoveEventInfo moveInfo;
        private TetrominoRotateEventInfo rotateInfo;
        private TetrominoLandEventInfo landInfo;

        private GameEnterEventInfo enterInfo;
        private CanvasToggledEventInfo canvasInfo;
        
        public void Awake() {
            Debug.Log(TAG + " Awake");
            delegatesMap = new Dictionary<System.Type, EventListener>();  // eventListeners;     
            delegateLookupMap = new Dictionary<System.Delegate, EventListener>();
            delta = Vector3.zero;
            spawnedInfo = new TetrominoSpawnedEventInfo();
            moveInfo = new TetrominoMoveEventInfo();
            rotateInfo = new TetrominoRotateEventInfo();
            landInfo = new TetrominoLandEventInfo();
            canvasInfo = new CanvasToggledEventInfo();
            enterInfo = new GameEnterEventInfo();
        }

        public void RegisterListener<T>(EventListener<T> listener) where T : EventInfo { 
            Debug.Log(TAG + ": RegisterListener()");

            EventListener internalDelegate = (el) => { listener((T)el); };

// 代理类型(EventListener<T>)已经存在，且值(所有注册过的回调方法,唯一一个)相等，那就不需要再做什么,直接返回
            if (delegateLookupMap.ContainsKey(listener) && delegateLookupMap[listener] == internalDelegate) 
                return;
// 还没有注册过这种代码的回调监听,就注册一个(唯一一个)            
            delegateLookupMap[listener] = internalDelegate; 
            EventListener  tmpDelegate;
            if (delegatesMap.TryGetValue(typeof(T), out tmpDelegate)) {    // 如果存在这个类型T的键，返回true，并且值的内容写入在tmpDelegate里面
                delegatesMap[typeof(T)] = tmpDelegate += internalDelegate; // 那么对于当前键，其值的内容再添加一个新的代理监听监听回调(注册回调方法的时候也是这么写的)
            } else
                delegatesMap[typeof(T)] = internalDelegate;

            Debug.Log(TAG + " RegisterListener() delegatesMap.Count after: " + delegatesMap.Count + "; delegateLookupMap.Count after: " + delegateLookupMap.Count); 
        }
        
        public void UnregisterListener<T>(EventListener<T> listener) where T : EventInfo { // System.Action 这里并没有能真正移除掉监听，需要再理解、更改
            Debug.Log(TAG + ": UnregisterListener()"); 
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
            Debug.Log(TAG + " delegatesMap.Count after: " + delegatesMap.Count + "; delegateLookupMap.Count after: " + delegateLookupMap.Count); 
        }
// 事件:不带任何增量信息的
        public void FireEvent(string type) {
            Debug.Log(TAG + ": FireEvent() type: " + type); 
            switch (type) {
            case "entergame":
                FireEvent(enterInfo);
                return;
            case "spawned":
                FireEvent(spawnedInfo);
                return;
            case "land":
                FireEvent(landInfo);
                return;
            case "canvas":
                FireEvent(canvasInfo);
                return;
            }
        }

        public void FireEvent(string type, Vector3 delta) {
            Debug.Log(TAG + ": FireEvent() type + delta. type: " + type); 
            switch (type) {
            case "spawned":
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
            case "land":
                FireEvent(landInfo);
                return;
            case "canvas":
                FireEvent(canvasInfo);
                return;
            }
        }
        
        public void FireEvent(EventInfo eventInfo) {
            Debug.Log(TAG + "FireEvent EventInfo.TAG: " + EventInfo.TAG);
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