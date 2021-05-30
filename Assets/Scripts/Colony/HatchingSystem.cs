using Assets.Scripts.Ants;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;

namespace Assets.Scripts.Colony
{
    [BurstCompile]
    public class HatchingSystem : SystemBase
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
            if(Time.ElapsedTime > _nextActionTime)
            {
                _nextActionTime += UpdateInterval;

                var ecb = _ecbSystem.CreateCommandBuffer();
                var gameTime = Time.ElapsedTime + GameState.GameStartTime;
                var larvaArchetype = World.EntityManager.CreateArchetype(typeof(Larva), typeof(SpeciesDatasheet));

                Entities.ForEach((Entity entity, ref Egg egg, in SpeciesDatasheet data) =>
                {
                    var age = gameTime - egg.TimeLaid;
                    if (age - egg.HatchingDelay > data.MinHatchingTime)
                    {
                        var larva = egg.Hatch(gameTime);
                        ecb.DestroyEntity(entity);

                        var larvaEntity = ecb.CreateEntity(larvaArchetype);

                        ecb.AddComponent(larvaEntity, larva);
                        ecb.AddComponent(larvaEntity, data);
                    }
                }).ScheduleParallel();

                _ecbSystem.AddJobHandleForProducer(Dependency);
            }
        }
    }
}