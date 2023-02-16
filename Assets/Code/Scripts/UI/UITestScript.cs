using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UITestScript : MonoBehaviour
{
    private Label counterLabel;
    private Button counterButton;

    private int count = 0;

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        counterLabel = rootVisualElement.Q<Label>("counter-label");
        counterButton = rootVisualElement.Q<Button>("counter-button");

        counterButton.RegisterCallback<ClickEvent>(ev => IncrementCounter());
    }

    private void IncrementCounter()
    {
        count++;

        counterLabel.text = "Count: " + count;
    }
}
