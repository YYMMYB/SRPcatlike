using UnityEngine;
using UnityEngine.Rendering;

namespace SRP
{
    public partial class CameraRenderer
    {
        private ScriptableRenderContext _context;
        private Camera _camera;
        private CommandBuffer _buffer;
        private const string BufferName = "Render Camera";
        
        private CullingResults _cullingResults;
        private static ShaderTagId _unlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");

        public void Render(ScriptableRenderContext context, Camera camera)
        {
            _context = context;
            _camera = camera;
            _buffer = new CommandBuffer(){name = BufferName};

            SetupEditor();
            if (!Cull()) return;
            SetupBuffer();
            Setup();
            Draw();
            DrawLegacy();
            DrawGizmos();
            Submit();
        }

        private bool Cull()
        {
            if (_camera.TryGetCullingParameters(out var cullingParameters))
            {
                _cullingResults = _context.Cull(ref cullingParameters);
                return true;
            }

            return false;
        }

        partial void SetupEditor();

        partial void SetupBuffer();
        private void Setup()
        {
            _context.SetupCameraProperties(_camera);
            var flags = _camera.clearFlags;
            _buffer.ClearRenderTarget(
                 flags <= CameraClearFlags.Depth
                ,flags == CameraClearFlags.Color
                , flags == CameraClearFlags.Color
                    ? _camera.backgroundColor.linear 
                    : Color.clear);
            _buffer.BeginSample(SampleName);
            ExecuteBuffer();
        }

        private void Draw()
        {
            var sortingSettings = new SortingSettings(_camera)
            {
                criteria = SortingCriteria.CommonOpaque,
            };
            var drawingSetting = new DrawingSettings(
                _unlitShaderTagId, sortingSettings
            );
            var filteringSetting = new FilteringSettings(RenderQueueRange.opaque);
            _context.DrawRenderers(_cullingResults, ref drawingSetting, ref filteringSetting);
            _context.DrawSkybox(_camera);
            sortingSettings.criteria = SortingCriteria.CommonTransparent;
            drawingSetting.sortingSettings = sortingSettings;
            filteringSetting.renderQueueRange = RenderQueueRange.transparent;
            _context.DrawRenderers(_cullingResults, ref drawingSetting, ref filteringSetting);
        }

        partial void DrawLegacy();
        partial void DrawGizmos();

        private void Submit()
        {
            _buffer.EndSample(SampleName);
            ExecuteBuffer();
            _context.Submit();
        }

        private void ExecuteBuffer()
        {
            _context.ExecuteCommandBuffer(_buffer);
            _buffer.Clear();
        }
    }
}
