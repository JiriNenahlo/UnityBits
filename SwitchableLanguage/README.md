# SwitchableLanguage
The Language class adds easy to use multiple language support to any Unity project by parsing an XML file
containing numerous strings translated into any languages of your choice.

Created by Adam T. Ryder
C# version by O. Glorieux
Static class rework & code cleanup by Jiří Nenáhlo

Usage:
1. Create an 'English.xml' language file (default) in the Resources/Language folder.
```
<?xml version="1.0" encoding="utf-8"?>
<languages>
    <English>
        <string name="test">Hello world!</string>
    </English>
</languages>
```

2. Create translated version of that file also in the Resources/Language folder. The name of the file must match any of the Unity's built in SystemLanguage enum value.
```
<?xml version="1.0" encoding="utf-8"?>
<languages>
    <Czech>
        <string name="test">Ahoj světe!</string>
    </Czech>
</languages>
```

3. Init the script.
```
public class Script : MonoBehaviour {
	void Awake() {
		Language.Change(Application.systemLanguage);
	}
}
```

4. Get the translated string by resource id written in the XML's name tag.
```
string translatedString = Language.GetString("test");
```

5. (Optional) Use LanguageTextSetter script attached to any GameObject with a Text component to automatically make the translated string show up when the game is started.