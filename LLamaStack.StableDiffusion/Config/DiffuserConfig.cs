using LLamaStack.StableDiffusion.Diffusers;

namespace LLamaStack.StableDiffusion.Config
{
    public class DiffuserConfig
    {
        public int TrainTimesteps { get; set; } = 1000;
        public float BetaStart { get; set; } = 0.00085f;
        public float BetaEnd { get; set; } = 0.012f;
        public IEnumerable<float> TrainedBetas { get; set; }
        public DiffuserBetaSchedule BetaSchedule { get; set; } = DiffuserBetaSchedule.ScaledLinear;
        public DiffuserType DiffuserType { get; set; } = DiffuserType.LMSDiffuser;
    }
}