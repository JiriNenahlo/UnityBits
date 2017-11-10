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
    /// This bool is set to false if the object is meant to be included in, and created by the pool.
    /// If this bool is true, it means that this script does not belong
    /// to an object in pool, eg. it is placed in the scene on purpose and has to be provided with a config via other scripts.
    /// 
    /// Useful for static objects in the scene that share the same behaviour as those meant to be included in a pool in the first place,
    /// for example having multiple dynamic pooled planets, but only one sun (with this flag set to true).
    /// </summary>
    public bool doesNotBelongToPool = false;
    
    /// <summary>
    /// Called by the pool when spawned.
    /// </summary>
    /// <param name="config">Spawn properties to init the object with.</param>
    public void Prepare(C config) {
        if (config == null)
            return;

        ChangeConfig(config);
    }

    /// <summary>
    /// Sets a new config and calls the appropriate methods for the object to update its state based on the new provided config properly.
    /// Must be called explicitly for objects that do not receive any config when spawned (doesNotBelongToPool == true), as they are meant
    /// to be explicitly put in the scene outside of the pool's influence.
    /// </summary>
    /// <param name="config">Config to init the object with.</param>
    public void ChangeConfig(C config) {
        this.config = config;
        OnSpawn();
    }

    /// <summary>
    /// Implemented by each poolable objects to init properties on spawn.
    /// </summary>
    public abstract void OnSpawn();

    /// <summary>
    /// Exposed for update manager implementation.
    /// </summary>
    public virtual void UpdateMe() { }

    /// <summary>
    /// Exposed for update manager implementation.
    /// </summary>
    public virtual void LateUpdateMe() { }

    /// <summary>
    /// Called by the pool when this pooled object is despawned (returned to the pool).
    /// </summary>
    public void OnDespawnInternal() {
        if (config != null) {
            OnDespawn();
        }
        transform.position = PoolableSpawnConfig.INIT_SPAWN_POS;
        transform.rotation = Quaternion.identity;
        config = null;
    }

    /// <summary>
    /// Object is being despawned, stop & cleanup everything related to this object's current instance,
    /// so it can be reused next time.
    /// </summary>
    public virtual void OnDespawn() { }

}
