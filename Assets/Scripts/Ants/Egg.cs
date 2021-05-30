using System;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Ants
{
    [BurstCompile]
    public struct Egg: IComponentData
    {
        public double HatchingDelay { get; private set; }
        public Guid Id { get; private set; }
        public float3 Position { get; set; }
        public double TimeLaid { get; private set; }
        public float Size { get; set; }
        public float Weight => Size * 2.0f; //TODO: Fetch from datasheet?

        public Egg(float3 position, float size, float timeLaid, float hatchDelay)
        {
            HatchingDelay = hatchDelay;
            Id = Guid.NewGuid();
            Position = position;
            TimeLaid = timeLaid;
            Size = size;
        }

        public Larva Hatch(double timeHatched)
        {
            return new Larva(this, timeHatched);
        }
    }
}