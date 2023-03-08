using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIWindowMovement : MonoBehaviour
{
    private VisualElement rootVisualElement;
    private VisualElement windowHeader;
    private VisualElement templateScreen;
    private bool isDragging = false;

    private Vector2 originalMousePosition;

    public void InitializeMovementCallbacks(VisualElement windowRoot)
    {
        if (rootVisualElement != null)
            UninitializeMovementCallbacks();

        try {
            rootVisualElement = null;

            rootVisualElement = windowRoot;

            windowHeader = rootVisualElement.Q<VisualElement>("screen-header");

            templateScreen = rootVisualElement.Q<VisualElement>("template-screen");

            rootVisualElement.pickingMode = PickingMode.Ignore;
            templateScreen.pickingMode = PickingMode.Ignore;

            if (templateScreen == null)
                templateScreen = rootVisualElement;

            windowHeader.RegisterCallback<PointerDownEvent>(SetDragging);

            templateScreen.RegisterCallback<PointerDownEvent>(SetOriginalMousePosition);

            templateScreen.RegisterCallback<PointerMoveEvent>(DragScreen);

            templateScreen.RegisterCallback<PointerUpEvent>(UnsetDragging);
        } catch (System.NullReferenceException ex)
        {
            Debug.Log("Null reference... do you have a proper UI Document or Template in your game object?");
        }

    }

    public void UninitializeMovementCallbacks()
    {
        windowHeader.UnregisterCallback<PointerDownEvent>(SetDragging);

        templateScreen.UnregisterCallback<PointerDownEvent>(SetOriginalMousePosition);

        templateScreen.UnregisterCallback<PointerMoveEvent>(DragScreen);
    }

    public void SetDragging(PointerDownEvent ev)
    {
        isDragging = true;
    }

    public void UnsetDragging(PointerUpEvent ev)
    {
        isDragging = false;
    }

    public void SetOriginalMousePosition(PointerDownEvent ev)
    {
        originalMousePosition = ev.localPosition;
    }

    public void DragScreen(PointerMoveEvent ev)
    {
        float diffX = originalMousePosition.x - ev.localPosition.x;
        float diffY = originalMousePosition.y - ev.localPosition.y;
        if (isDragging)
        {
            (ev.currentTarget as VisualElement).style.left = (ev.currentTarget as VisualElement).style.left.value.value - diffX;
            (ev.currentTarget as VisualElement).style.top = (ev.currentTarget as VisualElement).style.top.value.value - diffY;
        }
    }


    void Update()
    {
        if (rootVisualElement != null && templateScreen != null)
        {
            if (isDragging)
            {
                rootVisualElement.pickingMode = PickingMode.Position;
                templateScreen.pickingMode = PickingMode.Position;
            }
            else
            {
                rootVisualElement.pickingMode = PickingMode.Ignore;
                templateScreen.pickingMode = PickingMode.Ignore;
            }
        }
    }
}
