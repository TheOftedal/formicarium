using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Ants
{
    public struct Larva : IComponentData
    {
        public Guid Id { get; private set; }
        public int Health { get; set; }
        public int Hunger { get; set; }
        public float3 Position { get; set; }
        public double TimeHatched { get; private set; }
        public double TimeLaid { get; private set; }
        public float Size { get; set; }
        public int Thirst { get; set; }
        public float Weight => Size * 2.0f; //TODO: Fetch from datasheet?

        public Larva(Egg egg, double timeHatched)
        {
            Id = egg.Id;
            Health = 100;
            Hunger = 0;
            Position = egg.Position;
            TimeHatched = timeHatched;
            TimeLaid = egg.TimeLaid;
            Size = 0.1f;
            Thirst = 0;
        }

        public Pupa Pupate(double timeOfPupation)
        {
            return new Pupa(this, timeOfPupation);
        }
    }
}