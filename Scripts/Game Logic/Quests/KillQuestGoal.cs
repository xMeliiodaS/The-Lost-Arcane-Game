
[System.Serializable]
public class KillQuestGoal : QuestGoal
{
    public EnemyType EnemyType1; // Type of enemy to kill
    public void EnemyKilled(EnemyType killedEnemyType)
    {
        if (goalType == GoalType.Kill && EnemyType1 == killedEnemyType)
        {
            currentAmount++;
        }
    }
}

public enum EnemyType
{
    Goblin,
    Skeleton,
    Dragon,
    // Add more enemy types as needed
}
