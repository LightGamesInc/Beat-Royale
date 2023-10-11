﻿using System;
using System.Collections.Generic;
using GameCore.Battle.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public class EntityGameProps
    {
        [field: ValueDropdown("Destinations")] 
        [field: SerializeField] public int Destination { get; private set; } 
        
        [field: HideReferenceObjectPicker]
        [field: ValueDropdown("AvailableProps", IsUniqueList = true)]
        [field: OdinSerialize] public HashSet<BaseGameProperty> Props { get; } = new();
        
#if UNITY_EDITOR
        private ValueDropdownList<BaseGameProperty> list;
        protected virtual IList<ValueDropdownItem<BaseGameProperty>> AvailableProps
        {
            get
            {
                if (list == null)
                { 
                    list = new ValueDropdownList<BaseGameProperty>();
                    var allTypes = BaseGameProperty.AllPropertyTypes;
                    for (int i = 0; i < allTypes.Count; i++)
                    {
                        var type = allTypes[i];
                        list.Add(type.Name, (BaseGameProperty)Activator.CreateInstance(type));
                    }
                }
                
                return list;
            }
        }
        protected virtual IList<ValueDropdownItem<int>> Destinations => IdToName.GetValues(EntityMeta.EntityIds, LevelConfig.GetExcept(ref except, ref config, Filter));
        
        private LevelConfig config;
        private HashSet<int> except;

        private void Filter(LevelConfig level)
        {
            foreach (var upgrade in level.OtherUpgrades)
            {
                except.Add(upgrade.Destination);
            }
        }

#endif
    }

    [Serializable]
    public class AllDestinationsGameProps : EntityGameProps
    {
#if UNITY_EDITOR
        protected override IList<ValueDropdownItem<int>> Destinations => IdToName.GetValues(EntityMeta.AllDestinations, LevelConfig.GetExcept(ref except, ref config, Filter));
        
        private LevelConfig config;
        private HashSet<int> except;

        private void Filter(LevelConfig level) => except.Add(level.EntityId);
#endif
    }
}