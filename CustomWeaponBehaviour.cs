﻿namespace CustomWeaponBehaviour
{
    using BepInEx;
    using HarmonyLib;
    using SideLoader;
    using TinyHelper;
    using HolyDamageManager;
    using UnityEngine;
    using System.Collections.Generic;
    using InstanceIDs;
    using System;
    using System.Linq;
    using BaseDamageModifiers;

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("com.sinai.SideLoader", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(BaseDamageModifiers.GUID, BaseDamageModifiers.VERSION)]
    [BepInDependency(HolyDamageManager.GUID, HolyDamageManager.VERSION)]
    [BepInDependency(TinyHelper.GUID, TinyHelper.VERSION)]
    public class CustomWeaponBehaviour : BaseUnityPlugin
    {
        public const string GUID = "com.ehaugw.customweaponbehaviour";
        public const string VERSION = "4.0.3";
        public const string NAME = "Custom Weapon Behaviour";
        public static CustomWeaponBehaviour Instance;
        public static Tag BastardTag;
        public static Tag FinesseTag;
        public static Tag HolyWeaponTag;
        public static Tag HalfHandedTag;
        public static Tag SpecialLeapTag;
        public static Tag ParryTag;
        public static Tag BucklerTag;
        public static Tag WandTag;
        public static Tag HandsFreeTag;
        public static Tag LanternTag;
        public static Tag MaulShoveTag;
        public static Tag PointyTag;

        public BastardBehaviour bastardBehaviour = new BastardBehaviour();
        public MaulShoveBehaviour maulShoveBehaviour = new MaulShoveBehaviour();
        public FinesseBehaviour finesseBehaviour = new FinesseBehaviour();
        public HalfHandedBehaviour halfHandedBehaviour = new HalfHandedBehaviour();

        public HolyBehaviour holyBehaviour = new HolyBehaviour();
        public ParryBehaviour parryBehaviour = new ParryBehaviour();
        public AttackCancelByBlockBehaviour attackCancelByBlockBehaviour = new AttackCancelByBlockBehaviour();
        public AttackCancelBySkillBehaviour attackCancelBySkillBehaviour = new AttackCancelBySkillBehaviour();

        public static List<IBastardModifier> IBastardModifiers = new List<IBastardModifier>();
        public static List<IMaulShoveModifier> IMaulShoveModifiers = new List<IMaulShoveModifier>();

        public delegate void BastardDisabler(Weapon weapon, ref bool eligible);
        public BastardDisabler BastardDisablers = delegate (Weapon weapon, ref bool eligible) { };

        internal void Awake()
        {
            IBastardModifiers.Add(new BastardModifier());
            IMaulShoveModifiers.Add(new MaulShoveModifier());

            Instance = this;
            var harmony = new Harmony(GUID);
            harmony.PatchAll();

            SL.BeforePacksLoaded += BeforePacksLoaded;
            SL.OnPacksLoaded += OnPacksLoaded;
            TinyHelper.OnDescriptionModified += CustomTagDescriptionModifier;

            BaseDamageModifiers.WeaponDamageModifiers += delegate (Weapon weapon, DamageList original, ref DamageList result) { CustomBehaviourFormulas.PostAffectDamage(ref weapon, original, ref result); };
            BaseDamageModifiers.WeaponImpactModifiers += delegate (Weapon weapon, float original, ref float result) { CustomBehaviourFormulas.PostAffectImpact(ref weapon, original, ref result); };
        }

        private void CustomTagDescriptionModifier(Item item, ref string description)
        {
            string prefix = "";

            foreach (var tup in new List<Tuple<Tag, string>> {
                new Tuple<Tag, string>(CustomWeaponBehaviour.PointyTag, "Pointy"),
                new Tuple<Tag, string>(CustomWeaponBehaviour.BastardTag, "Bastard"),
                new Tuple<Tag, string>(CustomWeaponBehaviour.FinesseTag, "Finesse"),
                new Tuple<Tag, string>(CustomWeaponBehaviour.HolyWeaponTag, "Holy"),
                new Tuple<Tag, string>(CustomWeaponBehaviour.WandTag, "Wand"),
            })
            {
                if (item.HasTag(tup.Item1))
                {
                    prefix += (prefix.Length > 0 ? ", " : "") + tup.Item2;
                }
            }

            if (prefix.Length > 0)
            {
                description = prefix + "\n\n" + description;
            }
        }

        private void BeforePacksLoaded()
        {
            PointyTag = TinyTagManager.GetOrMakeTag(IDs.PointyTag);
            BastardTag = TinyTagManager.GetOrMakeTag(IDs.BastardTag);
            FinesseTag = TinyTagManager.GetOrMakeTag(IDs.FinesseTag);
            HolyWeaponTag = TinyTagManager.GetOrMakeTag(IDs.HolyTag);
            ParryTag = TinyTagManager.GetOrMakeTag(IDs.ParryTag);
            BucklerTag = TinyTagManager.GetOrMakeTag(IDs.BucklerTag);
            HalfHandedTag = TinyTagManager.GetOrMakeTag("HalfHanded");
            SpecialLeapTag = TinyTagManager.GetOrMakeTag("SpecialLeap");
            WandTag = TinyTagManager.GetOrMakeTag(IDs.WandTag);
            HandsFreeTag = TinyTagManager.GetOrMakeTag(IDs.HandsFreeTag);
            LanternTag = TinyTagManager.GetOrMakeTag(IDs.LanternTag);
            MaulShoveTag = TinyTagManager.GetOrMakeTag(IDs.MaulShoveTag);
        }

        private void OnPacksLoaded()
        {
        }
    }
}
