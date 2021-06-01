using Unity.Entities;

[GenerateAuthoringComponent]
public struct Pheromone : IComponentData
{
    public Entity Value;
}