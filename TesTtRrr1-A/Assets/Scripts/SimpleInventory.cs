using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
public class SimpleInventory : MonoBehaviour
{
    [Header("UI연결")]

    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private TextMeshProUGUI[] slotText;
    [SerializeField] private TextMeshProUGUI selectedItemText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private int healAmount = 30;

    private List <InventoryItem> items = new List<InventoryItem>();

    private PlayerController playerController;

    private PlayerPickup playerPickup;

    private ThirdPersonCamera thirdPersonCamera;

    private bool isOpen;

    [SerializeField] private int selectedIndex = -1; // 4개 
    // -1 : 선택없음, 0 첫 번째 슬롯, 1 두 번째 슬롯 ~> 4


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerPickup = GetComponent<PlayerPickup>();

        if (Camera.main != null )
        {
            thirdPersonCamera = Camera.main.GetComponent<ThirdPersonCamera>();
        }

        inventoryPanel.SetActive(false);
    }


    void Start()
    {
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        if (isOpen == false && playerController.IsNomalState() == false)
        {
            return;
        }
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if(isOpen)
        {
            playerController.ChangeState(PlayerState.inventory);
            playerPickup.enabled = false;

            if(thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = false;
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playerController.ChangeState(PlayerState.Normal);
            playerPickup.enabled = true;

            if (thirdPersonCamera != null)
            {
                thirdPersonCamera.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    private void UpdateUI()
    {
        for(int i = 0; i < slotText.Length; i++)
        {
            if(i <items.Count)
            {
                slotText[i].text = items[i]. itemName + "\n" + items[i].amount + "개";
            }
            else
            {
                slotText[i].text = "비어있음";
            }
        }
        if (selectedIndex >=  0 && selectedIndex < items.Count)
        {
            selectedItemText.text = "선택 : " + items[selectedIndex].itemName;
            descriptionText.text = items[selectedIndex].description;

            useButton.interactable = CanUseItem(items[selectedIndex].itemName);
            dropButton.interactable = true;
        }
        else
        {
            selectedItemText.text = "아이템을 선택하세요";
            descriptionText.text = "슬롯을 클릭하면 설명이 표시됩니다.";

            useButton.interactable = false;
            dropButton.interactable = false;
        }
    }

    public bool AddItem(string itemName, int amount, string description)
    {
        foreach(InventoryItem item in items)
        {
            if(item.itemName == itemName)
            {
                item.amount += amount;
                UpdateUI();
                return true;
            }
        }
        if(items.Count >= slotText.Length)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        InventoryItem newItem = new InventoryItem();
        newItem.itemName = itemName;
        newItem.amount = amount;
        newItem.description = description;

        items.Add(newItem);
        UpdateUI();

        return true;
        
    }

    //슬롯 선택 함수 
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;

        UpdateUI();
    }

    public void SelectSlot0() { SelectSlot(0); }
    public void SelectSlot1() { SelectSlot(1); }
    public void SelectSlot2() { SelectSlot(2); }
    public void SelectSlot3() { SelectSlot(3); }

    private bool CanUseItem(string itemName)
    {
        return itemName == "물약" || itemName == "회복약" || itemName == "구급키트";
    }

    public bool RemoveItem(string itemName, int amount)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemName != itemName) continue;
            if (items[i]. amount <  amount) return false;

            items[i].amount -= amount;

            if(items[i].amount <= 0)
            {
                items.RemoveAt(i);
                selectedIndex = -1;
            }
            UpdateUI();
            return true;
        }
        return false;
    }

    public void UseSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        InventoryItem item = items[selectedIndex];

        if (CanUseItem(item.itemName) == false)
        {
            selectedItemText.text = item.itemName + "은(는) 사용할 수 없습니다."; return;
        }

        playerHealth.Heal(healAmount);
        RemoveItem(item.itemName, 1);
    }
    
    public void DropSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        string itemName = items[selectedIndex].itemName;

        RemoveItem(itemName, 1);
    }

    public int GetItemAmount(string itemName)
    {
        foreach (InventoryItem item in items)
        {
            if (item.itemName == itemName)
            {
                return item.amount;
            }
        }
        return 0;
    }
}
