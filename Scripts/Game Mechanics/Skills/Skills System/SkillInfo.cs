using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class SkillInfo : MonoBehaviour
{
    public float cooldownSlot = 5f;
    public bool isCooldown = false;
    public bool isAssigned = false;
    public bool isUsing;
    public float effectAnimationTime;
    public int effectNumber;
    public Transform handEffectTransform;
    public GameObject handEffect;
    public GameObject collisionEffect;

    public bool isSlashEffect;
    public bool isMagicEffect;

    public float slashEffectTimeToDisable;

    public bool isAroundPlayer;
    public bool isUpToDownSkill;
    public bool isDash;
    public ability ability;
}
public enum ability { teleport, timeManup, riftWalker, swiftSlasher}
