using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerPickup : MonoBehaviour
{

    [Header("직접 연결")]

    [SerializeField] private Animator animator;

    [SerializeField] private PlayerController playerController;

    [Header("아이템 탐색")]

    [SerializeField] private float detectionRadius = 2f;

    [Header("줍기 모션")]

    [SerializeField] private float pickupDuration = 1.2f;
    [SerializeField] private float pickupMoment = 0.55f;

    private PickupItem targetItem;
    private bool isPickingUp;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickingUp) return;

        targetItem = FindClosestItem();

        if (targetItem == null) return;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null) return;

        if (keyboard.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PickupAnimation());
        }
    }

    private PickupItem FindClosestItem()
    {
        Collider[]colliders = Physics.OverlapSphere(transform.position, detectionRadius, ~0, QueryTriggerInteraction.Collide);
        PickupItem clossestItem = null;
        float clossestDistance = Mathf.Infinity;

        foreach(Collider itemCollider in colliders)
        {
            PickupItem item = itemCollider.GetComponent<PickupItem>();

            if (item == null) continue;

            float distance = Vector3.Distance(transform.position, item.transform.position);

            if (distance < clossestDistance)
            {
                clossestDistance = distance;
                clossestItem = item;
            }
        }
        return clossestItem;
    }

    private IEnumerator PickupAnimation()
    {
        isPickingUp = true;

        PickupItem itemToCollect = targetItem;

        Vector3 itemDircection = itemToCollect.transform.position - transform.position;
        itemDircection.y = 0; 

        if(itemDircection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(itemDircection);
        }

        playerController.ChangeState(PlayerState.Pickup);

        animator.CrossFade("PickUp01", 0.1f);

        yield return new WaitForSeconds(pickupMoment);

        if(itemToCollect != null)
        {
            itemToCollect.Collect();
        }

        yield return new WaitForSeconds(pickupDuration - pickupMoment);

        animator.CrossFade("Blend Tree", 0.1f);

        playerController.ChangeState(PlayerState.Normal);

        targetItem = null;
        isPickingUp = false;

    }


}
