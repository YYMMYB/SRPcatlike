using UnityEngine;
using UnityEngine.Rendering;

namespace SRP
{
    [CreateAssetMenu(menuName = SRPConst.RootMenuName + SRPAsset.MenuName, fileName = SRPAsset.FileName)]
    public class SRPAsset : RenderPipelineAsset
    {
        public const string MenuName = "/SRP Asset";
        public const string FileName = "SRP Asset";
        protected override RenderPipeline CreatePipeline()
        {
            return new YYMPipeline();
        }
    }
}
