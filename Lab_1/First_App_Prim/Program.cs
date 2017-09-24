using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.FreeGlut;
using OpenGL;
using System.Drawing;


namespace First_App_Prim
{
   class Program
    {
        private static int width = 1280, height = 720;
        private static ShaderProgram program, program_b;
        private static VBO<Vector3> cube, cube_2, background;
        private static VBO<Vector3> cubeColor, cubeColor_2;
        private static VBO<Vector2>    background_Color;           
        private static VBO<int>  cubeQuads, cubeQuads_2 ,backgroundElements;
        private static Texture crateTextu;
        private static float angle;

        static void Main(string[] args)
        {
            // Инициализируем главное окно
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);
            Glut.glutInitWindowSize(width, height);
            Glut.glutCreateWindow("OpenGL Tutorial");

            // Функции, необходимы для отрисовки всей этой дичи
            Glut.glutIdleFunc(OnRenderFrame);
            Glut.glutDisplayFunc(OnDisplay);
            Glut.glutCloseFunc(OnClose);

            //Для реализации прозрачности
            //Gl.Enable(EnableCap.DepthTest);
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


            // Компилим шейдеры
            program = new ShaderProgram(VertexShader, FragmentShader);
            program_b = new ShaderProgram(VertexShader_Back, FragmentShader_Back);

            program_b.Use();
            program_b["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program_b["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));


