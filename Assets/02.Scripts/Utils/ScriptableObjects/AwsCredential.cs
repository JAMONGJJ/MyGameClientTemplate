using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Herma
{
    [CreateAssetMenu(fileName = "AwsCredential", menuName = "ScriptableObject/AwsCredential")]
    public class AwsCredential : ScriptableObject
    {
        public string AwsBucketName;
        public string AwsAccessKey;
        public string AwsSecretKey;
    }
}
