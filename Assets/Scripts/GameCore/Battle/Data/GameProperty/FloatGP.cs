﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Battle.Data.GameProperty
{
    [Serializable]
    public abstract class BaseGameProperty : GamePropertyDrawer
    {
        [CustomValueDrawer("DrawValue")] [SerializeField] protected Prop prop;

        public Prop Prop => prop;
        public string Name => GetType().Name;
        
        public abstract Prop Upgrade(Prop propValue);
        
#if UNITY_EDITOR
        public static List<Type> AllPropertyTypes { get; private set; }

        static BaseGameProperty()
        {
            var type = typeof(BaseGameProperty);
            var assembly = Assembly.GetAssembly(type);
            var types = assembly.GetTypes();
            AllPropertyTypes = new List<Type>();

            for (int i = 0; i < types.Length; i++)
            {
                var t = types[i];

                if (t.IsSubclassOf(type) && !t.IsAbstract && !t.IsInterface)
                {
                    AllPropertyTypes.Add(t);
                }
            }
        }

        private Prop DrawValue(Prop _, GUIContent __, Func<GUIContent, bool> ___)
        {
            return DrawFields();
        }

        protected abstract Prop DrawFields();
#endif

    }

    [Serializable]
    public abstract class FloatAndPercent : BaseGameProperty
    {
        public const string ValueKey = "Value";
        public const string PercentKey = "Percent";

        public static float GetValue<T>(Dictionary<string, Prop> dict) where T : BaseGameProperty => dict[typeof(T).Name].Value[ValueKey];
        
        public override Prop Upgrade(Prop propValue)
        {
            propValue.Value[ValueKey] += prop.Value[ValueKey];
            propValue.Value[ValueKey] *= 1 + prop.Value[PercentKey] / 100;
            return propValue;
        }

#if UNITY_EDITOR
        protected override Prop DrawFields()
        {
            prop.DrawFloat(ValueKey);
            prop.DrawSlider(PercentKey, 0, 100);
            return prop;
        }
#endif
    }
}
