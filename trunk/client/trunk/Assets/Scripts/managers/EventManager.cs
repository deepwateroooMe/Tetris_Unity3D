using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using Framework.Util;

namespace tetris3d {

    // Unity 游戏框架搭建 (五) 简易消息机制, 再深理解一下
    // https://zhuanlan.zhihu.com/p/30978365
    // Something/reason I thought could be why I failed the first version of the EventManager
    
    public class EventManager : Singleton<EventManager> { 
        private const string TAG = "EventManager";
        private static bool isFistTimeAwake = true;
        
        private delegate void EventListener(EventInfo el);               // 普通代理
        public delegate void EventListener<T>(T el) where T : EventInfo; // 泛型代理，带一个EventInfo参数
        
        Dictionary<System.Type, EventListener> delegatesMap = new Dictionary<System.Type, EventListener>();  // eventListeners;     
        Dictionary<System.Delegate, EventListener> delegateLookupMap = new Dictionary<System.Delegate, EventListener>();

        // private MethodInfo methodInfo;
        private void Awake() {
            // Debug.Log(TAG + ": Awake()"); 
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name);
            // Debug.Log(TAG + " isFistTimeAwake: " + isFistTimeAwake); 
            if (isFistTimeAwake) {
                EventManager.Instance = this; // no need this one
                isFistTimeAwake = false;
            }
        }
        private void OnEnable() {
            // Debug.Log(TAG + ": OnEnable()");
            // Debug.Log(TAG + " gameObject.name: " + gameObject.name);
        }
        // private void OnDisable() { // has NOT really do anything because when OnDisable() OnDestroy() the registered events have NOT been cleaned up yet, so could NOT disX()
        //     Debug.Log(TAG + ": OnDisable()"); 
        //     Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
        // }
        // private void OnDestroy() {
        //     Debug.Log(TAG + ": OnDestroy()");
        //     Debug.Log(TAG + " gameObject.name: " + gameObject.name); 
        //     // Debug.Log(TAG + " delegatesMap.Count: " + delegatesMap.Count); 
        //     // Debug.Log(TAG + " delegateLookupMap.Count: " + delegateLookupMap.Count);
        // }        

        public int getDelegatesMapCount () {
            return delegatesMap.Count;
        }
        public int getDelegateLookupMapCount () {
            return delegateLookupMap.Count;
        }
        public bool isCleanedUp() {
            Debug.Log(TAG + ": isCleanedUp()"); 
            return (delegatesMap.Count == 0 && delegateLookupMap.Count == 0);
        }
        public void cleanUpLists() {
            Debug.Log(TAG + ": cleanUpLists()"); 
                if (delegatesMap != null && delegatesMap.Count > 0) 
                    delegatesMap.Clear();
                if (delegateLookupMap != null && delegateLookupMap.Count > 0) 
                    delegateLookupMap.Clear();
        }
        
        public void RegisterListener<T>(EventListener<T> listener) where T : EventInfo { // 一个意思
            Debug.Log(TAG + ": RegisterListener()");
            // Debug.Log(TAG + " delegatesMap.Count before: " + delegatesMap.Count); 
            // Debug.Log(TAG + " delegateLookupMap.Count before: " + delegateLookupMap.Count);
            
            // methodInfo = listener; // EventListener; // work on this one later
            // var parameterType = methodInfo.GetParameters().Select(pi ==> pi.ParameterType);
            // Debug.Log(TAG + " parameterType.ToString(): " + parameterType.ToString()); 
            // Debug.Log(TAG + ": delegate info:"); 
            // // DelegateHelper.ReadDelegateSignature("EventManager.EventListener<T>");
            // DelegateHelper.ReadDelegateSignature();
            
            // Debug.Log(TAG + " listener: " + listener);   // tetris3d.EventManager+EventListener`1[tetris3d.TetrominoMoveEventInfo] // Debug.Log(TAG + " typeof(T): " + typeof(T)); // tetris3d.TetrominoMoveEventInfo
            EventListener internalDelegate = (el) => { listener((T)el); };
            
            if (delegateLookupMap.ContainsKey(listener) && delegateLookupMap[listener] == internalDelegate) { // 已经存在，所以返回
                return;
            }
            delegateLookupMap[listener] = internalDelegate;
            EventListener  tmpDelegate;
            if (delegatesMap.TryGetValue(typeof(T), out tmpDelegate)) {
                delegatesMap[typeof(T)] = tmpDelegate += internalDelegate;

                Debug.Log(TAG + " typeof(T): " + typeof(T)); // tetris3d.PauseGameEventInfo etc
            } else
                delegatesMap[typeof(T)] = internalDelegate;

            // Debug.Log(TAG + " delegatesMap.Count after: " + delegatesMap.Count); 
            // Debug.Log(TAG + " delegateLookupMap.Count after: " + delegateLookupMap.Count);
        }
        
        public void UnregisterListener<T>(EventListener<T> listener) where T : EventInfo { // System.Action 这里并没有能真正移除掉监听，需要再理解、更改
            Debug.Log(TAG + ": UnregisterListener()"); 
            // Debug.Log(TAG + " delegatesMap.Count before: " + delegatesMap.Count); 
            // Debug.Log(TAG + " delegateLookupMap.Count before: " + delegateLookupMap.Count);

            // Debug.Log(TAG + " listener: " + listener); 
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

            // Debug.Log(TAG + " delegatesMap.Count after: " + delegatesMap.Count); 
            // Debug.Log(TAG + " delegateLookupMap.Count after: " + delegateLookupMap.Count);

            if ((delegatesMap.Count <= 2 && delegatesMap.Count > 0) || (delegateLookupMap.Count <= 2 && delegateLookupMap.Count > 0)) {
                Debug.Log(TAG + ": having NOT unregistered System.Type s: "); 
                foreach (System.Type type in delegatesMap.Keys) {
                    Debug.Log(TAG + " type: " + type); 
                }
            }
        }
        
        public void FireEvent(EventInfo eventInfo) {
            // Debug.Log(TAG + ": FireEvent()");
            // Debug.Log(TAG + " eventInfo.GetType().ToString(): " + eventInfo.GetType().ToString()); 

            // EventListener tmpDelegate;  // 不需要这些
            // if (delegatesMap.TryGetValue(eventInfo.GetType(), out tmpDelegate)) {
            //     tmpDelegate.Invoke(eventInfo);
            // }

            EventListener tmpDelegate;
            bool tmp = delegatesMap.TryGetValue(eventInfo.GetType(), out tmpDelegate);
            // Debug.Log(TAG + " tmp: " + tmp); 
            if (tmp) {
                tmpDelegate.Invoke(eventInfo);
            }
        }
    }
}