using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


// Controller class to handle player movement buttons.
public class PlayerController : MonoBehaviour
{
    private ShipMovement shipMovement;  // The shipmovement script of the current prefab
    
    private Camera myCamera;

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


        if (shipMovement != null)
        {
            shipMovement.MoveShip(Input.GetAxis("Vertical"));
            shipMovement.TurnShip(Input.GetAxis("Horizontal"));
            shipMovement.PitchShip(Input.GetAxis("Pitch"));
            shipMovement.RollShip(Input.GetAxis("Roll"));
        }
        
    }

    void Update()
    {
        if (Input.GetKey("left ctrl"))
        {
            multiselect = true;
        }
        else
        {
            multiselect = false;
        }

        if (Input.GetKey("left alt"))
        {
            switchMain = true;
        }
        else 
        {
            switchMain = false;
        }

        if (Input.GetKeyDown("tab"))
        {
            VulturaInstance.selectorList.CycleOne();
        }

        if (Input.GetMouseButtonDown(0))
        {
            handleSelection(multiselect, switchMain);
        }

        if (Input.GetKeyDown("n"))
        {
            if (VulturaInstance.selectorList.mainSelected != null)
            {
                if (VulturaInstance.selectorList.mainSelected.selectableObject.tag == "Ship") 
                {
                    VulturaInstance.currentPlayer = VulturaInstance.selectorList.mainSelected.selectableObject.GetComponent<PrefabHandler>().SwitchControl(VulturaInstance.currentPlayer);
                }

                if (VulturaInstance.selectorList.mainSelected != null && VulturaInstance.selectorList.mainSelected.selectableObject.tag == "Station")
                {
                    VulturaInstance.selectorList.mainSelected.selectableObject.GetComponent<StationComponent>().EnterStation();
                }
            }
        }
    }

    public void WarpHandler(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            this.gameObject.GetComponent<PrefabHandler>().BeginWarp(VulturaInstance.selectorList.mainSelected.selectableObject);
        }
    }

    public void CancelWarp(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            this.gameObject.GetComponent<PrefabHandler>().CancelWarp();
        }
    }

    public void OpenInventory(InputAction.CallbackContext context)
    {
        if (context.started)
            InventoryManager.Instance.HandleInventory();
    }

    void handleSelection(bool multiSelect = false, bool switchMain = false)
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

                        VulturaInstance.selectorList.ConfirmSelection(hitInfo.collider.gameObject.GetComponent<PrefabHandler>().currShip, multiSelect, switchMain);
                    }
                    
                }
                else if (hitInfo.collider.gameObject.tag == "Station")
                {
                    
                    VulturaInstance.selectorList.ConfirmSelection(hitInfo.collider.gameObject.GetComponent<StationComponent>().station, multiSelect, switchMain);
                }
                
            }
        }
    }
}
