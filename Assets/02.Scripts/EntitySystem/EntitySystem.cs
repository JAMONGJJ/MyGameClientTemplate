using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using ClientTemplate.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;
using UnityEngine;
using Unity.Entities;

namespace ClientTemplate
{
    namespace Entities
    {
        [BurstCompile]
        public struct MyEntity1 : IComponentData
        {
            
        }

        [BurstCompile]
        public struct MyEntity2 : IComponentData
        {

        }

        [BurstCompile]
        public struct MyEntity3 : IComponentData
        {

        }
    }

    [BurstCompile]
    public static class MyEntity
    {
        public static MyEntitySystem System { get; } = new MyEntitySystem();
    }

    [BurstCompile]
    [UpdateInGroup(typeof(MyEntitySystemGroup))]
    public partial struct MyEntitySystem : ISystem
    {
        private EntityQuery myQuery;
        private ComponentTypeHandle<MyEntity1> entity1Handle;
        private ComponentTypeHandle<MyEntity2> entity2Handle;
        private EntityTypeHandle entityHandle;
        
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityManager manager = state.EntityManager;

            Entity entity = manager.CreateEntity();
            manager.AddComponent<MyEntity1>(entity);
            manager.AddComponent<MyEntity2>(entity);
            manager.AddComponent<MyEntity3>(entity);

            manager.RemoveComponent<MyEntity3>(entity);

            manager.SetComponentData<MyEntity1>(entity, new MyEntity1());
            MyEntity1 myEntity1 = manager.GetComponentData<MyEntity1>(entity);

            bool hasMyEntity1 = manager.HasComponent<MyEntity1>(entity);

            manager.DestroyEntity(entity);

            var types = new NativeArray<ComponentType>(3, Allocator.Temp);
            types[0] = ComponentType.ReadWrite<MyEntity1>();
            types[1] = ComponentType.ReadWrite<MyEntity2>();
            EntityArchetype archeType = manager.CreateArchetype(types);

            Entity entity2 = manager.CreateEntity(archeType);
            Entity entiry3 = manager.Instantiate(entity2);

        }
    }

    public class MyEntitySystemGroup : ComponentSystemGroup
    {
        
    }
}