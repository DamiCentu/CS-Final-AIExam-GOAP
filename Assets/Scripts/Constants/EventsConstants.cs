using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventsConstants {
    //ONLY UPDATE MANAGER EVENTS
    public const string SUBSCRIBE_UPDATE = "subscribeUpdate";
    public const string DESUBSCRIBE_UPDATE = "desubscribeUpdate";
    public const string SUBSCRIBE_FIXED_UPDATE = "subscribeUpdate";
    public const string DESUBSCRIBE_FIXED_UPDATE = "desubscribeUpdate";

    public const string CLEAR_ALL_UPDATES = "ClearAllUpdates";
    //---------------------------------------------------------------

    public const string UI_UPDATE_GOLD = "UIupdateGold";
    public const string UI_UPDATE_BULLETS = "UIupdateBullets";
    //---------------------------------------------------------------
    public const string UI_UPDATE_GOLD_IA = "UIupdateGoldIA";
    public const string UI_UPDATE_BULLETS_IA = "UIupdateBulletsIA";

    public const string IA_MINNING = "IA_MINNING";
    public const string IA_SHOOTING = "IA_SHOOTING";
    public const string IA_CREATE_BULLET = "IA_CREATE_BULLET";
    public const string IA_CREATE_CANNON = "IA_CREATE_CANNON";
    public const string IA_CREATE_DEFENSE = "IA_CREATE_DEFENSE";
    public const string IA_UPGRADE_BULLET = "IA_UPGRADE_BULLET";
    public const string IA_UPGRADE_CANNON = "IA_UPGRADE_CANNON";
    public const string IA_UPGRADE_DEFENSE = "IA_UPGRADE_DEFENSE";



    //---------------------------------------------------------------
    public const string SET_MAP_NODE_UNACCESABLE = "mapNodesUpdate";

    public const string PLAYER_REQUEST_CREATE = "playerRequestCreate";
    public const string PLAYER_REQUEST_UPGRADE = "playerRequestUpgrade";
    public const string PLAYER_REQUEST_MINING = "playerRequestMining";
    public const string PLAYER_REQUEST_SHOOT = "playerRequestShoot";

    public const string PLAYER_CREATE = "playerCreate";
    public const string PLAYER_UPGRADE = "playerUpgrade";
    public const string PLAYER_MINING = "playerMining";
    public const string PLAYER_ATTACK = "playerAttack";

    public const string BLOCK_PLAYER_IF_FALSE = "blockOrUnblockPlayer";

    public const string PLAYER_NOT_ENOUGH_GOLD_NOTIFICATION = "playerNotEnoughGoldNotification";
    public const string PLAYER_NOT_ENOUGH_BULLETS_NOTIFICATION = "playerNotEnoughBulletsNotification";
    public const string PLAYER_OCCUPIED_NOTIFICATION = "playerOcupedNotification";

    public const string ITEM_CREATED = "itemCreated";
    public const string ITEM_UPGRADED = "itemUpgraded";

}
