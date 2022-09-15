using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MiningStationUI : MonoBehaviour
{
    public VisualTreeAsset contactCard;

    public GameObject homeGameobject;
    public GameObject contactGameobject;

    public VisualElement homeRoot;
    public VisualElement contactRoot;

    public BaseStation station;

    void OnEnable()
    {

        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
    }

    public void Exit()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(false);
    }

    public void OpenUI(BaseStation stationObject)
    {
        station = stationObject;
        InitializeHome();

    }

    public void InitializeHome()
    {
        homeGameobject.SetActive(true);
        contactGameobject.SetActive(false);

        homeRoot = homeGameobject.GetComponent<UIDocument>().rootVisualElement;

        homeRoot.Q<Button>("button-exit").RegisterCallback<ClickEvent>(ev => { Exit();});
        homeRoot.Q<Button>("button-contacts").RegisterCallback<ClickEvent>(ev => {
            InitializeContacts();
        });
        homeRoot.Q<Label>("station-name").text = station.SelectableName;
    }

    public void InitializeContacts()
    {
        homeGameobject.SetActive(false);
        contactGameobject.SetActive(true);
        contactRoot = contactGameobject.GetComponent<UIDocument>().rootVisualElement;
        contactRoot.Q<Label>("station-name").text = station.SelectableName;

        VisualElement contactVisual = contactRoot.Q<VisualElement>("contact-list");

        contactRoot.Q<Button>("button-back").RegisterCallback<ClickEvent>(ev => {
            InitializeHome();
        });

        var stationHeadInstance = contactCard.Instantiate();
        stationHeadInstance.Q<Label>("contact-name").text = station.stationHead.Name;
        stationHeadInstance.Q<Label>("contact-type").text = VulturaInstance.enumStringParser(station.stationHead.Type.ToString());
        contactVisual.Add(stationHeadInstance);

        foreach (Contact contactObject in station.contacts)
        {
            var contactInstance = contactCard.Instantiate();
            contactInstance.Q<Label>("contact-name").text = contactObject.Name;
            contactInstance.Q<Label>("contact-type").text = VulturaInstance.enumStringParser(contactObject.Type.ToString());
            contactVisual.Add(contactInstance);
        }
    }
}
