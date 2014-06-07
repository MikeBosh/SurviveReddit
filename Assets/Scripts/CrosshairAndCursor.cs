using UnityEngine;

public class CrosshairAndCursor : MonoBehaviour
{
    public Texture2D crosshairImage;
    public Texture2D cursorImage;
    public Texture2D interactImage;
	public Texture2D repairImage;

    private Inventory mInventory;
    private bool mIsInteractable;
    private bool mIsPickup;
	private bool mIsRepair;
    private PlayerController mPlayerController;
    public Texture2D pickupImage;

    private void Start()
    {
        mInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        mPlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        Screen.showCursor = false;
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, mPlayerController.UseDistance))
        {
            if (hit.transform.gameObject != null)
            {
                if (hit.transform.gameObject.tag == "Pick Up")
                {
                    mIsPickup = true;
                    mIsInteractable = false;
					mIsRepair = false;
                }
                else if (hit.transform.gameObject.tag == "Interactable")
                {
                    mIsPickup = false;
                    mIsInteractable = true;
					mIsRepair = false;
                }
				else if(hit.transform.gameObject.tag == "Repair"){

					mIsInteractable = false;
					mIsPickup = false;
					mIsRepair = true;
				}
                else
                {
                    mIsPickup = false;
                    mIsInteractable = false;
					mIsRepair = false;
                }
            }
        }
        else
        {
            mIsPickup = false;
            mIsInteractable = false;
			mIsRepair = false;
        }
    }

    private void OnGUI()
    {
        GUI.depth = 0;

        if (mInventory.IsInventoryShowing || mPlayerController.IsMenuShowing)
        {
            Screen.lockCursor = false;
            float mouseX = Event.current.mousePosition.x - (cursorImage.width/2);
            float mouseY = Event.current.mousePosition.y - (cursorImage.height/2);
            GUI.DrawTexture(new Rect(mouseX, mouseY, cursorImage.width, cursorImage.height), cursorImage);
        }
        else if (mIsPickup)
        {
            Screen.lockCursor = true;
            float xMin = (Screen.width/2) - (pickupImage.width/2);
            float yMin = (Screen.height/2) - (pickupImage.height/2);
            GUI.DrawTexture(new Rect(xMin, yMin, pickupImage.width, pickupImage.height), pickupImage);
        }
        else if (mIsInteractable)
        {
            Screen.lockCursor = true;
            float xMin = (Screen.width/2) - (interactImage.width/2);
            float yMin = (Screen.height/2) - (interactImage.height/2);
            GUI.DrawTexture(new Rect(xMin, yMin, interactImage.width, interactImage.height), interactImage);
        }
		else if (mIsRepair)
		{
			Screen.lockCursor = true;
			float xMin = (Screen.width/2) - (interactImage.width/2);
			float yMin = (Screen.height/2) - (interactImage.height/2);
			GUI.DrawTexture(new Rect(xMin, yMin, repairImage.width, repairImage.height), repairImage);
		}
        else
        {
            Screen.lockCursor = true;
            float xMin = (Screen.width/2) - (crosshairImage.width/2);
            float yMin = (Screen.height/2) - (crosshairImage.height/2);
            GUI.DrawTexture(new Rect(xMin, yMin, crosshairImage.width, crosshairImage.height), crosshairImage);
        }
    }
}