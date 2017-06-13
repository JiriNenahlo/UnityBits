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

public sealed class TouchControlModule : ControlsModule {

    public const float PerspectiveTouchZoomSpeed = 0.05f;

    int longPressActionUId;
    bool dragAllowed = false; // Don't call any further drag callback if longpress has already occured.
    Vector2 prevDragPosition;
    Vector2 dragStart;

    public TouchControlModule(Controls manager)
        : base(manager) {
    }

    public override Vector2 primaryInputPosition {
        get {
            return Input.touchCount > 0 ? Input.GetTouch(0).position : base.primaryInputPosition;
        }
    }

    public override void LateUpdate() {
        if (Input.touchCount == 1) {
            Touch currentTouch = Input.GetTouch(0);
            switch (currentTouch.phase) {
                case TouchPhase.Began:
                    dragAllowed = true;
                    dragStart = currentTouch.position;
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
                    break;
                case TouchPhase.Moved:
                    if (currentTouch.position != prevDragPosition && (dragAllowed || manager.ignoreLongPress)) {
                        prevDragPosition = currentTouch.position;
                        LeanTween.cancel(longPressActionUId);
                        manager.SendCallbacks(match => match.OnDrag(dragStart, prevDragPosition));
                    }
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    if (LeanTween.isTweening(longPressActionUId)) {
                        LeanTween.cancel(longPressActionUId);
                        manager.SendCallbacks(match => match.OnSelect(currentTouch.position));
                    } else if (dragAllowed) {
                        manager.SendCallbacks(match => match.OnDragStop(dragStart, currentTouch.position));
                    }
                    break;
            }
        } else if (Input.touchCount > 1) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);
            
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
            
            float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * PerspectiveTouchZoomSpeed;
            manager.SendCallbacks(match => match.OnZoomChanged(deltaMagnitudeDiff));
        }
    }

    public override void OnDisable() {
        LeanTween.cancel(longPressActionUId);
    }

}
