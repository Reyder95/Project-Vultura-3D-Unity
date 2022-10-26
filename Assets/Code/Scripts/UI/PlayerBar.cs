using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerBar : MonoBehaviour
{

    Label playerMoneyLabel;

    void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        playerMoneyLabel = rootVisualElement.Q<Label>("money-label");
    }

    void Update()
    {
        playerMoneyLabel.text = "$" + VulturaInstance.playerMoney.ToString();
    }

}
