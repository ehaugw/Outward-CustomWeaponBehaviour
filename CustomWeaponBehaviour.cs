namespace CustomWeaponBehaviour
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

    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency("com.sinai.SideLoader", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(HolyDamageManager.GUID, HolyDamageManager.VERSION)]
    [BepInDependency(TinyHelper.GUID, TinyHelper.VERSION)]
    public class CustomWeaponBehaviour : BaseUnityPlugin
    {
        public const string GUID = "com.ehaugw.customweaponbehaviour";
        public const string VERSION = "2.2.0";
        public const string NAME = "Custom Weapon Behaviour";
        //2.2.0 changes:
        // parry window: 0.5 sec to 0.35 sec
        // parry windup: 0.0 sec to 0.05 sec
        // support TinyTrinket for bastard weapons
        public static CustomWeaponBehaviour Instance;
        public static Tag BastardTag;
        public static Tag FinesseTag;
        public static Tag HolyWeaponTag;
        public static Tag HalfHandedTag;
        public static Tag SpecialLeapTag;
        public static Tag ParryTag;
        public static Tag BucklerTag;
        public static Tag WandTag;
        public static Tag TinyTrinketTag;

        public BastardBehaviour bastardBehaviour = new BastardBehaviour();
        public FinesseBehaviour finesseBehaviour = new FinesseBehaviour();
        public HalfHandedBehaviour halfHandedBehaviour = new HalfHandedBehaviour();
        public HolyBehaviour holyBehaviour = new HolyBehaviour();
        public ParryBehaviour parryBehaviour = new ParryBehaviour();
        public AttackCancelByBlockBehaviour attackCancelByBlockBehaviour = new AttackCancelByBlockBehaviour();
        public AttackCancelBySkillBehaviour attackCancelBySkillBehaviour = new AttackCancelBySkillBehaviour();

        public static List<BastardModifier> BastardModifiers = new List<BastardModifier>();

        internal void Awake()
        {
            BastardModifiers.Add(new BastardModifier());

            Instance = this;
            var harmony = new Harmony(GUID);
            harmony.PatchAll();

            SL.BeforePacksLoaded += BeforePacksLoaded;
            TinyHelper.OnDescriptionModified += CustomTagDescriptionModifier;
        }

        private void CustomTagDescriptionModifier(Item item, ref string description)
        {
            string prefix = "";

            foreach (var tup in new List<Tuple<Tag, string>> {
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
            BastardTag = TinyTagManager.GetOrMakeTag(IDs.BastardTag);
            FinesseTag = TinyTagManager.GetOrMakeTag(IDs.FinesseTag);
            HolyWeaponTag = TinyTagManager.GetOrMakeTag(IDs.HolyTag);
            ParryTag = TinyTagManager.GetOrMakeTag(IDs.ParryTag);
            BucklerTag = TinyTagManager.GetOrMakeTag(IDs.BucklerTag);
            HalfHandedTag = TinyTagManager.GetOrMakeTag("HalfHanded");
            SpecialLeapTag = TinyTagManager.GetOrMakeTag("SpecialLeap");
            WandTag = TinyTagManager.GetOrMakeTag(IDs.WandTag);
            TinyTrinketTag = TinyTagManager.GetOrMakeTag(IDs.TinyTrinketTag);
        }

        public static void ChangeGrip(Character character, Weapon.WeaponType toMoveset)
        {
            character.Inventory.SkillKnowledge.AddItem(new Item());

            character?.Animator?.SetInteger("WeaponType", (int) toMoveset);
        }

        public static void ResetGrip(Character character)
        {
            if (character?.CurrentWeapon is Weapon weapon)
                character?.Animator?.SetInteger("WeaponType", (int)weapon.Type);
        }
    }
}