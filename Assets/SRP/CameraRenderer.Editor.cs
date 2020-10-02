using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace SRP
{
    public partial class CameraRenderer
    {
#if UNITY_EDITOR
        private string SampleName { get; set; }
#else
        const string SampleName = bufferName;
#endif
#if UNITY_EDITOR
        private static ShaderTagId[] _legacyShaderTagIds =
        {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM")
        };

        private static Material _errorMaterial;

        partial void SetupEditor()
        {
            if (_camera.cameraType == CameraType.SceneView)
            {
                ScriptableRenderContext.EmitWorldGeometryForSceneView(_camera);
            }
        }

        partial void SetupBuffer()
        {
            Profiler.BeginSample("Editor Only");
            _buffer.name = SampleName = _camera.name;
            Profiler.EndSample();
        }
        partial void DrawLegacy()
        {
            _errorMaterial = _errorMaterial != null
                ? _errorMaterial
                : new Material(Shader.Find("Hidden/InternalErrorShader"));

            var drawingSettings = new DrawingSettings(
                _legacyShaderTagIds[0], new SortingSettings(_camera)
            )
            {
                overrideMaterial = _errorMaterial
            };
            for (int i = 1; i < _legacyShaderTagIds.Length; i++)
            {
                drawingSettings.SetShaderPassName(i, _legacyShaderTagIds[i]);
            }

            var filteringSettings = new FilteringSettings(RenderQueueRange.all);
            _context.DrawRenderers(_cullingResults, ref drawingSettings, ref filteringSettings);
        }

        partial void DrawGizmos()
        {
            if (Handles.ShouldRenderGizmos())
            {
                _context.DrawGizmos(_camera, GizmoSubset.PreImageEffects);
                _context.DrawGizmos(_camera, GizmoSubset.PostImageEffects);
            }
        }
        
#endif

    }
}