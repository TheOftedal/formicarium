using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Assets.Scripts.Ants
{
    public class AntMovementSystem : SystemBase
    {
        private EntityCommandBufferSystem _ecbSystem;
        private double _nextActionTime;
        private const float UpdateInterval = 10.0f;

        protected override void OnCreate()
        {
            _ecbSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
            _nextActionTime = 0.0f;
        }

        protected override void OnUpdate()
        {
            if (Time.ElapsedTime > _nextActionTime)
            {
                _nextActionTime += UpdateInterval;

                var ecb = _ecbSystem.CreateCommandBuffer();
                var gameTime = Time.ElapsedTime + GameState.GameStartTime;
                var larvaArchetype = World.EntityManager.CreateArchetype(typeof(Larva), typeof(SpeciesDatasheet));

                Entities.ForEach((Entity entity, ref Ant ant, in SpeciesDatasheet data) =>
                {
                    if(Vector3.Distance(ant.Position, ant.ActivityDestination) > 0.5f)
                    {

                    }
                }).ScheduleParallel();

                _ecbSystem.AddJobHandleForProducer(Dependency);
            }
        }
    }
}