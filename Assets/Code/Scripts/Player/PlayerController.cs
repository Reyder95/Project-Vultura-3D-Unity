using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// Controller class to handle player movement buttons.
public class PlayerController : MonoBehaviour
{
    private ShipMovement shipMovement;  // The shipmovement script of the current prefab
    
    private Camera myCamera;    // The camera on the player

    //public SelectorList selectorList = new SelectorList();  // A list of selected objects. Handles the selecting and deselecting of objects.
    private int mainSelected;           // The main selected index

    private bool multiselect = false;
    private bool switchMain = false;

    void Start()
    {
        shipMovement = this.gameObject.transform.GetComponent<ShipMovement>();
        //myCamera = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        // Move the ship if the player is allowed to move
        if (shipMovement != null && VulturaInstance.playerStatus == VulturaInstance.PlayerStatus.SPACE)
        {
            shipMovement.MoveShip(Input.GetAxis("Vertical"));
            shipMovement.TurnShip(Input.GetAxis("Horizontal"));
            shipMovement.PitchShip(Input.GetAxis("Pitch"));
            shipMovement.RollShip(Input.GetAxis("Roll"));
        }
        
    }

    void Update()
    {
        // Left CTRL lets you select multiple items
        if (Input.GetKey("left ctrl"))
        {
            multiselect = true;
        }
        else
        {
            multiselect = false;
        }

        // DEBUG
        if (Input.GetKeyDown("y"))
        {
            VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.ClearInventory();
            Game.Instance.GenerateInventory();
        }

        // Left Alt switches the main selection out of the list of currently selected items
        if (Input.GetKey("left alt"))
        {
            switchMain = true;
        }
        else 
        {
            switchMain = false;
        }

        // Tab lets you switch between items you have selected
        if (Input.GetKeyDown("tab"))
        {
            VulturaInstance.selectorList.CycleOne();
        }

        // When clicking on an item, it will handle a selection.
        if (Input.GetMouseButtonDown(0))
        {
            handleSelection(multiselect, switchMain);
            PickUpItem();
            
        }

        // Activate the item that is main selected
        if (Input.GetKeyDown("n"))
        {
            if (VulturaInstance.selectorList.mainSelected != null)
            {
                if (VulturaInstance.selectorList.mainSelected.entity.entity.selectableObject.tag == "Ship") 
                {
                    VulturaInstance.currentPlayer = VulturaInstance.selectorList.mainSelected.entity.entity.selectableObject.GetComponent<PrefabHandler>().SwitchControl(VulturaInstance.currentPlayer);
                }

                if (VulturaInstance.selectorList.mainSelected != null && VulturaInstance.selectorList.mainSelected.entity.entity.selectableObject.tag == "Station")
                {
                    VulturaInstance.selectorList.mainSelected.entity.entity.selectableObject.GetComponent<StationComponent>().EnterStation();
                }
            }
        }

        // Temp mining solution
        if (Input.GetKeyDown("f"))
        {
            if (VulturaInstance.selectorList.mainSelected != null)
            {
                if (VulturaInstance.selectorList.mainSelected.entity.entity.selectableObject.tag == "Asteroid")
                {
                    InstantiatedShip playerShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;

                    if (playerShip.turretMounts.Count != 0)
                        playerShip.turretMounts[0].GetComponent<MountComponent>().UseTurret(VulturaInstance.selectorList.mainSelected.entity.entity);

                }
            }
            else
            {
                InstantiatedShip playerShip = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip;
                    if (playerShip.turretMounts.Count != 0)
                        playerShip.turretMounts[0].GetComponent<MountComponent>().StopTurret();
            }
        }

        if (Input.GetKeyDown("p"))
        {
            VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.turretMounts[0].SetActive(true);
            VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.turretMounts[0].GetComponent<MountComponent>().EquipTurret(ItemManager.GenerateRandomBaseFromCategory("chaingun") as ActiveModule);
            
        }

        if (Input.GetKeyDown("c"))
        {
            EventManager.TriggerEvent("ship-screen UI Open");
        }
    }

    // Handle the warp when warp button is pressed
    public void WarpHandler(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            this.gameObject.GetComponent<PrefabHandler>().BeginWarp(VulturaInstance.selectorList.mainSelected.entity.entity.selectableObject);
        }
    }

    // Cancel the warp
    public void CancelWarp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            this.gameObject.GetComponent<PrefabHandler>().CancelWarp();
        }
    }

    // Open the inventory
    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (context.started)
            EventManager.TriggerEvent("inventory UI Open");
    }

    void PickUpItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {
            if (hitInfo.collider.gameObject.tag == "Dropped Item")
            {
                bool success = VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip.Cargo.Add(hitInfo.collider.gameObject.GetComponent<ItemGround>().GetItem(), VulturaInstance.currentPlayer.GetComponent<PrefabHandler>().currShip);

                if (success)
                    Destroy(hitInfo.collider.gameObject);
            }
        }
    }

    // Select the item requested based on boolean values
    void handleSelection(bool multiSelect = false, bool switchMain = false)
    {
        // Let the player select objects in the world if they are not over a UI element (or they are in the world at all)
        if (!UI_Manager.IsPointerOverUI(Input.mousePosition) && VulturaInstance.playerStatus == VulturaInstance.PlayerStatus.SPACE)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {

                if (hitInfo.collider.gameObject.GetComponent<Selectable>() != null)
                {
                    if (hitInfo.collider.gameObject.tag == "Ship")
                    {

                        if (hitInfo.collider.gameObject.GetComponent<PrefabHandler>().currShip != null)
                        {

                            VulturaInstance.selectorList.ConfirmSelection(hitInfo.collider.gameObject.GetComponent<PrefabHandler>().currShip.entity.aboveEntity, multiSelect, switchMain);
                        }
                    
                    }
                    else if (hitInfo.collider.gameObject.tag == "Station")
                    {
                    
                        VulturaInstance.selectorList.ConfirmSelection(hitInfo.collider.gameObject.GetComponent<StationComponent>().station.entity.aboveEntity, multiSelect, switchMain);
                    }
                    else if (hitInfo.collider.gameObject.tag == "Asteroid")
                    {
                        Debug.Log(hitInfo.collider.gameObject.GetComponent<Asteroid>().CurrAsteroid);
                        VulturaInstance.selectorList.ConfirmSelection(hitInfo.collider.gameObject.GetComponent<Asteroid>().CurrAsteroid.entity.aboveEntity, multiselect, switchMain);
                    }
                
                }
            }
        }
        
    }
}
