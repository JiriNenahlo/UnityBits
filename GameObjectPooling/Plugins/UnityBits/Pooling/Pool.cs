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

public abstract class Pool<T, C> where T : PoolableMonoBehaviour<C> where C : PoolableSpawnConfig {

    public List<T> pool = null;
    public readonly string prefabName;
    public readonly int initialCount;
    public readonly int expandCount;
    private readonly GameObject objectFromResources;
    private readonly Transform parentTransform;

    public Pool(string prefabName, Transform parentTransform, int initialCount, int expandCount) {
        this.prefabName = prefabName;
        this.initialCount = initialCount;
        this.expandCount = expandCount;
        this.parentTransform = parentTransform;

        pool = new List<T>();
        objectFromResources = Resources.Load<GameObject>(prefabName);

        Expand(initialCount);
    }

    private void Expand(int amount) {
        int previousPoolCount = pool.Count;
        for (int i = previousPoolCount; i < previousPoolCount + amount; i++) {
            GameObject newObject = Object.Instantiate(objectFromResources);
            pool.Add(newObject.GetComponent<T>());
            pool[i].transform.SetParent(parentTransform, false);
            pool[i].transform.rotation = Quaternion.identity;
            pool[i].transform.position = PoolableSpawnConfig.INIT_SPAWN_POS;
            pool[i].gameObject.SetActive(false);
        }
    }

    public T Pull(C config) {
        for (int i = 0; i < pool.Count; i++) {
            T obj = pool[i];
            if (!obj.gameObject.activeSelf) {
                obj.gameObject.SetActive(true);
                obj.Prepare(config);
                return obj;
            }
        }
        Expand(expandCount);
        return Pull(config);
    }

    public void ThrowBack(T objectScript) {
        objectScript.gameObject.SetActive(false);
        objectScript.OnDespawnInternal();
    }

    public void CallUpdateForAll() {
        for (int i = 0; i < pool.Count; i++) {
            T obj = pool[i];
            if (obj.isActiveAndEnabled && obj.config != null) {
                obj.UpdateMe();
            }
        }
    }

    public void CallLateUpdateForAll() {
        for (int i = 0; i < pool.Count; i++) {
            T obj = pool[i];
            if (obj.isActiveAndEnabled && obj.config != null) {
                obj.LateUpdateMe();
            }
        }
    }

    public void ThrowAllBack() {
        for (int i = 0; i < pool.Count; i++) {
            ThrowBack(pool[i]);
        }
    }

}
