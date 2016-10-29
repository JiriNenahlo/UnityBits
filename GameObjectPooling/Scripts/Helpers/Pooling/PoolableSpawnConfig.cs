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
/// When pulling an object from the pool, it is given a specific config
/// that a pooled object is spawned with. This is the config class
/// with only the required parameters to spawn an object.
/// </summary>
public abstract class PoolableSpawnConfig {
    
    /// <summary>
    /// Position at which the objects are spawned at when they are being pulled from the pool,
    /// to prevent them from showing up in front of the camera before they are prepared.
    /// </summary>
    public static readonly Vector3 INIT_SPAWN_POS = new Vector3(-2000f, -2000f, -2000f);

    public Vector3 position;
    public Quaternion rotation;

    public PoolableSpawnConfig(Vector3 position, Quaternion rotation) {
        this.position = position;
        this.rotation = rotation;
    }

}
