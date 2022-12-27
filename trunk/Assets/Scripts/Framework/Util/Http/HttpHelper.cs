using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Framework.Util;

// Http帮助类
public class HttpHelper : SingletonMono<HttpHelper> {
    
    public void WWWFormRequest(string url, WWWForm form, Action<string> onSuccess, Action<string> onFail) {
        CoroutineHelper.StartCoroutine(_WWWFormRequest(url, form, onSuccess, onFail));
    }

    IEnumerator _WWWFormRequest(string url, WWWForm form, Action<string> onSuccess, Action<string> onFail) {
        WWW www = new WWW(url, form);
        yield return www;
        if (www.error != null) {
            if (onFail != null) 
                onFail(www.error);
        }
        if (www.isDone) 
            if (onSuccess != null) 
                onSuccess(www.text);
    }
#region Get
    // GET请求
    public void Get(string url, Action<UnityWebRequest> onSuccess, 
                    Action<UnityWebRequest> onFail, int retryTime = 1) {
        CoroutineHelper.StartCoroutine(_Get(url, onSuccess, onFail, retryTime));
    }
    public UnityWebRequest Get(string url) {
        return UnityWebRequest.Get(url);
    }
    // GET请求
    IEnumerator _Get(string url, Action<UnityWebRequest> onSuccess, 
                     Action<UnityWebRequest> onFail, int retryTime = 1) {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url)) {
            yield return uwr.SendWebRequest();
            if (!(uwr.isNetworkError || uwr.isHttpError)) {
                if (onSuccess != null) 
                    onSuccess(uwr);
            } else {
                retryTime--;
                if (retryTime > 0) {
                    Get(url, onSuccess, onFail, retryTime);
                } else {
                    if (onFail != null) 
                        onFail(uwr);
                }
            }
        }
    }
    // 下载文件
    public void DownloadFile(string url, string downloadFilePathAndName, Action<UnityWebRequest> onSuccess, 
                             Action<UnityWebRequest> onFail, int retryTime = 1) {
        CoroutineHelper.StartCoroutine(_DownloadFile(url, downloadFilePathAndName, onSuccess, onFail, retryTime));
    }
    // 下载文件
    IEnumerator _DownloadFile(string url, string downloadFilePathAndName, Action<UnityWebRequest> onSuccess, 
                              Action<UnityWebRequest> onFail, int retryTime = 1) {
        var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerFile(downloadFilePathAndName);
        yield return uwr.SendWebRequest();
        if (!(uwr.isNetworkError || uwr.isHttpError)) {
            if (onSuccess != null) 
                onSuccess(uwr);
        } else {
            retryTime--;
            if (retryTime > 0) {
                DownloadFile(url, downloadFilePathAndName, onSuccess, onFail, retryTime);
            } else {
                if (onFail != null)
                    onFail(uwr);
            }
        }
    }
    // 请求图片
    public void GetTexture(string url, Action<Texture2D, UnityWebRequest> onSuccess, 
                           Action<UnityWebRequest> onFail, int retryTime = 1) {
        CoroutineHelper.StartCoroutine(_GetTexture(url, onSuccess, onFail, retryTime));
    }
    // 请求图片
    IEnumerator _GetTexture(string url, Action<Texture2D, UnityWebRequest> onSuccess, 
                            Action<UnityWebRequest> onFail, int retryTime = 1) {
        UnityWebRequest uwr = new UnityWebRequest(url);
        DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);
        uwr.downloadHandler = downloadTexture;
        yield return uwr.SendWebRequest();
        Texture2D t = null;
        if (!(uwr.isNetworkError || uwr.isHttpError)) {
            t = downloadTexture.texture;
            if (onSuccess != null) 
                onSuccess(t, uwr);
        } else {
            retryTime--;
            if (retryTime > 0) {
                GetTexture(url, onSuccess, onFail, retryTime);
            } else {
                if (onFail != null) 
                    onFail(uwr);
            }
        }
    }
    // 请求AssetBundle
    public void GetAssetBundle(string url, Action<AssetBundle> onSuccess, 
                               Action<UnityWebRequest> onFail, int retryTime = 1) {
        CoroutineHelper.StartCoroutine(_GetAssetBundle(url, onSuccess, onFail, retryTime));
    }
    // 请求AssetBundle
    IEnumerator _GetAssetBundle(string url, Action<AssetBundle> onSuccess, 
                                Action<UnityWebRequest> onFail, int retryTime = 1) {
        UnityWebRequest uwr = new UnityWebRequest(url);
        DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(uwr.url, uint.MaxValue);
        uwr.downloadHandler = handler;
        yield return uwr.SendWebRequest();
        AssetBundle bundle = null;
        if (!(uwr.isNetworkError || uwr.isHttpError)) {
            bundle = handler.assetBundle;
            if (onSuccess != null) 
                onSuccess(bundle);
        } else {
            retryTime--;
            if (retryTime > 0) {
                GetAssetBundle(url, onSuccess, onFail, retryTime);
            } else {
                if (onFail != null) 
                    onFail(uwr);
            }
        }
    }
#endregion

#region Post
    // 向服务器提交post请求
    public void Post(string serverURL, List<IMultipartFormSection> lstformData, Action<UnityWebRequest> onSuccess, 
                     Action<UnityWebRequest> onFail, int retryTime = 1) {
        // List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        // formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        // formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        CoroutineHelper.StartCoroutine(_Post(serverURL, lstformData, onSuccess, onFail, retryTime));
    }
    // 向服务器提交post请求
    IEnumerator _Post(string serverURL, List<IMultipartFormSection> lstformData, Action<UnityWebRequest> onSuccess, 
                      Action<UnityWebRequest> onFail, int retryTime = 1) {
        // List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        // formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        // formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        UnityWebRequest uwr = UnityWebRequest.Post(serverURL, lstformData);
        yield return uwr.SendWebRequest();
        if (!(uwr.isNetworkError || uwr.isHttpError)) {
            if (onSuccess != null) {
                onSuccess(uwr);
            }
        } else {
            retryTime--;
            if (retryTime > 0) {
                Post(serverURL, lstformData, onSuccess, onFail, retryTime);
            } else {
                if (onFail != null) {
                    onFail(uwr);
                }
            }
        }
    }
#endregion

    // 通过PUT方式将字节流传到服务器
    public void UploadByPut(string url, byte[] contentBytes, Action<bool> actionResult) {
        StartCoroutine(_UploadByPut(url, contentBytes, actionResult, ""));
    }

    // 通过PUT方式将字节流传到服务器
    IEnumerator _UploadByPut(string url, byte[] contentBytes, Action<bool> actionResult, string contentType = "application/octet-stream") {
        UnityWebRequest uwr = new UnityWebRequest();
        UploadHandler uploader = new UploadHandlerRaw(contentBytes);
        // Sends header: "Content-Type: custom/content-type";
        uploader.contentType = contentType;
        uwr.uploadHandler = uploader;
        yield return uwr.SendWebRequest();
        bool res = true;
        if (uwr.isNetworkError || uwr.isHttpError) 
            res = false;
        if (actionResult != null)
            actionResult(res);
    }
}


