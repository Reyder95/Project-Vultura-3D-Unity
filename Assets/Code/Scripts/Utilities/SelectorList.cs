using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that handles selections a player may make while clicking on objects
public class SelectorList
{
    // Different color selections
    private Color normalSelectedColor = new Color32(154, 30, 199, 255);
    private Color mainSelectedColor = new Color32(199, 30, 30, 255);
    
    public List<BaseSelectable> selected = new List<BaseSelectable>();      // The list of selected game objects
    public BaseSelectable mainSelected = null;      // The current main selected object

    // Buffer function. Determines what to do with the selection object based on the different flags and what object is being selected.
    public void ConfirmSelection(BaseSelectable selectedObject, bool multiSelect = false, bool switchMain = false)
    {
        bool doesExist = Exists(selectedObject);    // Holds a check whether or not an object exists in the list of selections.

        if (selectedObject.selectableObject != null)
        {
            // Normal selections, if we're not just switching our main selection.
            if (!switchMain)
            {
                // If the object exists and we have one object, deselect it.
                if (doesExist && selected.Count == 1)
                {
                    DeselectObject(selectedObject, multiSelect);
                    
                    EventManager.TriggerEvent("Selection Changed");
                    return;
                }

                // If it does exist and we have more thn one object
                if (doesExist && selected.Count > 1)
                {
                    // If CTRL is held
                    if (multiSelect) 
                    {
                        // Deselect the object
                        DeselectObject(selectedObject, multiSelect);
                        EventManager.TriggerEvent("Selection Changed");
                        return;
                    }
                    else
                    {
                        // If CTRL is not held, deselect all objects and only select that one.
                        DeselectAllObjects();
                        SelectObject(selectedObject, multiSelect);
                        EventManager.TriggerEvent("Selection Changed");
                        return;
                    }

                }
            }
            else
            {
                // If we are trying to switch the main selection, we check if the object exists, and we set that as the main.
                if (doesExist)
                {
                    if (selectedObject.GetType() == typeof(InstantiatedShip))
                    {
                        EventManager.TriggerEvent("Fleet Added");
                    }

                    SetMainSelected(selectedObject);
                    EventManager.TriggerEvent("Selection Changed");
                    return;
                }
            }

            // if none of the above, that means we're just trying to add an object to the selections.
            SelectObject(selectedObject, multiSelect);
            }

        EventManager.TriggerEvent("Selection Changed");
    }

    private void SelectObject(BaseSelectable selectedObject, bool multiSelect)
    {
        bool containsFleet = false;
        if (selectedObject.GetType() == typeof(InstantiatedShip))
             containsFleet = ContainsFleet(selectedObject.selectableObject.GetComponent<PrefabHandler>().fleetAssociation);
        // If we are not multi selecting, we want to deselect all objects so we can select one individual object
        if (!multiSelect)
        {
            DeselectAllObjects(containsFleet);
        }

        selected.Add(selectedObject);   // Add the object to-be selected to the list.

        if (selectedObject.GetType() == typeof(InstantiatedShip))
        {
            Debug.Log(selectedObject.selectableObject.GetComponent<PrefabHandler>().fleetAssociation.FleetGUID.ToString());
            if (!containsFleet)
                LoadFleetIntoFleetList(selectedObject.selectableObject);

            EventManager.TriggerEvent("Fleet Added");
        }
        selectedObject.Selected = true;

        SetMainSelected(selectedObject);    // Set the new object as the main selected object.    
    }

    private bool ContainsFleet(Fleet fleet)
    {
        foreach (BaseSelectable selectedItem in selected)
        {
            if (selectedItem.GetType() == typeof(InstantiatedShip))
            {
                if(selectedItem.selectableObject.GetComponent<PrefabHandler>().fleetAssociation.FleetGUID.CompareTo(fleet.FleetGUID) == 0)
                {
                    return true;
                }
                    
            }
        }
        return false;
    }

    private void LoadFleetIntoFleetList(GameObject shipPrefab)
    {
        
        Fleet fleet = shipPrefab.GetComponent<PrefabHandler>().fleetAssociation;

        VulturaInstance.fleetSelectables.Add(fleet.FleetCommander);
        
        foreach (InstantiatedShip ship in fleet.FleetShips)
        {
            VulturaInstance.fleetSelectables.Add(ship);
        }
    }

