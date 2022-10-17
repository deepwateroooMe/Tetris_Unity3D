using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Framework.Util;
using UnityEngine;

namespace HotFix.Control {

    public class EventManager : Singleton<EventManager> { // Singleton: 晚点儿把重构前的游戏逻辑理解清楚了再优化架构小细节
        private const string TAG = "EventManager";

        private delegate void EventListener(EventInfo el);               // 普通代理
        public delegate void EventListener<T>(T el) where T : EventInfo; // 泛型代理，带一个EventInfo参数
        // Dictionary<System.Type, List<EventListener>> eventListeners;
        
        Dictionary<System.Type, EventListener> delegatesMap = new Dictionary<System.Type, EventListener>();  // eventListeners;     
        Dictionary<System.Delegate, EventListener> delegateLookupMap = new Dictionary<System.Delegate, EventListener>();

        public bool isCleanedUp() {
            return (delegatesMap.Count == 0 && delegateLookupMap.Count == 0);
        }
        public void cleanUpLists() {
            if (delegatesMap != null && delegatesMap.Count > 0) {
                delegatesMap.Clear();
                // delegatesMap = new Dictionary<System.Type, EventListener>();  // eventListeners;
            }
            if (delegateLookupMap != null && delegateLookupMap.Count > 0) {
                delegateLookupMap.Clear();
                // delegateLookupMap = new Dictionary<System.Delegate, EventListener>();
            }
        }
        
        // public void RegisterListener<T>(System.Action<T> listener) where T : EventInfo { // with parameter
        public void RegisterListener<T>(EventListener<T> listener) where T : EventInfo { // 一个意思
            Debug.Log(TAG + ": RegisterListener()");

            EventListener internalDelegate = (el) => { listener((T)el); };

            // Debug.Log(TAG + " delegatesMap.Count before: " + delegatesMap.Count); 
            // Debug.Log(TAG + " delegateLookupMap.Count before: " + delegateLookupMap.Count);

            if (delegateLookupMap.ContainsKey(listener) && delegateLookupMap[listener] == internalDelegate) // 已经存在，所以返回
                return;
            delegateLookupMap[listener] = internalDelegate;
            EventListener  tmpDelegate;
            if (delegatesMap.TryGetValue(typeof(T), out tmpDelegate)) {
                delegatesMap[typeof(T)] = tmpDelegate += internalDelegate;
            } else
                delegatesMap[typeof(T)] = internalDelegate;

            Debug.Log(TAG + " delegatesMap.Count after: " + delegatesMap.Count); 
            Debug.Log(TAG + " delegateLookupMap.Count after: " + delegateLookupMap.Count);

            
            // System.Type eventType = typeof(T);
            // if (eventListeners == null) {
            //     eventListeners = new Dictionary<System.Type, List<EventListener>>();
            // }
            // List<EventListener> thisListenerList = null;
            // EventListener wrapper = (ei) => { listener((T)ei); };     // wrapper string: tetris3d.EventManager+EventListener
            // if (eventListeners.TryGetValue(eventType, out thisListenerList)) {
            //     Debug.Log(TAG + " eventListeners[eventType].Count before add: " + eventListeners[eventType].Count); 
            //     foreach (EventListener el in thisListenerList) { // 想要剔除重复
            //         if (el == wrapper)
            //             return;
            //     }
            //     thisListenerList.Add(wrapper);
            // } else {
            //     thisListenerList = new List<EventListener>();
            //     thisListenerList.Add(wrapper);
            // }
            // eventListeners[eventType] = thisListenerList;
            // Debug.Log(TAG + " eventListeners[eventType].Count after Add: " + eventListeners[eventType].Count); 
            
            // if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null) {
            //     eventListeners[eventType] = new List<EventListener>();
            // } 
            // EventListener wrapper = (ei) => { listener((T)ei); };     // wrapper string: tetris3d.EventManager+EventListener
            // foreach (EventListener el in eventListeners[eventType]) { // 想要剔除重复
            //     if (el == wrapper)
            //         return;
            // }
            // eventListeners[eventType].Add(wrapper); // 这里需要检查重复性
        }
        
