using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System.IO;

namespace Terra
{

    class Game : GameWindow
    {
        int programId;

        int vertShaderId;
        int fragShaderId;

        int vertColourAtt;
        int vertPositionAtt;
        int uniformMView;

        int vbo_position;
        int vbo_color;
        int vbo_mview;

        Vector3[] vertdata;
        Vector3[] coldata;
        Matrix4[] mviewdata;

        void initProgram()
        {
            programId = GL.CreateProgram();

            loadShader("vs.glsl", ShaderType.VertexShader, programId, out vertShaderId);
            loadShader("fs.glsl", ShaderType.FragmentShader, programId, out fragShaderId);

            GL.LinkProgram(programId);
            Console.WriteLine(GL.GetProgramInfoLog(programId));

            vertPositionAtt = GL.GetAttribLocation(programId, "vPosition");
            vertColourAtt = GL.GetAttribLocation(programId, "vColor");
            uniformMView = GL.GetUniformLocation(programId, "modelView");

            if(vertPositionAtt == -1 || vertColourAtt == -1 || uniformMView == -1)
            {
                Console.WriteLine("Error binding attributes");
            }

            GL.GenBuffers(1, out vbo_position);
            GL.GenBuffers(1, out vbo_color);
            GL.GenBuffers(1, out vbo_mview);
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            initProgram();

            vertdata = new Vector3[] { 
                new Vector3(-0.8f, -0.8f, 0f),
                new Vector3( 0.8f, -0.8f, 0f),
                new Vector3( 0f,  0.8f, 0f)};


            coldata = new Vector3[] { 
                new Vector3(1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f,  1f, 0f)};


            mviewdata = new Matrix4[]{
                Matrix4.Identity
            };




            Title = "Hello OpenTK";
            GL.ClearColor(Color.CornflowerBlue);
            VSync = VSyncMode.On;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // render graphics
            GL.Viewport(0, 0, Width, Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);

            GL.EnableVertexAttribArray(vertPositionAtt);
            GL.EnableVertexAttribArray(vertColourAtt);

            GL.DrawArrays(BeginMode.Triangles, 0, 3);
            //GL.DrawElements(BeginMode.Triangles,3,DrawElementsType.

            GL.DisableVertexAttribArray(vertPositionAtt);
            GL.DisableVertexAttribArray(vertColourAtt);

            GL.Flush();

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4, Width / (float)Height, 1.0f, 64.0f);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadMatrix(ref projection);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            // add game logic, input handling
            if (Keyboard[Key.Escape])
            {
                Exit();
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes), vertdata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(vertPositionAtt, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_color);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(coldata.Length * Vector3.SizeInBytes), coldata,BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(vertColourAtt, 3, VertexAttribPointerType.Float, true, 0, 0);

            GL.UniformMatrix4(uniformMView, false, ref mviewdata[0]);

            GL.UseProgram(programId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        void loadShader(String filename,ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

    }



    class Program
    {
        static void Main(string[] args)
        {
            using (Game game = new Game())
            {
                // Run the game at 60 updates per second
                game.Run(30.0);
            }
        }
    }
}
