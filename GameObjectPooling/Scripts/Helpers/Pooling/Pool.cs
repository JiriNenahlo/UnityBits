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
    private GameObject objectFromResources;

    public Pool(string prefabName, int initialCount, int expandCount) {
        this.prefabName = prefabName;
        this.initialCount = initialCount;
        this.expandCount = expandCount;
        
        pool = new List<T>();
        objectFromResources = Resources.Load<GameObject>(prefabName);

        Expand(initialCount);
    }

    private void Expand(int ammount) {
        int previousPoolCount = pool.Count;
        for (int i = previousPoolCount; i < previousPoolCount + ammount; i++) {
            GameObject newObject = Object.Instantiate(objectFromResources);
            pool.Add(newObject.GetComponent<T>());
            if (pool[i] == null) {
                pool[i] = newObject.GetComponentInParent<T>();
            }
            if (pool[i] == null) {
                pool[i] = newObject.GetComponentInChildren<T>();
            }
            pool[i].transform.SetParent(GetParent(), false);
            pool[i].transform.position = PoolableSpawnConfig.INIT_SPAWN_POS;
            pool[i].transform.rotation = Quaternion.identity;
            pool[i].gameObject.SetActive(false);
        }
    }

    public abstract Transform GetParent();

    public T PullGameObjectFromPool(C config) {
        for (int i = 0; i < pool.Count; i++) {
            if (!pool[i].gameObject.activeSelf) {
                pool[i].gameObject.SetActive(true);
                pool[i].Prepare(config);
                return pool[i];
            }
        }

        //Debug.LogWarning("No more pooled GameObjects available in the pool for '" + prefabName + "'. " +
        //    "Only " + pool.Count + " are available. Expanding pool by " + expandCount + ".");
        
        Expand(expandCount);
        return PullGameObjectFromPool(config);
    }

    public void ThrowGameObjectBackToPool(T objectScript) {
        objectScript.gameObject.SetActive(false);
    }

    public void ThrowAllBackToPool() {
        for (int i = 0; i < pool.Count; i++) {
            pool[i].gameObject.transform.position = Vector2.zero;
            ThrowGameObjectBackToPool(pool[i]);
        }
    }

}
