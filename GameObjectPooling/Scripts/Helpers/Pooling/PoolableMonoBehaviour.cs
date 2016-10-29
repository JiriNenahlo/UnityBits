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

/// <summary>
/// Script attached to every object that takes advantage of pooling.
/// </summary>
/// <typeparam name="C">Config class with all the parameters each object gets prepared with
/// before spawning, after being pulled from the pool.</typeparam>
public abstract class PoolableMonoBehaviour<C> : MonoBehaviour where C : PoolableSpawnConfig {

    /// <summary>
    /// Config assigned to this pooled GameObject.
    /// </summary>
    [HideInInspector] public C config;

    /// <summary>
    /// This bool is set to false when the object is created by the pool.
    /// Setting it via the inspector to true means that this gameObject does not belong
    /// to a pool, but shares the same scripts as pooled ones.
    /// 
    /// This is useful when there are for example a lot of stars,
    /// but only one sun (= the sun has this flag set to true).
    /// </summary>
    public bool doesNotBelongToPool = false;

    public virtual void Awake() {
        if (doesNotBelongToPool) {
            Prepare(GetUniqueConfig());
        }
    }

    /// <summary>
    /// Called by the pool when spawned.
    /// </summary>
    /// <param name="config">Spawn properties to init the object with.</param>
    public void Prepare(C config) {
        this.config = config;
        OnPrepareFromConfig();
    }

    /// <summary>
    /// Called by the pool to get a config for the object not belonging to the pool.
    /// </summary>
    public virtual C GetUniqueConfig() {
        return null;
    }

    /// <summary>
    /// Implemented by each poolable objects to init properties on spawn.
    /// </summary>
    public abstract void OnPrepareFromConfig();

    /// <summary>
    /// Called when an object is despawned, probably returned to pool.
    /// </summary>
    public virtual void OnDespawn() { }

    void OnDisable() {
        if (!doesNotBelongToPool) {
            OnDespawn();
            transform.position = PoolableSpawnConfig.INIT_SPAWN_POS;
            transform.rotation = Quaternion.identity;
            config = null;
        }
    }

}
