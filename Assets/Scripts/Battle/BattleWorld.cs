﻿using System.Collections.Generic;
using Battle;
using Battle.Windows;
using BeatHeroes.Interfaces;
using DG.Tweening;
using Battle.Data;
using LSCore.Async;
using UnityEngine;

namespace LSCore.BattleModule
{
    public class BattleWorld : ServiceManager<BattleWorld>
    {
        [SerializeField] private Effectors effectors;
        [SerializeField] private RaidByHeroRank raids;
        public static RaidConfig Raid => Instance.raids.Current;
        private static float timeSinceStart;
        private static int currentWave;
        
        private void Start()
        {
            BaseInitializer.Initialize(OnInitialize);
        }
        
        private void OnInitialize()
        {
            Setup();
            enabled = true;
        }

        private void Setup()
        {
            BattleWindow.AsHome();
            BattleWindow.Show();
            
            effectors.Init();
            raids.Setup();
            Unit.Killed += OnUnitKilled;
            PlayerWorld.Begin();
            OpponentWorld.Begin();
            timeSinceStart = 0;
            currentWave = 0;
            StartWave();
            
        }

        private static void UpdateTime()
        {
            timeSinceStart += Time.deltaTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DOTween.KillAll();
            Unit.Killed -= OnUnitKilled;
            World.Updated -= UpdateTime;
            OpponentWorld.AllUnitsDestroyed -= StartBreak;
        }

        private static void StartWave()
        {
            World.Updated += UpdateTime;
            OpponentWorld.Continue();

            currentWave++;
            timeTextPrefix = $"Wave {currentWave}";
            BattleWindow.SplashText($"WAVE {currentWave}");
            Wait.TimerBack(Raid.GetWaveDuration(), UpdateTimeText).OnComplete(PauseWave);

            Raid.CurrentWave++;
        }

        private static string timeTextPrefix;
        private static int currentTime;
        private static void UpdateTimeText(float time)
        {
            currentTime = (int)time;
            BattleWindow.StatusText.text = $"{timeTextPrefix}: {currentTime}s\nEnemy Count: {OpponentWorld.UnitCount}";
        }
        
        private static void PauseWave()
        {
            World.Updated -= UpdateTime;
            OpponentWorld.Pause();
            
            if (OpponentWorld.UnitCount == 0)
            {
                StartBreak();
            }
            else
            {
                OpponentWorld.AllUnitsDestroyed += StartBreak;
            }
        }

        private static void StartBreak()
        {
            OpponentWorld.AllUnitsDestroyed -= StartBreak;
            timeTextPrefix = "Break";
            Wait.TimerBack(Raid.BreakDuration, UpdateTimeText).OnComplete(StartWave);
        }
        
        public static Id GetEnemyId() => Raid.GetEnemyId((int)timeSinceStart);
        public static IEnumerable<Id> EnemyIds => Raid.EnemyIds;
        private static void OnUnitKilled(Unit unit)
        {
            UpdateTimeText(currentTime);
            Raid.OnEnemyKilled(unit.Id);
        }

        public static float GetSpawnFrequency() => Raid.GetSpawnFrequency((int)timeSinceStart);
    }
}