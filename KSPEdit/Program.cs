using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.Windows;
using System.Drawing;


namespace KSPEdit
{
    struct Vertex
    {
        public Vector4 Position;
        public int Color;
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = new Form1();

            var panel = form.SplitContainer.Panel1;

            var device = new Device(new Direct3D(), 0, DeviceType.Hardware, panel.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters()
            {
                BackBufferWidth = panel.ClientSize.Width,
                BackBufferHeight = panel.ClientSize.Height
                
            });

            
            var vertices = new VertexBuffer(device, 3 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            vertices.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { Color = Color.Red.ToArgb(), Position = new Vector4(400.0f, 100.0f, 0.5f, 1.0f) },
                new Vertex() { Color = Color.Blue.ToArgb(), Position = new Vector4(650.0f, 500.0f, 0.5f, 1.0f) },
                new Vertex() { Color = Color.Green.ToArgb(), Position = new Vector4(150.0f, 500.0f, 0.5f, 1.0f) }
            });
            vertices.Unlock();

            var vertices2 = new VertexBuffer(device, 3 * 20, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            vertices2.Lock(0, 0, LockFlags.None).WriteRange(new[] {
                new Vertex() { Color = Color.Red.ToArgb(), Position = new Vector4(300.0f, 100.0f, 0.5f, 1.0f) },
                new Vertex() { Color = Color.Blue.ToArgb(), Position = new Vector4(550.0f, 500.0f, 0.5f, 1.0f) },
                new Vertex() { Color = Color.Green.ToArgb(), Position = new Vector4(050.0f, 500.0f, 0.5f, 1.0f) }
            });
            vertices2.Unlock();
             

            var vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
        		new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
        	};

            var vertexDecl = new VertexDeclaration(device, vertexElems);


            MessagePump.Run(form, () =>
            {
                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();

                device.SetStreamSource(0, vertices, 0, 20);
                device.VertexDeclaration = vertexDecl;
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);

                device.SetStreamSource(0, vertices2, 0, 20);
                device.VertexDeclaration = vertexDecl;
                device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);

                device.EndScene();
                device.Present();
            });

            foreach (var item in ObjectTable.Objects)
                item.Dispose();
        }


    }
}
