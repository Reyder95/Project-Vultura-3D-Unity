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

    public void InitializeMovementCallbacks()
    {
        try {
            rootVisualElement = this.gameObject.GetComponent<UIDocument>().rootVisualElement;

            windowHeader = rootVisualElement.Q<VisualElement>("screen-header");

            templateScreen = rootVisualElement.Q<VisualElement>("template-screen");

            windowHeader.RegisterCallback<PointerDownEvent>(ev => {
                isDragging = true;
            });

            templateScreen.RegisterCallback<PointerDownEvent>(ev => {
                originalMousePosition = ev.localPosition;
            });

            templateScreen.RegisterCallback<PointerMoveEvent>(ev => {
                float diffX = originalMousePosition.x - ev.localPosition.x;
                float diffY = originalMousePosition.y - ev.localPosition.y;

                if (isDragging)
                {
                    Debug.Log("Diff X: " + diffX);
                    Debug.Log("Diff Y: " + diffY);

                    templateScreen.style.left = templateScreen.style.left.value.value - diffX;
                    templateScreen.style.top = templateScreen.style.top.value.value - diffY;
                }

            });

            templateScreen.RegisterCallback<PointerUpEvent>(ev => {
                isDragging = false;
            });
        } catch (System.NullReferenceException ex)
        {
            Debug.Log("Null reference... do you have a proper UI Document or Template in your game object?");
        }

    }

}
