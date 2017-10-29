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
/// When pulling an object from the pool, it is given a specific config that
/// changes its properties. This is the config.
/// </summary>
[System.Serializable]
public abstract class PoolableSpawnConfig {

    /// <summary>
    /// Position at which the objects are spawned at when they are being pulled from the pool,
    /// to prevent them from showing up in front of the camera before they are prepared.
    /// </summary>
    public static readonly Vector3 INIT_SPAWN_POS = new Vector3(-2000f, -2000f, 0f);

    float serializedPosX, serializedPosY, serializedPosZ;

    public Vector3 Position {
        get {
            return new Vector3(serializedPosX, serializedPosY, serializedPosZ);
        }
        set {
            serializedPosX = value.x;
            serializedPosY = value.y;
            serializedPosZ = value.z;
        }
    }

    float serializedRotX, serializedRotY, serializedRotZ, serializedRotW;

    public Quaternion Rotation {
        get {
            return new Quaternion(serializedRotX, serializedRotY, serializedRotZ, serializedRotW);
        }
        set {
            serializedRotX = value.x;
            serializedRotY = value.y;
            serializedRotZ = value.z;
            serializedRotW = value.w;
        }
    }

    public PoolableSpawnConfig(Vector3 position, Quaternion rotation) {
        this.Position = position;
        this.Rotation = rotation;
    }

}
