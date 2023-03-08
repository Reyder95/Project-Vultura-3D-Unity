using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class OSUIHandler
{
    public StationUI uiComponent;
    public abstract void SetTaggedReferences(VisualElement screen, StationUI screenComponent);
    public abstract void SetCallbacks();
    public abstract VisualElement ReturnPage();
}
