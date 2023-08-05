using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class that handles selections a player may make while clicking on objects
public class SelectorList
{
    // Different color selections
    private Color normalSelectedColor = new Color32(154, 30, 199, 255);
    private Color mainSelectedColor = new Color32(199, 30, 30, 255);
    
    public List<SelectableEntity> selected = new List<SelectableEntity>();      // The list of selected game objects
    public SelectableEntity mainSelected = null;      // The current main selected object

    // Buffer function. Determines what to do with the selection object based on the different flags and what object is being selected.
    public void ConfirmSelection(SelectableEntity selectedEntity, bool multiSelect = false, bool switchMain = false)
    {
        bool doesExist = Exists(selectedEntity);    // Holds a check whether or not an object exists in the list of selections.

        if (selectedEntity.entity != null)
        {
            // Normal selections, if we're not just switching our main selection.
            if (!switchMain)
            {
                // If the object exists and we have one object, deselect it.
                if (doesExist && selected.Count == 1)
                {
                    Debug.Log("Test!!");
                    DeselectObject(selectedEntity, multiSelect);
                    
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
                        DeselectObject(selectedEntity, multiSelect);
                        EventManager.TriggerEvent("Selection Changed");
                        return;
                    }
                    else
                    {
                        // If CTRL is not held, deselect all objects and only select that one.
                        DeselectAllObjects();
                        SelectObject(selectedEntity, multiSelect);
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
                    if (selectedEntity.entity.entity != null)
                        if (selectedEntity.entity.entity.GetType() == typeof(InstantiatedShip))
                        {
                            EventManager.TriggerEvent("Fleet Added");
                        }

                    SetMainSelected(selectedEntity);
                    EventManager.TriggerEvent("Selection Changed");
                    return;
                }
            }

            // if none of the above, that means we're just trying to add an object to the selections.
            SelectObject(selectedEntity, multiSelect);
            }

        EventManager.TriggerEvent("Selection Changed");
    }

    private void SelectObject(SelectableEntity selectedEntity, bool multiSelect)
    {
        bool containsFleet = false;
        if (selectedEntity.entity.entity != null)
            if (selectedEntity.entity.entity.GetType() == typeof(InstantiatedShip))
                containsFleet = ContainsFleet(selectedEntity.entity.entity.selectableObject.GetComponent<PrefabHandler>().fleetAssociation);

        bool containsSubBodies = false;

        containsSubBodies = ContainsSubEntities(selectedEntity);

        if (Exists(selectedEntity))
            DeselectObject(selectedEntity, multiSelect);
        else if (!multiSelect)
        {
            DeselectAllObjects(containsFleet, containsSubBodies);
        }

        // If we are not multi selecting, we want to deselect all objects so we can select one individual object


        selected.Add(selectedEntity);   // Add the object to-be selected to the list.

        if (selectedEntity.entity.entity != null)
            if (selectedEntity.entity.entity.GetType() == typeof(InstantiatedShip))
            {
                LoadFleetIntoFleetList(selectedEntity.entity.entity.selectableObject);

                EventManager.TriggerEvent("Fleet Added");
            }
            
        if (selectedEntity.entity.subEntities.Count > 0)
        {
            LoadSubEntities(selectedEntity);
            EventManager.TriggerEvent("Fleet Added");
        }

        selectedEntity.selected = true;

        SetMainSelected(selectedEntity);    // Set the new object as the main selected object.    
    }

    private void LoadSubEntities(SelectableEntity selectedEntity)
    {
        foreach (SystemEntity entity in selectedEntity.entity.subEntities)
        {
            VulturaInstance.AddSubEntityToSystem(entity);
        }
    }

