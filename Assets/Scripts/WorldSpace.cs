using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpace {

    public int gold = 0;
    public string cannon = "";
    public bool hasDefense = false;
    public bool defenseIsRepaired = false;
    public float enemyLife = 100f;
    public int bullet = 0;


    internal void PickUpMine()
    {
        gold += 20;
    }

    internal bool CanCreateDefense()
    {
        return gold > 20 && hasDefense==false;
    }

    internal bool CanCreateCannon()
    {
        return gold > 20 && cannon == "";
    }

    internal bool CanUpgradeCannon()
    {
        return gold > 40 && cannon == "cannon";
    }
    internal bool CanAttack()
    {
        return bullet>0 && cannon == "cannon";
    }

    internal bool CanSuperAttack()
    {
        return bullet > 0 && cannon == "cannon upgraded";
    }

    internal bool CanCreateWorktable()
    {
        return gold > 10;
    }


    internal void CreateDefense()
    {
        gold -= 20;
        hasDefense = true;
    }

    internal bool PassiveComparison(WorldSpace other)
    {
        return this.hasDefense == other.hasDefense;
    }

    internal bool AgressiveComparison(WorldSpace other)
    {
        return this.enemyLife == other.enemyLife && this.cannon == other.cannon;
    }

    internal bool ReachGoal(WorldSpace other)
    {
        return this == other;

    }

    internal void CreateWorktable()
    {
        gold -= 10;
        bullet ++;
    }

    public override bool Equals(object value)
    {
        WorldSpace other = value as WorldSpace;

        return gold == other.gold
            && bullet == other.bullet
            && hasDefense == other.hasDefense
            && defenseIsRepaired == other.defenseIsRepaired
            && cannon == other.cannon
            && enemyLife == other.enemyLife;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // Choose large primes to avoid hashing collisions
            const int HashingBase = (int)2166136261;
            const int HashingMultiplier = 16777619;

            int hash = HashingBase;
            hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, gold) ? gold.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, bullet) ? bullet.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, hasDefense) ? hasDefense.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, defenseIsRepaired) ? defenseIsRepaired.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, cannon) ? cannon.GetHashCode() : 0);
            hash = (hash * HashingMultiplier) ^ (!ReferenceEquals(null, enemyLife) ? enemyLife.GetHashCode() : 0);
            return hash;
        }
    }

    internal float PassiveHeuristic(WorldSpace other)
    {
        
        int h_value = 2;
        if (!hasDefense) h_value--;
        if (!defenseIsRepaired) h_value--;
        return h_value;
    }

    internal void SuperAttack()
    {
        bullet--;
        enemyLife -= 40;
    }

    internal float AgressiveHeuristic(WorldSpace other)
    {
        return enemyLife - other.enemyLife;
    }

    internal void Attack()
    {
        bullet--;
        enemyLife -= 25;
    }

    internal void CreateCannon()
    {
        gold -= 20;
        cannon ="cannon";
    }

    internal void Upgradedefense()
    {
        defenseIsRepaired = true;
        gold -= 40;
    }

    internal void UpgradeCannon()
    {
        gold -= 40;
        cannon = "cannon upgraded";
    }

    internal bool CanUpgradedefense()
    {
        return hasDefense && gold > 40;
    }
}
