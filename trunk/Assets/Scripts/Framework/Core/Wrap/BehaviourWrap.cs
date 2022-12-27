using UnityEngine;
using System;

public class BehaviourWrap : MonoBehaviour {

    public string Description {
        get;
        set;
    }
    public object Data {
        get;
        set;
    }

    public Action OnStart;
    public Action OnUpdate;
    public Action OnLateUpdate;
    public Action DrawGizmos;
    public Action DrawGUI;

    void Start() {
        if (OnStart != null) {
            OnStart();
        }
    }
    void Update() {
        if (OnUpdate != null) {
            OnUpdate();
        }
    }
    void LateUpdate() {
        if (OnUpdate != null) {
            OnUpdate();
        }
    }
    void OnDrawGizmos() {
        if (DrawGizmos != null) {
            DrawGizmos();
        }
    }

    public void Dispose() {
        Destroy(this);
    }
}
