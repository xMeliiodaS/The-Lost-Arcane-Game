using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAttack : StateMachineBehaviour
{

    public MonoBehaviour script; // Reference to the MonoBehaviour script tp use "Coroutine"

    private float speed = 0;
    private float originalSpeed = 6;
    private float rangeToAttack;
    private int counter = 0;
    private bool changedAnimation = false;
    Transform player;

    float currentWeight = 1.0f;
    float targetWeight = 0.0f;

    float delayDuration = 0.70f; // 0.20 seconds delay
    float elapsedDelayTime = 0.0f;
    float blendSpeed = 1.0f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        changedAnimation = false;
        rangeToAttack = animator.GetComponent<Enemy>().GetRangeToAttack();
    }
    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<NavMeshAgent>().speed = speed;

        if(player == null)
        {
            player = FindPlayer.Instance.Player.transform;
        }

        float distanceSquared = (player.position - animator.transform.position).sqrMagnitude;
        if (distanceSquared >  rangeToAttack * rangeToAttack && !changedAnimation)
        {
            changedAnimation = true;
            while (currentWeight > targetWeight)
            {
                currentWeight -= Time.deltaTime * blendSpeed;

                // Accumulate elapsed delay time
                elapsedDelayTime += Time.deltaTime;

                if (elapsedDelayTime >= delayDuration)
                {
                    // Start the "Chasing" animation
                    animator.SetBool("isChasing", true);
                    animator.SetBool("isWalking", false);
                    animator.SetBool("isIdle", false);
                    //animator.Play("Chasing", layerIndex, 0f);
                    break; // Exit the loop after the delay
                }
            }

            //script.StartCoroutine(ChangeAnimationToChasing(animator, layerIndex));
        }



        if (animator.gameObject.name.Contains("Enemy") && counter == 0)
        {
            AudioManager.Instance.Play(GetRandomAttackEffectSFX());
            counter++;
        }
        else if (animator.gameObject.name.Contains("Boss") && counter == 0)
        {
            AudioManager.Instance.Play(GetRandomAttackBossSFX());
            counter++;
        }

    }


    IEnumerator ChangeAnimationToChasing(Animator animator, int layerIndex)
    {   // If the player is far away while this animation is playing, stop and and chase the player
        //animator.SetBool("isIdle", true);
        Debug.Log("PLaying 312");

        yield return new WaitForSeconds(0.15f);
        Debug.Log("PLaying ???");
        animator.Play("Chasing", layerIndex, 0f); // Play the target state immediately
        animator.GetComponent<Enemy>().enabled = false;
        animator.GetComponent<RandomEnemyMovements>().enabled = false;
        animator.GetComponent<EnemyController>().enabled = true;

        //animator.GetComponent<NavMeshAgent>().speed = originalSpeed;
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        float distanceSquared = (player.position - animator.transform.position).sqrMagnitude;
        if (distanceSquared > rangeToAttack * rangeToAttack)
        {
            animator.SetBool("isChasing", true);
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);

            animator.GetComponent<Enemy>().enabled = false;
            animator.GetComponent<RandomEnemyMovements>().enabled = false;
            animator.GetComponent<EnemyController>().enabled = true;

        }
        animator.GetComponent<NavMeshAgent>().speed = originalSpeed;
    }


    public string GetRandomAttackEffectSFX()
    {
        string[] attackNames = { "AttackEffect1", "AttackEffect2", "AttackEffect3" };
        int randomIndex = Random.Range(0, attackNames.Length);
        return attackNames[randomIndex];
    }

    public string GetRandomAttackBossSFX()
    {
        string[] attackNames = { "BossAttackEffect1", "BossAttackEffect2",};
        int randomIndex = Random.Range(0, attackNames.Length);
        return attackNames[randomIndex];
    }


}
