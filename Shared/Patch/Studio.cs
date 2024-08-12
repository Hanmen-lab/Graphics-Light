﻿using Graphics.Settings;
using HarmonyLib;
using LuxWater;
using Studio;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Graphics.Patch
{
    internal static class StudioHooks
    {
        public static void Init()
        {
            //HarmonyWrapper.PatchAll(typeof(StudioHooks));
            HarmonyLib.Harmony.CreateAndPatchAll(typeof(StudioHooks));
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MPLightCtrl), "Awake")]
        static void MPLightCtrl_Awake_Postfix(MPLightCtrl __instance)
        {
            var instance = Traverse.Create(__instance);
            var viIntensity = instance.Field("viIntensity").GetValue();
            var ValueInfo = Traverse.Create(viIntensity);
            var slider = ValueInfo.Field("slider").GetValue<Slider>();
            slider.minValue = LightSettings.IntensityMin;
            slider.maxValue = LightSettings.IntensityMax;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(MPLightCtrl), "OnValueChangeIntensity")]
        static bool MPLightCtrl_OnValueChangeIntensity_Prefix(MPLightCtrl __instance, float _value)
        {
            var instance = Traverse.Create(__instance);
            var m_OCILight = instance.Field("m_OCILight").GetValue<OCILight>();
            var viIntensity = instance.Field("viIntensity").GetValue();
            var ValueInfo = Traverse.Create(viIntensity);
            var inputField = ValueInfo.Field("inputField").GetValue<InputField>();
            if (instance.Field("isUpdateInfo").GetValue<bool>())
            {
                return false;
            }
            if (m_OCILight.SetIntensity(_value, false))
            {
                inputField.text = m_OCILight.lightInfo.intensity.ToString("0.000");
            }
            return false;
        }
        [HarmonyPrefix, HarmonyPatch(typeof(MPLightCtrl), "OnEndEditIntensity")]
        static bool MPLightCtrl_OnEndEditIntensity_Prefix(MPLightCtrl __instance, string _text)
        {
            var instance = Traverse.Create(__instance);
            var m_OCILight = instance.Field("m_OCILight").GetValue<OCILight>();
            var viIntensity = instance.Field("viIntensity").GetValue();
            var ValueInfo = Traverse.Create(viIntensity);
            var inputField = ValueInfo.Field("inputField").GetValue<InputField>();
            var slider = ValueInfo.Field("slider").GetValue<Slider>();
            float value = Mathf.Clamp(StringToFloat(_text), LightSettings.IntensityMin, LightSettings.IntensityMax);
            m_OCILight.SetIntensity(value, false);
            inputField.text = m_OCILight.lightInfo.intensity.ToString("0.00");
            slider.value = m_OCILight.lightInfo.intensity;
            return false;
        }

        private static float StringToFloat(string _text)
        {
            return (!float.TryParse(_text, out float num)) ? 0f : num;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(Studio.Studio), nameof(Studio.Studio.AddLight))]
        static bool Studio_AddLight_Prefix(Studio.Studio __instance, int _no)
        {
            var instance = Traverse.Create(__instance);
            OCILight ocilight = Studio.AddObjectLight.Add(_no);
            Singleton<UndoRedoManager>.Instance.Clear();
            if (Studio.Studio.optionSystem.autoHide)
            {
                RootButtonCtrl ctrl = instance.Property<RootButtonCtrl>("rootButtonCtrl").Value;
                ctrl.OnClick(-1);
            }
            if (Studio.Studio.optionSystem.autoSelect && ocilight != null)
            {
                TreeNodeCtrl ctrl = instance.Field("m_TreeNodeCtrl").GetValue<TreeNodeCtrl>();
                ctrl.SelectSingle(ocilight.treeNodeObject, true);
            }
            Graphics.Instance?.LightManager?.Light();
            return false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(AddObjectLight), nameof(AddObjectLight.Load), new Type[] {
            typeof(OILightInfo), typeof(ObjectCtrlInfo), typeof(TreeNodeObject), typeof(bool), typeof(int)
        })]
        public static void PCSSInitialization(ref OCILight __result, OILightInfo _info, ObjectCtrlInfo _parent, TreeNodeObject _parentNode, bool _addInfo, int _initialPosition)
        {
            Graphics.Instance?.LightManager?.Light();
        }
        [HarmonyPostfix, HarmonyPatch(typeof(Studio.Map), "OnLoadAfter")]

        //Check and disable map Post Process Volume.
        public static void Map_OnLoadAfter()
        {
            GameObject postVolumeMap = GameObject.Find("Map/PostVolume_map");
            if (postVolumeMap != null)
            {            
                postVolumeMap.SetActive(false);
                Graphics.Instance.Log.LogMessage("[Graphics] Map Post Process Volume detected. Disabled.");
            }
            
        }

        [HarmonyPostfix, HarmonyPatch(typeof(LuxWater_PlanarReflection), "OnEnable")]
        static void LuxWater_PlanarReflection_OnEnable_Postfix(LuxWater_PlanarReflection __instance)
        {
            __instance.StartCoroutine(LuxWater_PlanarReflection_OnEnable_Hook(__instance));
        }
        static System.Collections.IEnumerator LuxWater_PlanarReflection_OnEnable_Hook(LuxWater_PlanarReflection __instance)
        {
            yield return null;
            __instance.m_SharedMaterial = __instance.GetComponent<Renderer>().sharedMaterial;
        }
    }
}