            crateTextu = new Texture("Background.jpg");
            // Созддаем фон
            background = new VBO<Vector3>(new Vector3[] { new Vector3(-10, 10, 0), new Vector3(10, 10, 0), new Vector3(10, -10, 0), new Vector3(-10, -10, 0) });
            //background_Color = new VBO<Vector3>(new Vector3[] { new Vector3(0.5f, 0.5f, 1), new Vector3(0.5f, 0.5f, 1), new Vector3(0.5f, 0.5f, 1), new Vector3(0.5f, 0.5f, 1) });
            background_Color = new VBO<Vector2>(new Vector2[] {
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) });
            backgroundElements = new VBO<int>(new int[] { 0, 1, 2, 3 }, BufferTarget.ElementArrayBuffer);
            program.Use();
            program["projection_matrix"].SetValue(Matrix4.CreatePerspectiveFieldOfView(0.45f, (float)width / height, 0.1f, 1000f));
            program["view_matrix"].SetValue(Matrix4.LookAt(new Vector3(0, 0, 10), Vector3.Zero, new Vector3(0, 1, 0)));

            //crateTextu = new Texture(Properties.Resources.Background);
            // Создаем первый кубик (правый)
            cube = new VBO<Vector3>(new Vector3[] {
                new Vector3(0.85f, 0.85f, -0.85f), new Vector3(-0.85f,0.85f,-0.85f), new Vector3(-0.85f,0.85f,0.85f), new Vector3(0.85f,0.85f,0.85f),
                new Vector3(0.85f, -0.85f, 0.85f), new Vector3(-0.85f,-0.85f,0.85f), new Vector3(-0.85f,-0.85f,-0.85f), new Vector3(0.85f,-0.85f,-0.85f),
                new Vector3(0.85f,0.85f,0.85f), new Vector3(-0.85f,0.85f,0.85f), new Vector3(-0.85f,-0.85f,0.85f), new Vector3(0.85f,-0.85f,0.85f),
                new Vector3(0.85f,-0.85f,-0.85f), new Vector3(-0.85f,-0.85f,-0.85f), new Vector3(-0.85f,0.85f,-0.85f), new Vector3(0.85f,0.85f,-0.85f),
                new Vector3(-0.85f,0.85f,0.85f), new Vector3(-0.85f,0.85f,-0.85f), new Vector3(-0.85f,-0.85f,-0.85f), new Vector3(-0.85f,-0.85f,0.85f),
                new Vector3(0.85f,0.85f, -0.85f), new Vector3(0.85f,0.85f,0.85f), new Vector3(0.85f,-0.85f,0.85f), new Vector3(0.85f,-0.85f,-0.85f) });
            cubeColor = new VBO<Vector3>(new Vector3[]  {
               new Vector3(0, 0, 0.45f), new Vector3(0, 0, 0.45f), new Vector3(0, 0, 0.45f), new Vector3(0, 0, 0.45f), 
               new Vector3(0, 0, 0.55f), new Vector3(0, 0, 0.55f), new Vector3(0, 0, 0.55f), new Vector3(0, 0, 0.55f),
               new Vector3(0, 0, 0.85f), new Vector3(0, 0, 0.85f), new Vector3(0, 0, 0.85f), new Vector3(0, 0, 0.85f), 
               new Vector3(0, 0, 0.65f), new Vector3(0, 0, 0.65f), new Vector3(0, 0, 0.65f), new Vector3(0, 0, 0.65f), 
               new Vector3(0, 0, 0.35f), new Vector3(0, 0, 0.35f), new Vector3(0, 0, 1), new Vector3(0, 0, 1), 
               new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 0.35f), new Vector3(0, 0, 1) });
            cubeQuads = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);


            // Создаем второй кубик
            cube_2 = new VBO<Vector3>(new Vector3[] {
                new Vector3(0.85f, 0.85f, -0.85f), new Vector3(-0.85f,0.85f,-0.85f), new Vector3(-0.85f,0.85f,0.85f), new Vector3(0.85f,0.85f,0.85f),
                new Vector3(0.85f, -0.85f, 0.85f), new Vector3(-0.85f,-0.85f,0.85f), new Vector3(-0.85f,-0.85f,-0.85f), new Vector3(0.85f,-0.85f,-0.85f),
                new Vector3(0.85f,0.85f,0.85f), new Vector3(-0.85f,0.85f,0.85f), new Vector3(-0.85f,-0.85f,0.85f), new Vector3(0.85f,-0.85f,0.85f),
                new Vector3(0.85f,-0.85f,-0.85f), new Vector3(-0.85f,-0.85f,-0.85f), new Vector3(-0.85f,0.85f,-0.85f), new Vector3(0.85f,0.85f,-0.85f),
                new Vector3(-0.85f,0.85f,0.85f), new Vector3(-0.85f,0.85f,-0.85f), new Vector3(-0.85f,-0.85f,-0.85f), new Vector3(-0.85f,-0.85f,0.85f),
                new Vector3(0.85f,0.85f, -0.85f), new Vector3(0.85f,0.85f,0.85f), new Vector3(0.85f,-0.85f,0.85f), new Vector3(0.85f,-0.85f,-0.85f) });
            cubeColor_2 = new VBO<Vector3>(new Vector3[] {
               new Vector3(0, 0, 0.45f), new Vector3(0, 0, 0.45f), new Vector3(0, 0, 0.45f), new Vector3(0, 0, 0.45f), // Задняя сторона
               new Vector3(0, 0, 0.55f), new Vector3(0, 0, 0.55f), new Vector3(0, 0, 0.55f), new Vector3(0, 0, 0.55f),//правый бок
               new Vector3(0, 0, 0.7f), new Vector3(0, 0, 0.7f), new Vector3(0, 0, 0.7f), new Vector3(0, 0, 0.7f), 
               new Vector3(0, 0, 0.6f), new Vector3(0, 0, 0.6f), new Vector3(0, 0, 0.65f), new Vector3(0, 0, 0.65f),// низ 
               new Vector3(0, 0, 0.85f), new Vector3(0, 0, 0.85f), new Vector3(0, 0, 1), new Vector3(0, 0, 1), 
               new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 0.35f), new Vector3(0, 0, 0.35f) });//левый бок
            cubeQuads_2 = new VBO<int>(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 }, BufferTarget.ElementArrayBuffer);



            Glut.glutMainLoop();
        }

        private static void OnClose()
        {
   
            cube.Dispose();
            cubeColor.Dispose();
            cubeQuads.Dispose();
            cube_2.Dispose();
            cubeColor_2.Dispose();
            cubeQuads_2.Dispose();
            background.Dispose();
            background_Color.Dispose();
            backgroundElements.Dispose();
            crateTextu.Dispose();
            program.DisposeChildren = true;
            program.Dispose();
            program_b.DisposeChildren = true;
            program_b.Dispose();
        }

        private static void OnDisplay()
        {

        }

        private static void OnRenderFrame()
        {


            // Подготовка к отрисовки
            Gl.Viewport(0, 0, width, height);
            Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Подбор значений поворота , для каждой координаты
            float angle_rot_cube1_Y = 2.8f;
            float angle_rot_cube1_X = 6.5f;
            float angle_rot_cube1_Z = 6.0f;
            float angle_rot_cube2_Y = -5.37f;
            float angle_rot_cube2_X = 0.8f;
            float angle_rot_cube2_Z = 0.1f;

      
            

            Gl.UseProgram(program_b);
            Gl.BindTexture(crateTextu);
            // bind the vertex positions, colors and elements of the square
            program_b["model_matrix"].SetValue(Matrix4.CreateRotationY(angle_rot_cube1_Y) * Matrix4.CreateRotationX(angle_rot_cube1_X) * Matrix4.CreateRotationZ(angle_rot_cube1_Z) * Matrix4.CreateTranslation(new Vector3(1.5f, 0, 0)));
            Gl.BindBufferToShaderAttribute(background, program_b, "vertexPosition_b");
            Gl.BindBufferToShaderAttribute(background_Color, program_b, "vertexUV");
            Gl.BindBuffer(backgroundElements);
            // Используем шейдерную программу
           // Gl.UseProgram(program_b);
            
            // draw the square
            Gl.DrawElements(BeginMode.Quads, backgroundElements.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
            // Используем шейдерную программу
            Gl.UseProgram(program);
            //Gl.BindTexture(crateTextu);



            // Рисуем первый кубик
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(angle_rot_cube2_Y) * Matrix4.CreateRotationX(angle_rot_cube2_X) * Matrix4.CreateRotationZ(angle_rot_cube2_Z) * Matrix4.CreateTranslation(new Vector3(-0.5, 1, 0)));
            Gl.BindBufferToShaderAttribute(cube, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeColor, program, "vertexColor");
            Gl.BindBuffer(cubeQuads);
            Gl.DrawElements(BeginMode.Quads, cubeQuads.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            //Рисуем второй кубик
            program["model_matrix"].SetValue(Matrix4.CreateRotationY(angle_rot_cube1_Y) * Matrix4.CreateRotationX(angle_rot_cube1_X) * Matrix4.CreateRotationZ(angle_rot_cube1_Z) * Matrix4.CreateTranslation(new Vector3(-2.35f, -0.85f, 0)));
            Gl.BindBufferToShaderAttribute(cube_2, program, "vertexPosition");
            Gl.BindBufferToShaderAttribute(cubeColor_2, program, "vertexColor");
            Gl.BindBuffer(cubeQuads_2);
            Gl.DrawElements(BeginMode.Quads, cubeQuads_2.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

           
            Glut.glutSwapBuffers();
        }

        public static string VertexShader = @"
#version 130
in vec3 vertexPosition;
in vec3 vertexColor;
out vec3 color;
uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;
void main(void)
{
    color = vertexColor;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition, 1);
}
";

        public static string FragmentShader = @"
#version 130
in vec3 color;
out vec4 fragment;
void main(void)
{
    fragment = vec4(color, 0.66);
 
}
";


        public static string VertexShader_Back = @"
#version 130
in vec3 vertexPosition_b;
in vec2 vertexUV;
out vec2 uv;
uniform mat4 projection_matrix;
uniform mat4 view_matrix;
uniform mat4 model_matrix;
void main(void)
{
    uv = vertexUV;
    gl_Position = projection_matrix * view_matrix * model_matrix * vec4(vertexPosition_b, 1);
}
";

        public static string FragmentShader_Back = @"
#version 130
uniform sampler2D texture;
in vec2 uv;
out vec4 fragment;
void main(void)
{
    fragment = texture2D(texture, uv);
}
";
    }
}

