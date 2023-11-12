
[System.Serializable]
public class GatheringQuestGoal : QuestGoal
{

    public ItemType ItemType1;
    public void ItemGathered(ItemType itemGathered)
    {
        if (goalType == GoalType.Kill && ItemType1 == itemGathered)
        {
            currentAmount++;
        }
    }
}
public enum ItemType
{
    Wings,
    Sword,
    Axe,
    Scythe,
    Magic,
    Heal
}
