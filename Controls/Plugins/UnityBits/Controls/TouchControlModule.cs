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

    public const float
        PerspectiveTouchZoomSpeed = 0.0025f;

    int longPressActionUId;
    Vector2 prevDragPosition;
    Vector2 dragStart;
    bool hasDragged, longPressHappened;
    bool singleFingerActionsAllowed = true;

    public TouchControlModule(Controls manager)
        : base(manager) {
    }

    public override Vector2 PrimaryInputPosition { get { return Input.touchCount > 0 ? Input.GetTouch(0).position : base.PrimaryInputPosition; } }

    public override void UpdateMe() {
        if (Input.touchCount == 0) {
            singleFingerActionsAllowed = true;
        } else if (Input.touchCount == 1) {
            Touch currTouch = Input.GetTouch(0);

            if (!singleFingerActionsAllowed)
                return;

            switch (currTouch.phase) {
                case TouchPhase.Began:
                    manager.SetPossibleInteractingWithUI(true);
                    hasDragged = false;
                    longPressHappened = false;
                    dragStart = currTouch.position;
                    manager.SendCallbacks(callback => callback.OnPreparePrimaryDragVariables(dragStart));
                    prevDragPosition = dragStart;
                    longPressActionUId = LeanTween.delayedCall(Controls.LongPressActionDelay, () => {
                        manager.SetPossibleInteractingWithUI(false);
                        manager.SendCallbacks(callback => callback.OnPrepareSecondaryDragVariables(dragStart));
                        manager.SendCallbacks(callback => callback.OnPickup(dragStart));
                        longPressHappened = true;
                    }).uniqueId;
                    break;
                case TouchPhase.Moved:
                    hasDragged = true;
                    prevDragPosition = currTouch.position;
                    LeanTween.cancel(longPressActionUId);
                    if (longPressHappened) {
                        manager.SendCallbacks(callback => callback.OnItemDragAction(dragStart, currTouch.position));
                        manager.SendCallbacks(callback => callback.OnSecondaryDragAction(dragStart, currTouch.position));
                    } else {
                        manager.SendCallbacks(callback => callback.OnPrimaryDragAction(dragStart, prevDragPosition));
                    }
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    EndSingleTouch(currTouch);
                    break;
            }
        } else if (Input.touchCount == 2) {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector3 touchZeroStart = Vector3.zero;
            
            switch (touchOne.phase) {
                case TouchPhase.Began:
                    // When the other finger joins, cancel the contextual action along with other one-finger-only gestures.
                    EndSingleTouch(touchZero);
                    singleFingerActionsAllowed = false;
                    break;
                case TouchPhase.Moved:
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    float deltaMagnitudeDiff = (prevTouchDeltaMag - touchDeltaMag) * PerspectiveTouchZoomSpeed;
                    manager.SendCallbacks(match => match.OnZoomChanged(deltaMagnitudeDiff));
                    break;
                case TouchPhase.Canceled:
                case TouchPhase.Ended:
                    break;
            }
        }
    }

    void EndSingleTouch(Touch currTouch) {
        LeanTween.cancel(longPressActionUId);
        if (longPressHappened) {
            Vector3 touchPos = currTouch.position;
            manager.SendCallbacks(match => match.OnSecondaryDragStop(dragStart, touchPos));
            manager.SendCallbacks(match => match.OnDrop(dragStart, touchPos));
            manager.SendCallbacks(match => match.OnContextualActionPerformed(touchPos));
        } else {
            if (hasDragged) {
                manager.SendCallbacks(match => match.OnPrimaryDragStop(dragStart, currTouch.position));
            } else {
                manager.SendCallbacks(match => match.OnSelect(currTouch.position));
            }
        }
        manager.SetPossibleInteractingWithUI(false);
    }

    public override void OnDisable() {
        LeanTween.cancel(longPressActionUId);
    }

}
