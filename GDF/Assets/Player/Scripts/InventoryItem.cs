using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField]
    public int quantity;
    [SerializeField]
    public string itemName;
    [SerializeField]
    public Sprite itemIcon;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PickMeUp(other.gameObject.GetComponent<SimpleInventory>());
        }
    }

    private void PickMeUp(SimpleInventory simpleInventory)
    {
        simpleInventory.AddItem(this);
        Destroy(this.gameObject);
    }
}
