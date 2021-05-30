using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Ants
{
    public struct Pupa : IComponentData
    {
        public Guid Id { get; private set; }
        public float3 Position { get; set; }
        public float Size { get; set; }
        public double TimeHatched { get; private set; }
        public double TimeLaid { get; private set; }
        public double TimeOfPupation { get; private set; }
        public float Weight => Size * 2.0f; //TODO: Fetch from datasheet?

        public Pupa(Larva larva, double timeOfPupation)
        {
            Id = larva.Id;
            Position = larva.Position;
            Size = GetSize(larva);
            TimeHatched = larva.TimeHatched;
            TimeLaid = larva.TimeLaid;
            TimeOfPupation = timeOfPupation;
        }

        private static float GetSize(Larva larva)
        {
            return larva.Size;
        }

        public Ant Eclose(double timeOfEclosion)
        {
            return new Ant(this, timeOfEclosion);
        }
    }
}