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
using System.Drawing;
using OSVR.ClientKit;
using OSVR.RenderManager;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.ComponentModel;

namespace OSVR.Samples.RenderManagerGL
{
    class OpenTkGraphicsToolkit : OSVR.RenderManager.OpenGLToolkitFunctions
    {
        SimpleWindow glWindow;
        public OpenTkGraphicsToolkit(SimpleWindow glWindow)
        {
            if(glWindow == null)
            {
                throw new ArgumentNullException("glWindow");
            }
            this.glWindow = glWindow;
        }

        protected override bool AddOpenGLContext(ref OpenGLContextParams p)
        {
            glWindow.Width = p.Width;
            glWindow.Height = p.Height;
            glWindow.WindowBorder = WindowBorder.Hidden;
            glWindow.Bounds = new Rectangle(p.XPos, p.YPos, p.Width, p.Height);
            return true;
        }

        protected override bool MakeCurrent(UIntPtr display)
        {
            glWindow.MakeCurrent();
            return true;
        }

        protected override bool SetVerticalSync(bool verticalSync)
        {
            glWindow.VSync = verticalSync ? VSyncMode.On : VSyncMode.Off;
            return true;
        }

        protected override bool SwapBuffers(UIntPtr display)
        {
            glWindow.SwapBuffers();
            return true;
        }

        //protected override bool GetDisplaySizeOverride(UIntPtr display, out int width, out int height)
        //{
        //    width = glWindow.Width;
        //    height = glWindow.Height;
        //    return true;
        //}

        protected override bool GetDisplayFrameBuffer(UIntPtr display, out uint frameBufferOut)
        {
            frameBufferOut = 0;
            return true;
        }

        protected override bool HandleEvents()
        {
            glWindow.ProcessEvents();
            return true;
        }

        protected override bool RemoveOpenGLContext()
        {
            glWindow.Exit();
            return true;
        }
    }

    class SimpleWindow : GameWindow
    {
        RenderManagerOpenGL renderManager;
        ClientContext context;
        GraphicsLibraryOpenGL graphicsLibrary;
        ViewportDescription[] normalizedCroppingViewports;
        RenderBufferOpenGL[] buffers;
        UInt64 frameNumber = 0;
        int windowFrameBuffer = -1;
        int frameBuffer = -1;

        public SimpleWindow(int width, int height) : base(width, height)
        {
            this.KeyDown += SimpleWindow_KeyDown;
        }

        private void SimpleWindow_KeyDown(object sender, OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == OpenTK.Input.Key.Escape)
            {
                this.Exit();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color.MidnightBlue);
            context = new ClientContext("com.osvr.exampleclients.managed.TrackerCallback");
            graphicsLibrary = new GraphicsLibraryOpenGL();
            graphicsLibrary.Toolkit = new OpenTkGraphicsToolkit(this);
            renderManager = new RenderManagerOpenGL(context, "OpenGL", graphicsLibrary);

            while (!renderManager.DoingOkay)
            {
                context.update();
            }

            var openResults = renderManager.OpenDisplay();
            if(openResults.Status == OpenStatus.Failure)
            {
                throw new InvalidOperationException("Could not open display");
            }
            RenderInfoOpenGL[] renderInfo = new RenderInfoOpenGL[2];
            RenderParams renderParams = RenderParams.Default;
            //int tryCount = 0;
            bool gotRenderInfo = false;
            while (!gotRenderInfo)
            {
                context.update();
                if (renderManager.GetRenderInfo(renderParams, ref renderInfo))
                {
                    gotRenderInfo = true;
                    break;
                }
            }

            if(!gotRenderInfo)
            {
                throw new Exception("Couldn't get render info after 10000 tries");
            }

            double width = 0;
            double height = 0;
            for (int i = 0; i < renderInfo.Length; i++)
            {
                width += renderInfo[i].Viewport.Width;
                if (height != 0 && height != renderInfo[i].Viewport.Height)
                {
                    throw new InvalidOperationException("Expected all render targets to be the same height.");
                }
                height = renderInfo[i].Viewport.Height; // should all be the same height
            }

            normalizedCroppingViewports = new ViewportDescription[renderInfo.Length];
            for (int i = 0; i < normalizedCroppingViewports.Length; i++)
            {
                normalizedCroppingViewports[i] = new ViewportDescription
                {
                    Height = 1.0,
                    Width = renderInfo[i].Viewport.Width / width,
                    Left = (i * renderInfo[i].Viewport.Width) / width, // assumes equal width
                    Lower = 0,
                };
            }

            frameBuffer = GL.GenFramebuffer();
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

            buffers = new RenderBufferOpenGL[renderInfo.Length];
            for (int i = 0; i < buffers.Length; i++)
            {
                buffers[i] = new RenderBufferOpenGL
                {
                    ColorBufferName = (uint)colorBuffer,
                    DepthStencilBufferName = (uint)depthBuffer,
                };
            }

            base.OnLoad(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //GL.Begin(BeginMode.Triangles);

            //GL.Color3(Color.MidnightBlue);
            //GL.Vertex2(-1.0f, 1.0f);
            //GL.Color3(Color.SpringGreen);
            //GL.Vertex2(0.0f, -1.0f);
            //GL.Color3(Color.Ivory);
            //GL.Vertex2(1.0f, 1.0f);

            //GL.End();

            //this.SwapBuffers();

            context.update();

            RenderInfoOpenGL[] renderInfo = new RenderInfoOpenGL[2];
            RenderParams renderParams = new RenderParams();
            renderManager.GetRenderInfo(renderParams, ref renderInfo);

            //renderManager.PresentSolidColor(new RGBFloat { R = frameNumber % 255, G = frameNumber % 255, B = 0, });
            renderManager.Present(buffers, renderInfo, normalizedCroppingViewports, renderParams, false);

            frameNumber++;
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            ClientContext.PreloadNativeLibraries();
            using (ServerAutoStarter serverAutoStarter = new ServerAutoStarter())
            using (SimpleWindow window = new SimpleWindow(600, 600))
            {
                window.Run();
                // clean up
                //GL.DeleteFramebuffer(frameBuffer);
            }
        }
    }
}
