﻿
using System;
using UnityEngine.Experimental.Rendering.LightweightPipeline;
using UnityEngine.Rendering;

namespace UnityEngine.XR.ARFoundation.LWRPSupport
{
    [CreateAssetMenu(fileName = "LWRPBackroundRendererAsset", menuName = "XR/LWRPBackroundRendererAsset")]
    public class LWRPBackroundRendererAsset : ARBackgroundRendererAsset
    {
        /// <summary>
        /// we're going to reference all materials that we want to use so that they get built into the project
        /// </summary>
        public Material[] MaterialsUsed;
        
        public override ARFoundationBackgroundRenderer CreateARBackgroundRenderer()
        {
            var bUseRenderPipeline = GraphicsSettings.renderPipelineAsset != null;
            return bUseRenderPipeline ? new LWRPBackgroundRenderer() : new ARFoundationBackgroundRenderer();
        }

        public override void CreateHelperComponents(GameObject cameraGameObject)
        {
            var bUseRenderPipeline = GraphicsSettings.renderPipelineAsset != null;

            if (bUseRenderPipeline)
            {
                var lwrpBeforeCameraRender = cameraGameObject.GetComponent<LWRPBeforeCameraRender>();
                if (lwrpBeforeCameraRender == null)
                {
                    cameraGameObject.AddComponent<LWRPBeforeCameraRender>();
                }

                var lightweightAdditionalCameraData = cameraGameObject.GetComponent<LightweightAdditionalCameraData>();
                if (lightweightAdditionalCameraData == null)
                {
                    lightweightAdditionalCameraData = cameraGameObject.AddComponent<LightweightAdditionalCameraData>();
                }

                lightweightAdditionalCameraData.renderShadows = false;
                lightweightAdditionalCameraData.requiresColorTexture = false;
                lightweightAdditionalCameraData.requiresDepthTexture = false;

            }
        }

        public override Material CreateCustomMaterial()
        {
            var cameraSubsystem = ARSubsystemManager.cameraSubsystem;
            if (cameraSubsystem == null)
                return null;

            // Try to create a material from the plugin's provided shader.
            var shaderName = "";
            if (!cameraSubsystem.TryGetShaderName(ref shaderName))
                return null;

            shaderName = shaderName + "LWRP";
            
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                 throw new InvalidOperationException(string.Format(
                    "Could not find shader named \"{0}\" required for LWRP video overlay on camera subsystem named \"{1}\".",
                    shaderName,
                    cameraSubsystem.SubsystemDescriptor.id));
            }

            return new Material(shader);
        }
    }
    
}    
