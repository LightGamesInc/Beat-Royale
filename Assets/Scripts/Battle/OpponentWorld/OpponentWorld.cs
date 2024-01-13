﻿using System.Collections.Generic;
using DG.Tweening;
using Battle.Data;
using LSCore;
using LSCore.Async;
using LSCore.Extensions.Unity;
using UnityEngine;

namespace Battle
{
    public partial class OpponentWorld : BasePlayerWorld<OpponentWorld>
    {
        private const int MaxEnemyCount = 100;
        [SerializeField] private UnitsById enemies;
        
        private Dictionary<Id, OnOffPool<Unit>> pools = new();
        private Camera cam;
        private Tween spawnLoopTween;
        private RaidByHeroRank raids;
        private static Rect cameraRect;
        
        protected override void OnBegin()
        {
            UserId = "Opponent";
            cam = Camera.main;
            cameraRect = cam.GetRect();
            
            foreach (var unit in enemies.ByKey.Values)
            {
                var pool = CreatePool(unit);
                pool.Got += OnGot;
                SubscribeOnChange(pool);
                pools.Add(unit.Id, pool);
            }
            
            AddDebugData();
        }

        protected override void OnStop()
        {
            RemoveDebugData();
            spawnLoopTween.Kill();
        }

        private void Spawn()
        {
            if (UnitCount < MaxEnemyCount)
            {
                pools[BattleWorld.GetEnemyId()].Get();
            }
            
            spawnLoopTween = Wait.Delay(BattleWorld.GetSpawnFrequency(), Spawn);
        }
                
        private void OnGot(Unit unit)
        {
            cameraRect.center = cam.transform.position;
            unit.transform.position = cameraRect.RandomPointAroundRect(5);
        }
        
        public static void Continue() => Instance.Spawn();
        public static void Pause() => Instance.spawnLoopTween.Kill();

        private void OnDrawGizmosSelected()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = new Color(0f, 1f, 0f, 0.49f);
            Gizmos.DrawCube(cameraRect.center, cameraRect.size);
            Gizmos.color = oldColor;
        }

        static partial void SubscribeOnChange(OnOffPool<Unit> pool);
        static partial void AddDebugData();
        static partial void RemoveDebugData();
    }
}