        public void UnregisterListener<T>(EventListener<T> listener) where T : EventInfo { // System.Action 这里并没有能真正移除掉监听，需要再理解、更改
            Debug.Log(TAG + ": UnregisterListener()"); 
            // Debug.Log(TAG + " delegatesMap.Count before: " + delegatesMap.Count); 
            // Debug.Log(TAG + " delegateLookupMap.Count before: " + delegateLookupMap.Count);

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

            Debug.Log(TAG + " delegatesMap.Count after: " + delegatesMap.Count); 
            Debug.Log(TAG + " delegateLookupMap.Count after: " + delegateLookupMap.Count);


            // if (delegatesMap.Count > 0 || delegateLookupMap.Count > 0) {
                
            // }

            // if (Instance == null) return;
            // System.Type eventType = typeof(T);
            // Debug.Log(TAG + " (eventListeners == null): " + (eventListeners == null)); 
            // if (eventListeners != null) {
            //     Debug.Log(TAG + " (eventListeners[eventType] == null): " + (eventListeners[eventType] == null)); 
            // }
            // EventListener wrapper = (ei) => { listener((T)ei); };
            // List<EventListener> tmpDelegateList;
            // Debug.Log(TAG + " eventListeners[eventType].Count before: " + eventListeners[eventType].Count); 
            // if (eventListeners.TryGetValue(eventType, out tmpDelegateList)) {
            //     foreach (EventListener el in tmpDelegateList) {  // method 1, try another one too
            //         if (el == wrapper)
            //             tmpDelegateList.Remove(el);
            //     }
            //     if (tmpDelegateList == null)
            //         eventListeners.Remove(eventType);
            //     else
            //         eventListeners[eventType] = tmpDelegateList;
            // } 
            // Debug.Log(TAG + " eventListeners[eventType].Count after: " + eventListeners[eventType].Count); 

            
            // System.Type eventType = typeof(T);
            // EventListener wrapper = (ei) => { listener((T)ei); };
            
            // Debug.Log(TAG + " (eventListeners == null): " + (eventListeners == null)); 
            // if (eventListeners != null) {
            //     Debug.Log(TAG + " (eventListeners[eventType] == null): " + (eventListeners[eventType] == null)); 
            // }

            // if (eventListeners != null && eventListeners[eventType] != null) {
            //     Debug.Log(TAG + " eventListeners[eventType].Count before: " + eventListeners[eventType].Count); 
            //     // eventListeners[eventType].Remove(wrapper);
            //     eventListeners[eventType].Remove((ei) => {listener((T)ei); } );
            //     Debug.Log(TAG + " eventListeners[eventType].Count after: " + eventListeners[eventType].Count); 
            // }

            // Debug.Log(TAG + " (eventType == null): " + (eventType == null));
            // Debug.Log(TAG + " (eventListeners == null): " + (eventListeners == null));
            // if (eventListeners != null)
            //     Debug.Log(TAG + " eventListeners.ContainsKey(eventType): " + eventListeners.ContainsKey(eventType));
            // if (eventListeners.ContainsKey(eventType)) {
            //     Debug.Log(TAG + " eventListeners[eventType].Count: " + eventListeners[eventType].Count);
            // }
        }
        
        public void FireEvent(EventInfo eventInfo) {
            // Debug.Log(TAG + ": FireEvent()"); 

            EventListener tmpDelegate;
            if (delegatesMap.TryGetValue(eventInfo.GetType(), out tmpDelegate)) {
                tmpDelegate.Invoke(eventInfo);
            }
            
            // System.Type trueEventInfoClass = eventInfo.GetType();
            // if (eventListeners == null || eventListeners[trueEventInfoClass] == null) { 
            //     return;
            // }
            // foreach (EventListener el in eventListeners[trueEventInfoClass]) {
            //     el(eventInfo); 
            // }
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
    }
}
