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
using UnityEngine.UI;
using System.Collections.Generic;

public interface IPagerItemCallback {
    void OnPageChanged(int page, int pageCount);
    void OnListItemClick(object itemData);
}

/// <summary>
/// Responsive GridLayoutGroup that sets correct column & row count based on minimum item cell size
/// and attempts to stretch items across the full width/height if no more columns would fit.
/// 
/// Works like a pager, with associated buttons that show other parts of the dataset based on the selected page.
/// Items are dynamically sent to this layout and can be swapped during runtime easily.
/// </summary>
public sealed class PagerLayout : GridLayoutGroup {
    
    GameObject prevBtn, nextBtn;

    public GameObject itemObject;
    List<PagerItem> items;
    List<object> dataset;
    IPagerItemCallback callback;

    public bool init = false;
    int colCount, rowCount;
    int cellCount = 0, prevCellCount = 0;
    int currPageIndex;
    public int pageCount {
        get;
        private set;
    }

    Vector2 cellMinSize = Vector2.zero;
    Vector2 layoutSize = Vector2.zero;

    public void Init(GameObject itemObject, List<object> data, GameObject previousPageBtn, GameObject nextPageBtn,
            IPagerItemCallback callback) {
        if (init) {
            throw new UnityException("PagerLayout can only be initiated once!");
        }

        this.itemObject = itemObject;
        prevBtn = previousPageBtn;
        nextBtn = nextPageBtn;
        this.callback = callback;

        items = new List<PagerItem>();
        LayoutElement referenceItem = itemObject.GetComponent<LayoutElement>();
        cellMinSize = new Vector2(referenceItem.minWidth, referenceItem.minHeight);

        init = true;
        UpdateDataSet(data);
    }
    
    public void UpdateDataSet(List<object> data, bool stayOnTheSamePage = false) {
        dataset = data;
        RecalculateResponsivity();
        ShowDataForPage(stayOnTheSamePage ? currPageIndex : 0);
    }

    private void RecalculateResponsivity() {
        if (!init)
            return;

        layoutSize = rectTransform.rect.size;

        colCount = Mathf.FloorToInt(layoutSize.x / cellMinSize.x);
        if (colCount <= 0) {
            colCount = 1;
        }

        rowCount = Mathf.FloorToInt(layoutSize.y / cellMinSize.y);
        if (rowCount <= 0) {
            rowCount = 1;
        }

        prevCellCount = cellCount;
        cellCount = rowCount * colCount;

        cellSize = new Vector2(
            (layoutSize.x / colCount) - spacing.x,
            (layoutSize.y / rowCount) - spacing.y);

        bool cellCountChanged = prevCellCount != cellCount;

        if (cellCountChanged) {
            int itemHoldersToSpawn = cellCount - prevCellCount;
            if (itemHoldersToSpawn < 0) {
                // Despawn items from the end of the layout.
                for (int i = 0; i < Mathf.Abs(itemHoldersToSpawn); i++) {
                    PagerItem despawnedItem = items[items.Count - 1];
                    items.Remove(despawnedItem);
                    // This plugin is originally intended for games on mobile platforms without
                    // orientation support. Cells count will be different for the first time,
                    // after that it will stay constant. Not using pooling is safe here.
                    Destroy(despawnedItem.gameObject);
                }
            } else {
                // Add more items to the end of the layout.
                for (int i = 0; i < itemHoldersToSpawn; i++) {
                    GameObject newObject = Instantiate(itemObject);
                    newObject.transform.SetParent(rectTransform, false);
                    PagerItem newItem = newObject.GetComponent<PagerItem>();
                    newItem.Prepare(items.Count, callback);
                    items.Add(newItem);
                }
            }
            ShowDataForPage(currPageIndex);
        }
    }

    protected override void OnRectTransformDimensionsChange() {
        base.OnRectTransformDimensionsChange();

        if (gameObject.activeInHierarchy) {
            RecalculateResponsivity();
        }
    }

    void ShowDataForPage(int newPageIndex, bool skipEqualPageCheck = true) {
        if (cellCount <= 0 || dataset == null)
            return;
        
        pageCount = Mathf.CeilToInt((float) dataset.Count / (float) cellCount);
        int newPageIndexClamped = Mathf.Clamp(newPageIndex, 0, pageCount - 1);
        if (currPageIndex == newPageIndex && !skipEqualPageCheck) {
            return;
        }
        currPageIndex = newPageIndexClamped;

        int itemObjectIndex = 0;
        for (int itemDataIndex = currPageIndex * cellCount; itemDataIndex < (currPageIndex + 1) * cellCount; itemDataIndex++) {
            if (itemDataIndex >= dataset.Count || dataset.Count == 0) {
                items[itemObjectIndex].gameObject.SetActive(false);
            } else {
                items[itemObjectIndex].gameObject.SetActive(true);
                items[itemObjectIndex].ChangeData(dataset[itemDataIndex]);
            }
            itemObjectIndex++;
        }

        // Update the buttons.
        prevBtn.gameObject.SetActive(currPageIndex > 0);
        nextBtn.gameObject.SetActive(currPageIndex < pageCount - 1);

        callback.OnPageChanged(currPageIndex + 1, pageCount);
    }

    public void NextPage() {
        ShowDataForPage(currPageIndex + 1, false);
    }

    public void PreviousPage() {
        ShowDataForPage(currPageIndex - 1, false);
    }

    public void CallUpdateForAll() {
        foreach (PagerItem item in items) {
            item.UpdateMe();
        }
    }

}
