using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Util;
using System;

// 这里就是用血淋淋的教训告诉你：必须得搞懂每一点的细节，要不然你去网上爬去搜搜看，什么时候能够搜得到答案！！！爱表哥，爱生活！！！
// 试问：游戏程序域里的这些方法，若是不包装好给热更新程序域用，热更新怎么使用这些方法呢？其它层级不是设置了四大类型，六个不同的适配器吗？
// 你怎么就这么确信热更新程序域能够直接调用到这些方法＞？
public static class Extension {

    // 删除所有子物体
    public static void DestroyAllChildren(this GameObject go) {
        List<GameObject> toDeleted = new List<GameObject>();
        int length = go.transform.childCount;
        for (int i = 0; i < length; i++) {
            var child = go.transform.GetChild(i);
            DontDestroyOnLoad donot = child.GetComponent<DontDestroyOnLoad>();
            if (donot == null) {
                toDeleted.Add(child.gameObject);
            }
        }
        foreach (var g in toDeleted) {
            GameObject.Destroy(g);
        }
    }

    // 立即删除
    public static void DestoryImmediateAllChildren(this GameObject go) {
        List<GameObject> toDeleted = new List<GameObject>();
        int length = go.transform.childCount;
        for (int i = 0; i < go.transform.childCount; i++) {
            var child = go.transform.GetChild(i);
            DontDestroyOnLoad donot = child.GetComponent<DontDestroyOnLoad>();
            if (donot == null) {
                toDeleted.Add(child.gameObject);
            }
        }
        foreach (var g in toDeleted) {
            GameObject.DestroyImmediate(g);
        }
    }

    // 设置Layer
    public static void SetLayerRecursively(this GameObject go, int layer) {
        if (!go)
            return;
        go.layer = layer;
        foreach (Transform child in go.transform) {
            child.gameObject.SetLayerRecursively(layer);
        }
    }

    // 设置Tag
    public static void SetTagRecursively(this GameObject go, string tag) {
        if (!go)
            return;
        go.tag = tag;
        foreach (Transform child in go.transform) {
            child.gameObject.SetTagRecursively(tag);
        }
    }

    // 得到Transform
    public static Transform transform(this GameObject go) {
        return go.gameObject.transform;
    }

    // 查找child物体
    public static GameObject FindChildByName(this GameObject parent, string name, bool logNotFound = true) {
        Transform result = parent.transform.Find(name);
        if (result != null) {
            return result.gameObject;
        } else {
            foreach (Transform childTrans in parent.transform) {
                GameObject go = FindChildByName(childTrans.gameObject, name, false);
                if (go != null) {
                    return go;
                }
            }
        }
        if (logNotFound) {
            DebugHelper.LogError(parent.name + " can't find " + name, false);
        }
        return null;
    }

    // 查找child 物体 Tag
    public static List<GameObject> FindChildrenWithTag(this GameObject go, string tag, bool recursion = false) {
        List<GameObject> result = new List<GameObject>();
        if (!go) {
            return result;
        }
        int length = go.transform.childCount;
        for (int i = 0; i < length; i++) {
            var child = go.transform.GetChild(i);
            if (recursion) {
                var cs = child.gameObject.FindChildrenWithTag(tag, recursion);
                result.AddRange(cs);
            }
        }
        if (go.tag == tag) {
            result.Add(go);
        }
        return result;
    }

    // AddComponent
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
        T ret = go.GetComponent<T>();
        if (ret == null) {
            ret = go.AddComponent<T>();
        }
        return ret;
    }

    //public static bool enabled<T>(bool val) where T : Component
    //{
	   // return val;
    //}
    // 转换时间戳
    public static DateTime ConvertFromTimeStamp(long timeStamp) {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timeStamp).ToLocalTime();
    }

    public static long ConvertToTimeStamp(DateTime time) {
        return (time.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
    }
}
