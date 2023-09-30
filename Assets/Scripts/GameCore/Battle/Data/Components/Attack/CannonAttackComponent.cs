﻿using System;
using DG.Tweening;
using LSCore.Async;
using UnityEngine;
using Object = UnityEngine.Object;
using Health = GameCore.Battle.Data.Components.HealthComponent;

namespace GameCore.Battle.Data.Components
{
    [Serializable]
    internal class CannonAttackComponent : AttackComponent
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private ParticleSystem deathFx;
        
        protected override Tween AttackAnimation()
        {
            var bullet = Object.Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            var target = findTargetComponent.target;
            return bullet.transform.DOMove(target.position, duration).SetEase(Ease.InExpo).OnComplete(() =>
            {
                Wait.Delay(0.35f, () => Object.Destroy(bullet));
                target.Get<Health>().TakeDamage(Damage);
                Object.Instantiate(deathFx, findTargetComponent.target.position, Quaternion.identity);
            });
        }
    }
}