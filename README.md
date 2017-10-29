# UnityBits
Set of useful C# Unity engine scripts to help build a solid codebase.

There are a few sets of scripts:

# Controls
Modular PC/mobile controls framework with callback-based input actions.

# GameObjectPooling
Simple and quick GameObject pooling implementation.

# PagerLayout
Responsive GridLayoutGroup that sets correct column & row count based on minimum item cell size
and attempts to stretch items across the full width/height if no more columns would fit.

Works like a pager, with associated buttons that show other parts of the dataset based on the selected page.
Items are dynamically sent to this layout and can be swapped during runtime easily.

# SingletonMonoBehaviour
Avoid GameObject.Find when calling methods from various singleton scripts.

# SwitchableLanguage
The Language class adds easy to use multiple language support to any Unity project by parsing an XML file
containing numerous strings translated into any languages of your choice.

Created by Adam T. Ryder
C# version by O. Glorieux
Static class rework & code cleanup by Jiří Nenáhlo

# ToStringBuilder
Outputs more user-friendly indented ToString output for complicated objects.
Supports nesting custom objects thanks to the provided interface.

Common TODOs:
- .unitypackage with all scripts