    private bool ContainsShip()
    {
        foreach (BaseSelectable item in selected)
        {
            if (item.GetType() == typeof(InstantiatedShip))
            {
                return true;
            }
        }

        return false;
    }

    private void DeselectObject(BaseSelectable selectedObject, bool multiSelect)
    {

        // Look through the entire selected list
        foreach (BaseSelectable deselector in selected)
        {
            // If the current object is the object that we're trying to deselect
            if (selectedObject.Equals(deselector))
            {
                selected.Remove(deselector);    // Remove it from the selected list
                selectedObject.selectableObject.GetComponent<Outline>().enabled = false;     // Turn off the outline
                selectedObject.Selected = false;

                // Check if what we deselected was a main selection
                if (mainSelected.Equals(deselector))
                {
                    mainSelected = null;
                    selectedObject.MainSelected = false;

                    // Switch main selection to last object if any objects remain in the list
                    if (selected.Count > 0)
                        SetMainSelected(selected[selected.Count - 1]);
                }

                if (!ContainsShip())
                {
                    VulturaInstance.fleetSelectables.Clear();
                    EventManager.TriggerEvent("Deselect Ship");
                }
                else 
                {
                    if (selectedObject.GetType() == typeof(InstantiatedShip))
                    {
                        
                        if (!ContainsFleet(selectedObject.selectableObject.GetComponent<PrefabHandler>().fleetAssociation))
                        {
                            int counter = 0;

                            while (counter < VulturaInstance.fleetSelectables.Count)
                            {
                                if (selectedObject.selectableObject.GetComponent<PrefabHandler>().fleetAssociation.FleetGUID.CompareTo(VulturaInstance.fleetSelectables[counter].selectableObject.GetComponent<PrefabHandler>().fleetAssociation.FleetGUID) == 0)
                                {
                                    VulturaInstance.fleetSelectables.RemoveAt(counter);
                                }
                                else
                                    counter++;
                            }

                            EventManager.TriggerEvent("Deselect Ship");
                            EventManager.TriggerEvent("Fleet Added");
                        }
                    }
                    
                }
                

                return;
            }
        }
    }

    // When tab is pressed, cycle the main selection one up. if we're at the last element, go back to the beginning
    public void CycleOne()
    {
        if (selected.Count > 0)
        {
            int mainIndex = selected.IndexOf(mainSelected);
        
            if (mainIndex == selected.Count - 1)
                SetMainSelected(selected[0]);
            else
                SetMainSelected(selected[mainIndex + 1]);

            EventManager.TriggerEvent("Fleet Added");
        }
        
    }

    // Set the new main selection
    private void SetMainSelected(BaseSelectable newSelected)
    {
        // If no main selection exists
        if (mainSelected == null)
        {
            newSelected.MainSelected = true;
            mainSelected = newSelected;     // Set the new main selection
            mainSelected.selectableObject.GetComponent<Outline>().enabled = true;    // Set the outline to true
            mainSelected.selectableObject.GetComponent<Outline>().OutlineColor = mainSelectedColor;  // Set the color
            return;
        }

        // If a main selection already exists
        mainSelected.selectableObject.GetComponent<Outline>().OutlineColor = normalSelectedColor;    // Set the current main selection to a normal color
        mainSelected.MainSelected = false;
        newSelected.MainSelected = true;
        newSelected.selectableObject.GetComponent<Outline>().enabled = true;                         // Set the outline on the new object to enabled
        newSelected.selectableObject.GetComponent<Outline>().OutlineColor = mainSelectedColor;       // Give the new object a main selection color
        mainSelected = newSelected; // Set the new object as the main selection
    }

    // Check if an element exists in the list
    private bool Exists(BaseSelectable selectedObject)
    {
        foreach (BaseSelectable myObject in selected)
        {
            if (myObject.Equals(selectedObject))
                return true;
        }

        return false;
    }

    // Deselect every object in the list
    public void DeselectAllObjects(bool containsFleet = false)
    {
        if (!containsFleet)
        {
            VulturaInstance.fleetSelectables.Clear();
        
            EventManager.TriggerEvent("Deselect Ship");
        }

        for (int i = 0; i < selected.Count; i++)
        {
            selected[i].selectableObject.GetComponent<Outline>().enabled = false;
            selected[i].Selected = false;
            selected[i].MainSelected = false;
        }

        selected.Clear();

        mainSelected = null;
    }
}
