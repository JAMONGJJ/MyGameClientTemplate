using System.Collections;
using System.Collections.Generic;
using Unity.Core;
using Unity.Entities;
using UnityEngine;

namespace ClientTemplate
{
    public static class Entity
    {
        public static EntitySystem System { get; } = new EntitySystem();
    }
    public class EntitySystem
    {

    }
}