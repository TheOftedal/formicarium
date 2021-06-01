using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.Ants
{
    public struct Ant : IComponentData
    {
        public float3 ActivityDestination { get; set; }
        public AntCaste Caste { get; private set; }
        public int Health { get; set; }
        public int Hunger { get; set; }
        public Guid Id { get; private set; }
        public FixedString32 Name { get; set; }
        public float3 Position { get; set; }
        public float Size { get; set; }
        public float Strength => Weight * 10.0f; //TODO: Fetch from datasheet?
        public int Thirst { get; set; }
        public double TimeHatched { get; private set; }
        public double TimeLaid { get; private set; }
        public double TimeOfEclosion { get; private set; }
        public double TimeOfPupation { get; private set; }
        public float Weight => Size * 2.0f; //TODO: Fetch from datasheet?

        public Ant(Pupa pupa, double timeOfEclosion)
        {
            ActivityDestination = pupa.Position;
            Caste = GetAntCaste(pupa);
            Health = 100;
            Hunger = 25;
            Id = pupa.Id;
            Name = "";
            Position = pupa.Position;
            Size = GetSize(pupa);
            Thirst = 25;
            TimeHatched = pupa.TimeHatched;
            TimeLaid = pupa.TimeLaid;
            TimeOfEclosion = timeOfEclosion;
            TimeOfPupation = pupa.TimeOfPupation;
        }

        public Ant(float3 position, float size, AntCaste caste, double timeOfEclosion)
        {
            ActivityDestination = position;
            Caste = caste;
            Health = 100;
            Hunger = 0;
            Id = new Guid();
            Name = "";
            Position = position;
            Size = size;
            Thirst = 0;
            TimeHatched = timeOfEclosion;
            TimeLaid = timeOfEclosion;
            TimeOfEclosion = timeOfEclosion;
            TimeOfPupation = timeOfEclosion;
        }

        private static AntCaste GetAntCaste(Pupa pupa)
        {
            //TODO: Implement logic
            return AntCaste.Worker;
        }

        private static float GetSize(Pupa pupa)
        {
            //var prng = new Unity.Mathematics.Random();
            //return prng.NextInt();
            return 1f;
        }

        private static float GetStrength(Pupa pupa)
        {
            //TODO: Implement logic
            return 1f;
        }

        public static Ant SpawnQueen(float3 position, double timeOfEclosion)
        {
            return new Ant(position, 1f, AntCaste.Queen, timeOfEclosion);
        }
    }
}