    private bool ContainsFleet(Fleet fleet)
    {
        foreach (SelectableEntity selectedItem in selected)
        {
            if (selectedItem.entity.entity != null)
                if (selectedItem.entity.entity.GetType() == typeof(InstantiatedShip))
                {
                    if(selectedItem.entity.entity.selectableObject.GetComponent<PrefabHandler>().fleetAssociation.FleetGUID.CompareTo(fleet.FleetGUID) == 0)
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

        VulturaInstance.AddSubSelectableToSystem(fleet.FleetCommander);
        
        foreach (InstantiatedShip ship in fleet.FleetShips)
        {
            VulturaInstance.AddSubSelectableToSystem(ship);
        }
    }

    private bool ContainsShip()
    {
        foreach (SelectableEntity item in selected)
        {
            if (item.entity.entity.GetType() == typeof(InstantiatedShip))
            {
                return true;
            }
        }

        return false;
    }

    private bool ContainsSubEntities(SelectableEntity entity)
    {
        if (entity.entity.subEntities.Count > 0)
            return true;

        return false;
    }

    private void DeselectObject(SelectableEntity selectedEntity, bool multiSelect)
    {

        // Look through the entire selected list
        foreach (SelectableEntity deselector in selected)
        {
            // If the current object is the object that we're trying to deselect
            if (selectedEntity.Equals(deselector))
            {
                Debug.Log("IN DESELECT!");
                selected.Remove(deselector);    // Remove it from the selected list
                if (selectedEntity.entity.entity != null)
                    selectedEntity.entity.entity.selectableObject.GetComponent<Outline>().enabled = false;     // Turn off the outline
                selectedEntity.selected = false;

                // Check if what we deselected was a main selection
                if (mainSelected.Equals(deselector))
                {
                    mainSelected = null;
                    selectedEntity.mainSelected = false;

                    // Switch main selection to last object if any objects remain in the list
                    if (selected.Count > 0)
                        SetMainSelected(selected[selected.Count - 1]);
                }

                //VulturaInstance.subEntities.Clear();

                if (!ContainsShip() && !ContainsSubEntities(selectedEntity))
                {
                    VulturaInstance.subEntities.Clear();
                    EventManager.TriggerEvent("Deselect Ship");
                }
                else 
                {
                    Debug.Log("Test!");
                    if (selectedEntity.entity.entity != null)
                        if (selectedEntity.entity.entity.GetType() == typeof(InstantiatedShip))
                        {
                            
                            if (!ContainsFleet(selectedEntity.entity.entity.selectableObject.GetComponent<PrefabHandler>().fleetAssociation))
                            {
                                int counter = 0;

                                    while (counter < VulturaInstance.subEntities.Count)
                                    {
                                        if (selectedEntity.entity.entity.selectableObject.GetComponent<PrefabHandler>().fleetAssociation.FleetGUID.CompareTo(VulturaInstance.subEntities[counter].entity.entity.selectableObject.GetComponent<PrefabHandler>().fleetAssociation.FleetGUID) == 0)
                                        {
                                            VulturaInstance.subEntities.RemoveAt(counter);
                                        }
                                        else
                                            counter++;
                                    }

                                    EventManager.TriggerEvent("Deselect Ship");
                                    EventManager.TriggerEvent("Fleet Added");
                            }
                        }

                    if (ContainsSubEntities(selectedEntity))
                    {
                        Debug.Log("Test!");
                        int counter = 0;

                        SystemEntity mainEntity = selectedEntity.entity.mainEntity;

                        if (mainEntity != null)
                        {
                            VulturaInstance.RemoveSubEntitiesOfMainEntity(mainEntity);
                        }
                        else
                        {
                            Debug.Log("Another Test");
                            VulturaInstance.RemoveSubEntitiesOfMainEntity(selectedEntity.entity);
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
        EventManager.TriggerEvent("RefreshList");    // Calls the "Cycle Ship" event which is received by the entity lists
    }

    // Set the new main selection
    private void SetMainSelected(SelectableEntity newSelected)
    {
        try
        {
            // If no main selection exists
            if (mainSelected == null)
            {
                newSelected.mainSelected = true;
                mainSelected = newSelected;     // Set the new main selection

                if (mainSelected.entity.entity != null)
                {
                    mainSelected.entity.entity.selectableObject.GetComponent<Outline>().enabled = true;    // Set the outline to true
                    mainSelected.entity.entity.selectableObject.GetComponent<Outline>().OutlineColor = mainSelectedColor;  // Set the color
                }
                return;
            }

            // If a main selection already exists

            if (mainSelected.entity.entity != null)
                mainSelected.entity.entity.selectableObject.GetComponent<Outline>().OutlineColor = normalSelectedColor;    // Set the current main selection to a normal color
            
            mainSelected.mainSelected = false;
            newSelected.mainSelected = true;

            if (newSelected.entity.entity != null)
            {
                newSelected.entity.entity.selectableObject.GetComponent<Outline>().enabled = true;                         // Set the outline on the new object to enabled
                newSelected.entity.entity.selectableObject.GetComponent<Outline>().OutlineColor = mainSelectedColor;       // Give the new object a main selection color
            }

            mainSelected = newSelected; // Set the new object as the main selection
        } catch (System.NullReferenceException) 
        {
        }

    }

    // Check if an element exists in the list
    private bool Exists(SelectableEntity selectedObject)
    {
        foreach (SelectableEntity myObject in selected)
        {
            if (myObject.Equals(selectedObject))
                return true;
        }

        return false;
    }

    public void RemoveSubShipsFromList()
    {

    }

    // Deselect every object in the list
    public void DeselectAllObjects(bool containsFleet = false, bool containsSubEntities = false)
    {
        Debug.Log(containsFleet);

        //VulturaInstance.subEntities.Clear();
        // if (!containsFleet && !containsSubEntities)
        // {
        //     VulturaInstance.subEntities.Clear();
        
        //     EventManager.TriggerEvent("Deselect Ship");
        // }


        VulturaInstance.subEntities.Clear();
        

        for (int i = 0; i < selected.Count; i++)
        {
            if (selected[i].entity != null)
            {
                if (selected[i].entity.entity != null)
                    selected[i].entity.entity.selectableObject.GetComponent<Outline>().enabled = false;
                selected[i].selected = false;
                selected[i].mainSelected = false;
            }

            
        }

        selected.Clear();

        mainSelected = null;
    }
}
