# SingletonMonoBehaviour
Avoid GameObject.Find when calling methods from various singleton scripts.

Usage:
1. Change your script from a standard monobehaviour...
```
using UnityEngine;

public class Script : MonoBehaviour {
	void Awake() {
		Debug.Log("Hello MonoBehaviour!");
	}
	
	public void SayHi() {
		Debug.Log("Hi!");
	}
}
```

2. ...to the singleton implementation.
```
using UnityEngine;

public sealed class Script : SingletonMonoBehaviour<Script> {
	public override void Awake() {
		base.Awake();
		Debug.Log("Hello SingletonMonoBehaviour!");
	}
	
	public void SayHi() {
		Debug.Log("Hi!");
	}
}
```

3. Enjoy easier method referencing.
```
Script.Instance.SayHi();
```