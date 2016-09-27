/// Managed-OSVR binding
///
/// <copyright>
/// Copyright 2014, 2015 Sensics, Inc. and contributors
///
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///
///     http://www.apache.org/licenses/LICENSE-2.0
///
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// </copyright>

using System;
using OSVR.ClientKit;
using OSVR.RenderManager;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OSVR.Samples.RenderManagerGL
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            using (ServerAutoStarter serverAutoStarter = new ServerAutoStarter())
            using (ClientContext context = new ClientContext("com.osvr.exampleclients.managed.TrackerCallback"))
            using (var renderManager = new RenderManagerOpenGL(context, "OpenGL", new GraphicsLibraryOpenGL()))
            {
                while (!renderManager.DoingOkay)
                {
                    context.update();
                }

                renderManager.OpenDisplay();
                RenderInfoOpenGL[] renderInfo = new RenderInfoOpenGL[2];
                RenderParams renderParams = new RenderParams();
                renderManager.GetRenderInfo(renderParams, ref renderInfo);

                double width = 0;
                double height = 0;
                for(int i = 0; i < renderInfo.Length; i++)
                {
                    width += renderInfo[i].Viewport.Width;
                    if(height != 0 && height != renderInfo[i].Viewport.Height)
                    {
                        throw new InvalidOperationException("Expected all render targets to be the same height.");
                    }
                    height = renderInfo[i].Viewport.Height; // should all be the same height
                }

                ViewportDescription[] normalizedCroppingViewports = new ViewportDescription[renderInfo.Length];
                for(int i = 0; i < normalizedCroppingViewports.Length; i++)
                {
                    normalizedCroppingViewports[i] = new ViewportDescription
                    {
                        Height = 1.0,
                        Width = renderInfo[i].Viewport.Width / width,
                        Left = (i * renderInfo[i].Viewport.Width) / width, // assumes equal width
                        Lower = 0,
                    };
                }

                int frameBuffer = GL.GenFramebuffer();
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer);

                int colorBuffer = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, colorBuffer);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, 
                    (int)width, (int)height, 0, PixelFormat.Rgb, PixelType.UnsignedByte,
                    IntPtr.Zero);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, 
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, 
                    (int)TextureMagFilter.Linear);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, 
                    TextureTarget.Texture2D, colorBuffer, 0);

                int depthBuffer = GL.GenRenderbuffer();
                GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
                GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth24Stencil8,
                    (int)width, (int)height);

                GL.FramebufferRenderbuffer(
                    FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, 
                    RenderbufferTarget.Renderbuffer, depthBuffer);


                RenderBufferOpenGL[] buffers = new RenderBufferOpenGL[renderInfo.Length];
                for(int i = 0; i < buffers.Length; i++)
                {
                    buffers[i] = new RenderBufferOpenGL
                    {
                        ColorBufferName = (uint)colorBuffer,
                        DepthStencilBufferName = (uint)depthBuffer,
                    };
                }

                // main rendering loop
                // @todo replace with time-based loop instead of iteration based loop
                for(int i = 0; i < 100000; i++)
                {
                    context.update();
                    renderManager.PresentSolidColor(new RGBFloat { R = i % 255, G = i % 255, B = 0, });
                    renderManager.Present(buffers, renderInfo, normalizedCroppingViewports, renderParams, false);
                }

                // clean up
                GL.DeleteFramebuffer(frameBuffer);
            }
        }
    }
}
