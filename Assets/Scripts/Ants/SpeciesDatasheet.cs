using Unity.Entities;

namespace Assets.Scripts.Ants
{
    public struct SpeciesDatasheet : IComponentData
    {
        public int EggCount { get; set; }
        public float MinHatchingTime { get; set; }
        public float MaxHatchingTime { get; set; }
        public Species Species { get; set; }
    }
}