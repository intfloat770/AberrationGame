using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using System.Collections.Generic;

class IsometricPass : CustomPass
{
    [SerializeField] float test;
    [SerializeField] Camera cameraRef;
    [SerializeField] Mesh mesh;
    [SerializeField] Material isometricMaterial;
    [SerializeField] List<Transform> transforms;

    // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
    // When empty this render pass will render to the active camera render target.
    // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
    // The render pipeline will ensure target setup and clearing happens in an performance manner.
    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        // Setup code here
    }

    protected override void Execute(CustomPassContext ctx)
    {
        // Executed every frame for all the camera inside the pass volume.
        // The context contains the command buffer to use to enqueue graphics commands.
        
        CommandBuffer cmd = ctx.cmd;

        //ctx.renderContext.

        //Graphics.Blit(ctx.renderContext.);

        foreach (Transform t in transforms)
        {
            if (t == null)
            {
                continue;
            }

            Matrix4x4 matrix = Matrix4x4.TRS(t.position, Quaternion.Euler(-90, 0, 0), Vector3.one * 100);
            ctx.cmd.DrawMesh(mesh, matrix, isometricMaterial, 0, isometricMaterial.FindPass("ForwardOnly"));
        }


    }

    protected override void Cleanup()
    {
        // Cleanup code
    }
}