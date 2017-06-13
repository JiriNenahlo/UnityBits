/*  MIT License

    Copyright(c) 2016 Jiří Nenáhlo

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE. */

using UnityEngine;
using System.Collections.Generic;

public interface IControlsEventListener {
    void OnSelect(Vector2 screenPos);
    void OnLongPress(Vector2 screenPos);
    void OnDragStart(Vector2 screenPos);
    void OnDrag(Vector2 initialScreenPos, Vector2 currScreenPos);
    void OnDragStop(Vector2 initialScreenPos, Vector2 endScreenPos);
    void OnZoomChanged(float amount);
    bool IsListeningForControlCallbacks();
}

public class Controls : SingletonMonoBehaviour<Controls> {

    public const float LongPressActionDelay = 0.450f;
    
    public static readonly Vector3 DefaultPrimaryInputPosition = new Vector3(float.MaxValue, float.MaxValue, 0f);

    ControlsModule module;
    List<IControlsEventListener> listeners = new List<IControlsEventListener>();
    public bool ignoreLongPress {
        set;
        get;
    }

    public void RegisterListener(IControlsEventListener listener) {
        if (!listeners.Contains(listener)) {
            listeners.Add(listener);
        }
    }

    public void SendCallbacks(System.Action<IControlsEventListener> action) {
        listeners.ForEach(listener => {
            if (listener.IsListeningForControlCallbacks()) {
                action.Invoke(listener);
            }
        });
    }

    public override void Awake() {
        base.Awake();
        ignoreLongPress = false;
#if UNITY_EDITOR
        module = new MouseControlModule(this);
#elif UNITY_ANDROID
        module = new TouchControlModule(this);
#endif
    }

    public Vector2 primaryInputPosition {
        get {
            return module.primaryInputPosition;
        }
    }

    void Start() {
        module.Start();
    }

    void OnEnable() {
        module.OnEnable();
    }

    void LateUpdate() {
        module.LateUpdate();
    }

    void OnDisable() {
        module.OnDisable();
    }

    /// <summary>
    /// When a custom longpress action is handled somewhere and drag needs to be registered after that longpress,
    /// by calling this method this script will continue to send drag callbacks.
    /// </summary>
    public void IgnoreLongPress() {
        ignoreLongPress = true;
    }
}

public abstract class ControlsModule {
    protected Controls manager;
    public ControlsModule(Controls manager) {
        this.manager = manager;
    }

    public virtual Vector2 primaryInputPosition {
        get {
            return Controls.DefaultPrimaryInputPosition;
        }
    }

    public virtual void Start() { }
    public virtual void OnEnable() { }
    public virtual void LateUpdate() { }
    public virtual void OnDisable() { }
    public virtual void IgnoreLongPress() { }
    public virtual void StopIgnoringLongPress() { }
}
