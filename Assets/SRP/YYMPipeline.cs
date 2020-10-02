using UnityEngine;
using UnityEngine.Rendering;

namespace SRP
{
    public class YYMPipeline : RenderPipeline
    {
        protected override void Render(ScriptableRenderContext context, Camera[] cameras)
        {
            var renderer = new CameraRenderer();
            foreach (var camera in cameras)
            {
                renderer.Render(context, camera);
            }
        }
    }
}
