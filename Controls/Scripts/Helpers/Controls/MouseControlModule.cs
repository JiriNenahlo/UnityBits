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

public sealed class MouseControlModule : ControlsModule {

    int longPressActionUId;
    bool dragAllowed = false; // Don't call any further drag callback if longpress has already occured.
    Vector2 prevDragPosition;
    Vector2 dragStart;

    public MouseControlModule(Controls manager)
        : base(manager) {
    }

    public override Vector2 primaryInputPosition {
        get {
            return Input.mousePosition;
        }
    }

    public override void LateUpdate() {
        if (Input.GetMouseButtonDown(0)) {
            dragAllowed = true;
            dragStart = Input.mousePosition;
            manager.SendCallbacks(callback => callback.OnDragStart(dragStart));
            prevDragPosition = dragStart;
            longPressActionUId = LeanTween.delayedCall(Controls.LongPressActionDelay, () => {
                if (!manager.ignoreLongPress) {
                    manager.SendCallbacks(callback => callback.OnLongPress(dragStart));
                    dragAllowed = false;
                } else {
                    manager.ignoreLongPress = false;
                }
            }).uniqueId;
        }

        if (Input.GetMouseButton(0) && (dragAllowed || manager.ignoreLongPress)) {
            if ((Vector2) Input.mousePosition != prevDragPosition) {
                prevDragPosition = Input.mousePosition;
                LeanTween.cancel(longPressActionUId);
                manager.SendCallbacks(callback => callback.OnDrag(dragStart, prevDragPosition));
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            if (LeanTween.isTweening(longPressActionUId)) {
                LeanTween.cancel(longPressActionUId);
                manager.SendCallbacks(callback => callback.OnSelect(Input.mousePosition));
            } else if (dragAllowed) {
                manager.SendCallbacks(callback => callback.OnDragStop(dragStart, Input.mousePosition));
            }
        }

        float zoom = -Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0) {
            manager.SendCallbacks(callback => callback.OnZoomChanged(zoom));
        }
    }

    public override void OnDisable() {
        LeanTween.cancel(longPressActionUId);
    }

}
