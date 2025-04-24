using UnityEngine;
using System;

public class Enemy : BaseEnemy
{
    protected override void Start()
    {
        maxHealth *= (float)Math.Pow(1.5f, gameSettings.currentLevel - 1);
        base.Start();
    }
}