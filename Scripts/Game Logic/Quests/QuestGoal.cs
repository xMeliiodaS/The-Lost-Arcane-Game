[System.Serializable]
public class QuestGoal
{
    public GoalType goalType;

    public int requiredAmount;
    public int currentAmount;


    public GoalType GoalType
    {
        get { return goalType; }
        set { goalType = value; }
    }

    public int RequiredAmount
    {
        get { return requiredAmount; }
        set { requiredAmount = value; }
    }

    public int CurrentAmount
    {
        get { return currentAmount; }
        set { currentAmount = value; }
    }

    public void IncreaseCurrentAmount(int currentAmount)
    {
        this.currentAmount++;
    }

    public bool isReached()
    {
        return (currentAmount >=  requiredAmount);
    }

    public void EnemyKilled()
    {
        if(goalType == GoalType.Kill)
        {
            currentAmount++;
        }
    }

    public void ItemCollected()
    {
        if(goalType == GoalType.Gathering)
        {
            currentAmount++;
        }
    }
}

public enum GoalType
{
    Kill,
    Gathering
}