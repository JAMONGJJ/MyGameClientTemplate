using System.Collections;
using System.Collections.Generic;
using ClientTemplate.JobStructs;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace ClientTemplate
{
    namespace JobStructs
    {
        [BurstCompile]
        public struct SquareNumbersJob : IJobParallelFor
        {
            public NativeArray<int> Nums;

            public void Execute(int index)
            {
                Nums[index] *= Nums[index];
            }
        }
    }

    public interface IJobManager : IManager
    {
        void ExecuteJob();
    }

    public class JobManager : IJobManager
    {
        private NativeArray<int> myArray;
        
        public void Init()
        {
            
        }

        public void Release()
        {
            
        }

        public void ReSet()
        {
            Release();
            Init();
        }

        public void ExecuteJob()
        {
            SquareNumbersJob job = new SquareNumbersJob { Nums = myArray };
            JobHandle handle = job.Schedule(
                myArray.Length,
                100);
        }

        public void ExecuteEntity()
        {
            
        }
        
    }
}
