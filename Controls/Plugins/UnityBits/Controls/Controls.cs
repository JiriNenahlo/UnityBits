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
using UnityEngine.EventSystems;

public interface IControlsEventListener {
    void OnSelect(Vector2 screenPos);
    void OnContextualActionPerformed(Vector2 screenPos);
    void OnPreparePrimaryDragVariables(Vector2 screenPos);
    void OnPrepareSecondaryDragVariables(Vector2 screenPos);
    void OnItemDragAction(Vector2 initialScreenPos, Vector2 currScreenPos);
    void OnPrimaryDragAction(Vector2 initialScreenPos, Vector2 currScreenPos);
    void OnSecondaryDragAction(Vector2 initialScreenPos, Vector2 currScreenPos);
    void OnPrimaryDragStop(Vector2 initialScreenPos, Vector2 endScreenPos);
    void OnSecondaryDragStop(Vector2 initialScreenPos, Vector2 endScreenPos);
    void OnPickup(Vector2 screenPos);
    void OnDrop(Vector2 initialScreenPos, Vector2 endScreenPos);
    void OnZoomChanged(float amount);
    bool IsListeningForControlCallbacks();
}

public sealed class Controls : SingletonMonoBehaviour<Controls> {

    public const float LongPressActionDelay = 0.450f;
    
    public static readonly Vector3 DefaultPrimaryInputPosition = new Vector3(-float.MaxValue, -float.MaxValue, 0f);

    public bool interactingWithUI = false;

    ControlsModule module;
    List<IControlsEventListener> listeners = new List<IControlsEventListener>();

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
#if UNITY_EDITOR || !MOBILE_PLATFORM
        module = new MouseControlModule(this);
#elif UNITY_ANDROID
        module = new TouchControlModule(this);
#endif
    }

    public Vector2 PrimaryInputPosition { get { return module.PrimaryInputPosition; } }

    public void SetPossibleInteractingWithUI(bool start) {
        if (start) {
            interactingWithUI = EventSystem.current.currentSelectedGameObject != null;
        } else {
            interactingWithUI = false;
        }
    }

    void Start() { module.Start(); }

    void OnEnable() { module.OnEnable(); }

    /// <summary>
    /// Exposed for update manager implementation.
    /// </summary>
    public void UpdateMe() { module.UpdateMe(); }

    void OnDisable() { module.OnDisable(); }
}

public abstract class ControlsModule {
    protected Controls manager;
    public ControlsModule(Controls manager) {
        this.manager = manager;
    }

    public virtual Vector2 PrimaryInputPosition { get { return Controls.DefaultPrimaryInputPosition; } }

    public virtual void Start() { }
    public virtual void OnEnable() { }
    public virtual void UpdateMe() { }
    public virtual void LateUpdate() { }
    public virtual void OnDisable() { }
    public virtual void StopIgnoringLongPress() { }
}
