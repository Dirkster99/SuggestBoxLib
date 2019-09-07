[![Build status](https://ci.appveyor.com/api/projects/status/nw16d7fi59x0se0p?svg=true)](https://ci.appveyor.com/project/Dirkster99/suggestboxlib) [![Release](https://img.shields.io/github/release/Dirkster99/SuggestBoxLib.svg)](https://github.com/Dirkster99/SuggestBoxLib/releases/latest) [![NuGet](https://img.shields.io/nuget/dt/Dirkster.SuggestBoxLib.svg)](http://nuget.org/packages/Dirkster.SuggestBoxLib)

![Net4](https://badgen.net/badge/Framework/.Net&nbsp;4/blue) ![NetCore3](https://badgen.net/badge/Framework/NetCore&nbsp;3/blue)

The SuggestBox control in this repository was originally developed by <b>Leung Yat Chun Joseph <a href="https://github.com/lycj">lycj</a></b> in his FileExplorer application originating from CodePlex and <a href="https://www.codeproject.com/Members/Fainx">CodeProject</a>.

# SuggestBoxLib

<h2><img src="https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/icons/AutoComplete_64x.png" height="64"/>&nbsp;Overview</h2>

A WPF Dark/Light AutoComplete TextBox that can easily handle 20.000+ entries.

This project implements a WPF Dark/Light AutoComplete TextBox that can easily handle 20.000+ entries in the
list of suggestions. The screenshots below show a dark themed demo appliaction with a classic AutoComplete
use case for browsing the file system. This control can also be used to browse other data structures since
the data processing is implemented in the ViewModel/Model layers of the MVVM demo app, while
the control itself is limited to the view.

Review the [Wiki section](https://github.com/Dirkster99/SuggestBoxLib/wiki) to find out more details of the available API.

This control is also used in a [Metro Breadcrumb control](https://github.com/Dirkster99/bm).

The first two screenshots show how a seperate combobox like drop down control can be used to select an entry from
a list of recently visited locations (bound to a collection in the viewmodel):
![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/Unbenannt-7.png)

![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/Unbenannt-8.png)

A selection of a recently visited location can be used as a starting point to follow up with more suggestions:
![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/Unbenannt-9.png)

![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/Unbenannt-10.png)

The control can shorten text that is too long for display by inserting ellipses '...' on the:
- left
- right or in the
- center

of a given string. This display is available only if the control is not currently focused.
![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/ShowEllipses_Centered.png)

## User Feedback

The control implements a [NextTargetLocationArgs](https://github.com/Dirkster99/SuggestBoxLib/blob/master/source/SuggestBoxLib/Events/NextTargetLocationArgs.cs) event that can be raised via enter/escape key in the textbox control to support keyboard
gestures to confirm/cancel editing of a location.

![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/OK_Cancel.png)

## Error Feedback
The control can show a red rectangle if the user types a completely unmatchable string. This red rectangle
can be triggered with the property attached to the checkbox in the demo application.

![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/Unbenannt-6.png)

## Highlighting Color and Themes
Screenshot in this repository where done with this highlighting color on Windows 10:
![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/Untitled.png)

A Dark/Light themed demo application and a Generic application are part of this repository.  

![](https://raw.githubusercontent.com/Dirkster99/Docu/master/SuggestBoxLib/screenshots/Unbenannt-4.png)

Load *Light* or *Dark* brush resources in you resource dictionary to take advantage of existing definitions.

```XAML
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="/SuggestBoxLib;component/Themes/DarkBrushes.xaml" />
    </ResourceDictionary.MergedDictionaries>
```

```XAML
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="/SuggestBoxLib;component/Themes/LightBrushes.xaml" />
    </ResourceDictionary.MergedDictionaries>
```

These definitions do not theme all controls used within this library. You should use a standard theming library, such as:
- [MahApps.Metro](https://github.com/MahApps/MahApps.Metro),
- [MLib](https://github.com/Dirkster99/MLib), or
- [MUI](https://github.com/firstfloorsoftware/mui)

to also theme standard elements, such as, button and textblock etc.
