﻿using System;
using System.Drawing;
using ClientInterfaces.Resource;
using GorgonLibrary;
using GorgonLibrary.Graphics;
using Image = GorgonLibrary.Graphics.Image;

namespace SS3D.LightTest
{
    public enum ShadowmapSize
    {
        Size128 = 6,
        Size256 = 7,
        Size512 = 8,
        Size1024 = 9,
    }

    public class ShadowMapResolver : IDisposable
    {
        private readonly IResourceManager _resourceManager;

        private readonly int baseSize;
        private readonly QuadRenderer quadRender;
        private readonly int reductionChainCount;
        private int depthBufferSize;
        private RenderImage distancesRT;

        private RenderImage distortRT;
        private RenderImage processedShadowsRT;
        private FXShader reductionEffect;

        private RenderImage[] reductionRT;
        private FXShader resolveShadowsEffect;
        private RenderImage shadowMap;
        private RenderImage shadowsRT;

        public ShadowMapResolver(QuadRenderer quad, ShadowmapSize maxShadowmapSize, ShadowmapSize maxDepthBufferSize,
                                 IResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
            quadRender = quad;

            reductionChainCount = (int) maxShadowmapSize;
            baseSize = 2 << reductionChainCount;
            depthBufferSize = 2 << (int) maxDepthBufferSize;
        }

        public void LoadContent()
        {
            reductionEffect = _resourceManager.GetShader("reductionEffect");
            resolveShadowsEffect = _resourceManager.GetShader("resolveShadowsEffect");

            // BUFFER TYPES ARE VERY IMPORTANT HERE AND IT WILL BREAK IF YOU CHANGE THEM!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            distortRT = new RenderImage("distortRT" + baseSize, baseSize, baseSize, ImageBufferFormats.BufferGR1616F);
            distancesRT = new RenderImage("distancesRT" + baseSize, baseSize, baseSize, ImageBufferFormats.BufferGR1616F);
            shadowMap = new RenderImage("shadowMap" + baseSize, 2, baseSize, ImageBufferFormats.BufferGR1616F);
            reductionRT = new RenderImage[reductionChainCount];
            for (int i = 0; i < reductionChainCount; i++)
            {
                reductionRT[i] = new RenderImage("reductionRT" + i + baseSize, 2 << i, baseSize,
                                                 ImageBufferFormats.BufferGR1616F);
            }
            shadowsRT = new RenderImage("shadowsRT" + baseSize, baseSize, baseSize, ImageBufferFormats.BufferRGB888A8);
            processedShadowsRT = new RenderImage("processedShadowsRT" + baseSize, baseSize, baseSize,
                                                 ImageBufferFormats.BufferRGB888A8);
        }

        public void ResolveShadows(Image shadowCastersTexture, RenderImage result, Vector2D lightPosition,
                                   bool attenuateShadows, Image mask, Vector4D maskProps, Vector4D diffuseColor)
        {
            resolveShadowsEffect.Parameters["AttenuateShadows"].SetValue(attenuateShadows ? 0 : 1);
            resolveShadowsEffect.Parameters["MaskProps"].SetValue(maskProps);
            resolveShadowsEffect.Parameters["DiffuseColor"].SetValue(diffuseColor);
            //Gorgon.CurrentRenderTarget.BlendingMode = BlendingModes.None;
            ExecuteTechnique(shadowCastersTexture, distancesRT, "ComputeDistances");
            ExecuteTechnique(distancesRT.Image, distortRT, "Distort");
            ApplyHorizontalReduction(distortRT, shadowMap);
            ExecuteTechnique(mask, result, "DrawShadows", shadowMap);
            //ExecuteTechnique(shadowsRT.Image, processedShadowsRT, "BlurHorizontally");
            //ExecuteTechnique(processedShadowsRT.Image, result, "BlurVerticallyAndAttenuate");
            Gorgon.CurrentShader = null;
        }

        private void ExecuteTechnique(Image source, RenderImage destination, string techniqueName)
        {
            ExecuteTechnique(source, destination, techniqueName, null);
        }

        private void ExecuteTechnique(Image source, RenderImage destination, string techniqueName, RenderImage shadowMap)
        {
            Vector2D renderTargetSize;
            renderTargetSize = new Vector2D(baseSize, baseSize);
            Gorgon.CurrentRenderTarget = destination;
            Gorgon.CurrentRenderTarget.Clear(Color.White);

            Gorgon.CurrentShader = resolveShadowsEffect.Techniques[techniqueName];
            resolveShadowsEffect.Parameters["renderTargetSize"].SetValue(renderTargetSize);
            if (source != null)
                resolveShadowsEffect.Parameters["InputTexture"].SetValue(source);
            if (shadowMap != null)
                resolveShadowsEffect.Parameters["ShadowMapTexture"].SetValue(shadowMap);

            quadRender.Render(new Vector2D(1, 1)*-1, new Vector2D(1, 1));

            Gorgon.CurrentRenderTarget = null;
        }

        private void ApplyHorizontalReduction(RenderImage source, RenderImage destination)
        {
            int step = reductionChainCount - 1;
            RenderImage s = source;
            RenderImage d = reductionRT[step];
            Gorgon.CurrentShader = reductionEffect.Techniques["HorizontalReduction"];

            while (step >= 0)
            {
                d = reductionRT[step];

                Gorgon.CurrentRenderTarget = d;
                d.Clear(Color.White);

                reductionEffect.Parameters["SourceTexture"].SetValue(s);
                var textureDim = new Vector2D(1.0f/s.Width, 1.0f/s.Height);
                reductionEffect.Parameters["TextureDimensions"].SetValue(textureDim);
                quadRender.Render(new Vector2D(1, 1)*-1, new Vector2D(1, 1));
                s = d;
                step--;
            }

            //copy to destination
            Gorgon.CurrentRenderTarget = destination;
            Gorgon.CurrentShader = reductionEffect.Techniques["Copy"];
            reductionEffect.Parameters["SourceTexture"].SetValue(d);
            Gorgon.CurrentRenderTarget.Clear(Color.White);
            quadRender.Render(new Vector2D(1, 1)*-1, new Vector2D(1, 1));

            reductionEffect.Parameters["SourceTexture"].SetValue(reductionRT[reductionChainCount - 1]);
            Gorgon.CurrentRenderTarget = null;
        }

        public void Dispose()
        {
            distancesRT.ForceRelease();
            distancesRT.Dispose();
            distortRT.ForceRelease();
            distortRT.Dispose();
            processedShadowsRT.ForceRelease();
            processedShadowsRT.Dispose();
            foreach(var rt in reductionRT)
            {
                rt.ForceRelease();
                rt.Dispose();
            }
            shadowMap.ForceRelease();
            shadowMap.Dispose();
            shadowsRT.ForceRelease();
            shadowsRT.Dispose();
        }
    }
}