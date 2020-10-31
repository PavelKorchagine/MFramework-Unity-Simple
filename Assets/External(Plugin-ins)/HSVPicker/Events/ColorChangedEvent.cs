using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class ColorChangedEvent : UnityEvent<Color>
{

}

[Serializable]
public class ColorChangedBeginEvent : UnityEvent<Color>
{

}

[Serializable]
public class ColorChangedEndEvent : UnityEvent<Color>
{

}