﻿using Graphics.Inspector;
using MessagePack;
using UnityEngine;
using UnityEngine.Rendering;

namespace Graphics.Settings
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class GlobalSettings
    {
        private int _pixelLightCount;

        public int PixelLightCount
        {
            get => QualitySettings.pixelLightCount;
            set => QualitySettings.pixelLightCount = _pixelLightCount = value;
        }

        public AnisotropicFiltering AnisotropicFiltering
        {
            get => QualitySettings.anisotropicFiltering;
            set => QualitySettings.anisotropicFiltering = value;
        }

        public int AntiAliasing
        {
            get => QualitySettings.antiAliasing;
            set => QualitySettings.antiAliasing = value;
        }

        public bool RealtimeReflectionProbes
        {
            get => QualitySettings.realtimeReflectionProbes;
            set => QualitySettings.realtimeReflectionProbes = value;
        }

        public bool PulseReflectionProbes
        {
            get => Graphics.Instance.PulseReflectionProbes; 
            set => Graphics.Instance.PulseReflectionProbes = value;               
        }
       
        public float PulseReflectionTimer
        {
            get => Graphics.Instance.ReflectionProbesPulseTimer;
            set => Graphics.Instance.ReflectionProbesPulseTimer = value;
        }

        public ShadowmaskMode ShadowmaskModeSetting
        {
            get => QualitySettings.shadowmaskMode;
            set => QualitySettings.shadowmaskMode = value;
        }

        public ShadowQuality ShadowQualitySetting
        {
            get => QualitySettings.shadows;
            set => QualitySettings.shadows = value;
        }

        public ShadowResolution ShadowResolutionSetting
        {
            get => QualitySettings.shadowResolution;
            set => QualitySettings.shadowResolution = value;
        }

        public ShadowProjection ShadowProjectionSetting
        {
            get => QualitySettings.shadowProjection;
            set => QualitySettings.shadowProjection = value;
        }

        public float ShadowDistance
        {
            get => QualitySettings.shadowDistance;
            set => QualitySettings.shadowDistance = value;
        }

        public float ShadowNearPlaneOffset
        {
            get => QualitySettings.shadowNearPlaneOffset;
            set => QualitySettings.shadowNearPlaneOffset = value;
        }

        public bool LightsUseLinearIntensity
        {
            get => GraphicsSettings.lightsUseLinearIntensity;
            set => GraphicsSettings.lightsUseLinearIntensity = value;
        }

        public bool LightsUseColorTemperature
        {
            get => GraphicsSettings.lightsUseColorTemperature;
            set
            {
                if (value)
                {
                    LightsUseLinearIntensity = value;
                }

                GraphicsSettings.lightsUseColorTemperature = value;
            }
        }

        public bool UsePCSS
        {
            get => BuiltinShaderMode.UseCustom == GraphicsSettings.GetShaderMode(BuiltinShaderType.ScreenSpaceShadows)
                && PCSSLight.pcssName == GraphicsSettings.GetCustomShader(BuiltinShaderType.ScreenSpaceShadows).name;
            set
            {
                if (value)
                {
                    PCSSLight.EnablePCSS();
                }
                else
                {
                    PCSSLight.DisablePCSS();
                }
                Graphics.Instance.LightManager.Light();
            }
        }

        internal int FontSize
        {
            get => GUIStyles.FontSize;
            set => GUIStyles.FontSize = Graphics.ConfigFontSize.Value = value;
        }

        internal bool ShowAdvancedSettings { get; set; }
    }
}