using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Assets.Scripts.Ants.AI
{
    [BurstCompile]
    public class AntAgent : Agent
    {
        public Transform TargetTransform;
        public Transform Ground;
        private Rigidbody _agentRB;
        private Random _prng;
        //private EntityManager _entityManager;
        //public GameObject PheromonePrefab;

        //private Entity _pheromonePrefabEntity;

        public override void Initialize()
        {
            _agentRB = GetComponent<Rigidbody>();
            _prng = new Random((uint)DateTime.Now.Millisecond);
            //_entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        }

        public override void OnEpisodeBegin()
        {
            ResetAgent();
            ResetTarget();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            //sensor.AddObservation(transform.localPosition);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            MoveAgent(actions.DiscreteActions);

            if(Vector3.Distance(transform.localPosition, Ground.localPosition) > 14f)
            {
                SetReward(-1f);
                EndEpisode();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Food"))
            {
                SetReward(1f);
                EndEpisode();
                //SpawnPheromone();
                //ResetTarget();
            }

            if (collision.collider.CompareTag("Wall"))
            {
                SetReward(-1f);
                EndEpisode();
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.collider.CompareTag("Walkable"))
            {
                SetReward(-1f);
                EndEpisode();
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;

            if (Input.GetKey(KeyCode.W))
            {
                discreteActionsOut[0] = 1;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                discreteActionsOut[0] = 2;
            }
            else
            {
                discreteActionsOut[0] = 0;
            }

            if (Input.GetKey(KeyCode.D))
            {
                discreteActionsOut[1] = 2;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                discreteActionsOut[1] = 1;
            }
            else
            {
                discreteActionsOut[1] = 0;
            }
        }

        private void MoveAgent(ActionSegment<int> act)
        {
            AddReward(-0.0005f);

            var dirToGo = Vector3.zero;
            var rotateDir = Vector3.zero;
            var dirToGoForwardAction = act[0];
            var rotateDirAction = act[1];

            if (dirToGoForwardAction == 1 && _agentRB.velocity.magnitude < 6f)
            {
                dirToGo = 1f * Vector3.forward;
            }
            else if (dirToGoForwardAction == 2 && _agentRB.velocity.magnitude < 3)
            {
                dirToGo = -0.4f * Vector3.forward;
                AddReward(-0.001f);
            }
            if (rotateDirAction == 1)
            {
                rotateDir = transform.up * -1.2f;
                AddReward(-0.0005f);
            }
            else if (rotateDirAction == 2)
            {
                rotateDir = transform.up * 1.2f;
                AddReward(-0.0005f);
            }

            _agentRB.AddRelativeForce(dirToGo, ForceMode.VelocityChange);
            transform.Rotate(rotateDir, Time.fixedDeltaTime * (150f / Mathf.Max(Mathf.Min(_agentRB.velocity.magnitude, 3f), 1f)));
        }

        //private bool IsOnWalkableSurface()
        //{
        //    return Physics.Raycast(transform.localPosition, Vector3.down, out RaycastHit hit, 1f) && hit.collider.CompareTag("Walkable");
        //}

        private void ResetAgent()
        {
            _agentRB.velocity = Vector3.zero;
            transform.position = GetRandomPosition(Ground.position, 1f, 2f);
            transform.rotation = Quaternion.identity;
            transform.Rotate(0f, _prng.NextFloat(0f, 360f), 0f);
        }

        private void ResetTarget()
        {
            TargetTransform.position = GetRandomPosition(Ground.position, 1f, 2f);
        }

        private Vector3 GetRandomPosition(Vector3 zero, float size, float wallDistance)
        {
            var max = 13.5f - wallDistance;
            return _prng.NextFloat3(zero - new Vector3(max, -size / 2, max), zero + new Vector3(max, size / 2, max));
        }

        //private void SpawnPheromone()
        //{
        //    var pheromone = _entityManager.Instantiate(_pheromonePrefabEntity);
        //    _entityManager.AddComponentData(pheromone, new Translation { Value = transform.position });
        //}

        //public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        //{
        //    referencedPrefabs.Add(PheromonePrefab);
        //}

        //public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        //{
        //    _pheromonePrefabEntity = conversionSystem.GetPrimaryEntity(PheromonePrefab);
        //}
    }
}