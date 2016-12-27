# GameObjectPooling
Simple and quick GameObject pooling implementation.

Usage:
1. Create a script for the pooled object and its config. In this case, we also add a name to it, just to add some custom spawn values to it.
```
using UnityEngine;

public sealed class SomeObject : PoolableMonoBehaviour<SomeObject.Config> {
	public sealed class Config : PoolableSpawnConfig {
		public string name { get; private set; }
		public Config(Vector3 position, Quaternion rotation, string name)
				: base(position, rotation) {
			this.name = name;
		}
	}
	
	public override void OnPrepareFromConfig() {
		Debug.Log(config.name + " was spawned.");
	}
}
```

2. Create a prefab in the Assets/Resources folder. In this case, we will create it under a 'Prefabs' subfolder, to match with the pool class code below.

3. Create a Pool subclass for the pooled script and the config.
```
using UnityEngine;

public class SomePool : Pool<SomeObject, SomeObject.Config> {
    public SomePool() : base("Prefabs/SomeObjectPrefab", 30, 10) { }
    public override Transform GetParent() {
        return GameObject.Find("PoolParent").transform;
    }
}
```

4. Spawn the object.
```
public class SomeMonoBehaviour : MonoBehaviour {
	SomePool pool;
	void Awake() {
		pool = new SomePool();
		
		pool.Pull(new SomeObject.Config(Vector3.zero, Quaternion.identity, "Pepa"));
		pool.Pull(new SomeObject.Config(Vector3.one, Quaternion.identity, "Jarda"));
		pool.Pull(new SomeObject.Config(-Vector3.one, Quaternion.identity, "Ezop"));
	}
}
```

For more info take a look at how the code works.

TODO:
- GameObject config is not prepared when there it is called from FixedUpdate.