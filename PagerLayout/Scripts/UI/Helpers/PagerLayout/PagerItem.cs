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
using UnityEngine.EventSystems;

/// <summary>
/// An item gameobject in the dataset displayed in the pager layout.
/// </summary>
public abstract class PagerItem : MonoBehaviour, IPointerClickHandler {

    /// <summary>
    /// Data this item shows.
    /// </summary>
    protected object data;
    protected int layoutCellIndex;
    IPagerItemCallback callback;

    public void Prepare(int layoutCellIndex, IPagerItemCallback callback) {
        this.layoutCellIndex = layoutCellIndex;
        this.callback = callback;
    }

    /// <summary>
    /// Change the underlying data and update the UI.
    /// </summary>
    public void ChangeData(object data) {
        this.data = data;
        UpdateUIAfterDataChange();
    }
    
    public abstract void UpdateUIAfterDataChange();

    /// <summary>
    /// Exposed for update manager implementation.
    /// </summary>
    public virtual void UpdateMe() { }

    public void OnPointerClick(PointerEventData eventData) {
        callback.OnListItemClick(data);
    }

}
