using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UITestScript : MonoBehaviour
{
    void Start()
    {

    }

    public void InitializeUI()
    {
        GetComponent<UIWindowMovement>().InitializeMovementCallbacks(this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui"));
        GetComponent<UIWindowMovement>().InitializeMovementCallbacks(this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui2"));
        //UIScreenManager.Instance.AddScreen(this.gameObject.GetComponent<UIDocument>());

        this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui").Q<VisualElement>("screen-background").RegisterCallback<PointerDownEvent>(ev => {
            UIScreenManager.Instance.SetFocusedScreen(this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui"));
            GetComponent<UIWindowMovement>().InitializeMovementCallbacks(this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui"));
        });
        this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui2").Q<VisualElement>("screen-background").RegisterCallback<PointerDownEvent>(ev => {
            UIScreenManager.Instance.SetFocusedScreen(this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui2"));
            GetComponent<UIWindowMovement>().InitializeMovementCallbacks(this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui2"));
        });
    }

    void Update()
    {
        if (UIScreenManager.Instance.focusedScreen != this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui"))
        {
            this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui").Q<VisualElement>("screen-background").style.opacity = 0.2f;
        }
        else
        {
            this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui").Q<VisualElement>("screen-background").style.opacity = 1.0f;
        }

        if (UIScreenManager.Instance.focusedScreen != this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui2"))
        {
            this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui2").Q<VisualElement>("screen-background").style.opacity = 0.2f;
        }
        else
        {
            this.gameObject.GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("testui2").Q<VisualElement>("screen-background").style.opacity = 1.0f;
        }
    }
}
