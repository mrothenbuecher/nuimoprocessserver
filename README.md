# nuimoprocessserver

Companion application for [nuimo-foo](https://github.com/mrothenbuecher/nuimo-foo).
It's a simple ping server which response with the following JSON

```
{
  "process":"ApplicationFrameHost",
  "windowtitle":"NuimoFoo"
}

```
The name of the current focused process und the corresponding window title.
It runs on the localhost:1337.
