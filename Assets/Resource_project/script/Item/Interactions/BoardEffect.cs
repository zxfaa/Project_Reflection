using UnityEngine;

[CreateAssetMenu(fileName = "NewBoardEffect", menuName = "ItemEffects/BoardEffect")]
public class BoardEffect : ItemEffectBase
{
    public override void UseEffect(GameObject target)
    {
        // 播放動畫的邏輯
        Animator animator = target.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("PlayAnimation"); // 假設動畫使用的是一個名為"PlayAnimation"的Trigger
        }
        else
        {
            Debug.LogWarning("Target does not have an Animator component.");
        }

        // 獲取道具的邏輯
        InteractionHandler interactionHandler = target.GetComponent<InteractionHandler>();
        if (interactionHandler != null)
        {
            interactionHandler.PickUp();
        }
        else
        {
            Debug.LogWarning("Target does not have a InteractionHandler component.");
        }
        /*Inventory.Instance.AddItem(this.item); // 假設有一個Inventory管理類
        Debug.Log("Item used: " + this.item.itemName);*/
    }
}