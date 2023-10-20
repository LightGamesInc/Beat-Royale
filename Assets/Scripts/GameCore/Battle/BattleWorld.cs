﻿using Battle.Windows;
using BeatHeroes.Interfaces;
using GameCore.Battle;
using GameCore.Battle.Data;
using LSCore;
using LSCore.AddressablesModule.AssetReferences;
using LSCore.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Battle
{
    public class BattleWorld : ServiceManager
    {
        [FormerlySerializedAs("units")] [SerializeField] private Heroes heroes;
        [SerializeField] private Effectors effectors;
        [SerializeField] private Camera camera;
        [SerializeField] private Locations locations;
        [SerializeField] private Vector3 cameraOffset;

        private Unit hero;
        private GameObject location;

        protected override void Awake()
        {
            base.Awake();
            enabled = false;
            BaseInitializer.Initialize(OnInitialize);
        }

        private void InstatiateLocation()
        {
            var locationIndex = IListExtensions.ClosestBinarySearch(
                index => locations[index].maxLevel,
                locations.Length,
                PlayerData.Config.Level);
            var locationData = locations[locationIndex];
            location = Instantiate(locationData.locationRef.Load().prefab);
        }

        private void InstatiateHero()
        {
            hero = Instantiate(Heroes.ByName[PlayerData.Config.SelectedHero]);
            //hero.transform.position = location.HeroSpawnPoint.position;
            hero.Destroyed += OnHeroDied;
            hero.Init("Player");
        }

        private void OnHeroDied()
        {
            enabled = false;
        }
        
        private void OnInitialize()
        {
            Init();
            enabled = true;
        }

        private void Init()
        {
            heroes.Init();
            effectors.Init();
            InstatiateLocation();
            InstatiateHero();
            CameraMover.Init(camera, hero.transform, cameraOffset);
            MatchResultWindow.Showing += Unsubscribe;
            BattleWindow.Show();
        }

        private void Update()
        {
            hero.Run();
        }

        private void FixedUpdate()
        {
            hero.FixedRun();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            hero.Destroyed -= OnHeroDied;
        }

        private void Unsubscribe()
        {
            MatchResultWindow.Showing -= Unsubscribe;
        }

        private void OnApplicationQuit()
        {
            Unsubscribe();
        }
    }
}