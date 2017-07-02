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
    
    int mouseButtonAction = -1;
    Vector2 prevDragPosition;
    Vector2 dragStart;
    bool hasDragged;

    public MouseControlModule(Controls manager)
        : base(manager) {
    }

    public override Vector2 primaryInputPosition {
        get {
            return Input.mousePosition;
        }
    }

    public override void LateUpdate() {
        HandleDragActionButton(0);
        HandleDragActionButton(2);

        // Dont mess up callbacks with two buttons pressed at once.
        if (mouseButtonAction != -1 && !Input.GetMouseButton(0) && !Input.GetMouseButton(2)) {
            mouseButtonAction = -1;
        }

        if (mouseButtonAction == -1 && Input.GetMouseButtonDown(1)) {
            manager.SendCallbacks(callback => callback.OnContextualActionPerformed(Input.mousePosition));
        }

        float zoom = -Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0) {
            manager.SendCallbacks(callback => callback.OnZoomChanged(zoom));
        }
    }

    void HandleDragActionButton(int mouseButton) {
        if (Input.GetMouseButton(mouseButton)
                || Input.GetMouseButtonDown(mouseButton)
                || Input.GetMouseButtonUp(mouseButton)) {
            if (mouseButtonAction == -1) {
                mouseButtonAction = mouseButton;
            } else if (mouseButtonAction != mouseButton) {
                return;
            }
        } else {
            return;
        }

        if (Input.GetMouseButtonDown(mouseButton)) {
            dragStart = Input.mousePosition;
            hasDragged = false;
            
            if (mouseButton == 0) {
                manager.SendCallbacks(callback => {
                    callback.OnPickup(dragStart);
                    callback.OnPreparePrimaryDragVariables(dragStart);
                });
            } else if (mouseButton == 2) {
                manager.SendCallbacks(callback => callback.OnPrepareSecondaryDragVariables(dragStart));
            }
            prevDragPosition = dragStart;
        }

        if (Input.GetMouseButton(mouseButton)) {
            if ((Vector2) Input.mousePosition != prevDragPosition) {
                prevDragPosition = Input.mousePosition;
                hasDragged = true;
                if (mouseButton == 0) {
                    manager.SendCallbacks(callback => callback.OnItemDragAction(dragStart, prevDragPosition));
                    manager.SendCallbacks(callback => callback.OnPrimaryDragAction(dragStart, prevDragPosition));
                } else if (mouseButton == 2) {
                    manager.SendCallbacks(callback => callback.OnSecondaryDragAction(dragStart, prevDragPosition));
                }
            }
        }

        if (Input.GetMouseButtonUp(mouseButton)) {
            if (!hasDragged) {
                manager.SendCallbacks(callback => {
                    callback.OnSelect(Input.mousePosition);
                    callback.OnDrop(dragStart, Input.mousePosition);
                });
            } else {
                if (mouseButton == 0) {
                    manager.SendCallbacks(callback => callback.OnPrimaryDragStop(dragStart, Input.mousePosition));
                } else if (mouseButton == 2) {
                    manager.SendCallbacks(callback => callback.OnSecondaryDragStop(dragStart, Input.mousePosition));
                }
            }
        }
    }

}
