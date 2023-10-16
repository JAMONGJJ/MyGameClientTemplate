using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace ClientTemplate
{
    public class NetworkCommandLine : MonoBehaviour
    {
        private Unity.Netcode.NetworkManager Manager;

        private void Start()
        {
            Manager = GetComponentInParent<Unity.Netcode.NetworkManager>();

            if (Application.isEditor) return;

            var args = GetCommandlineArgs();

            if (args.TryGetValue("-mode", out string mode))
            {
                switch (mode)
                {
                    case "server":
                    {
                        Manager.StartServer();
                    }
                        break;
                    case "host":
                    {
                        Manager.StartHost();
                    }
                        break;
                    case "client":
                    {
                        Manager.StartClient();
                    }
                        break;
                }
            }
        }

        private Dictionary<string, string> GetCommandlineArgs()
        {
            Dictionary<string, string> argDictionary = new Dictionary<string, string>();

            var args = System.Environment.GetCommandLineArgs();

            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i].ToLower();
                if (arg.StartsWith("-"))
                {
                    var value = i < args.Length - 1 ? args[i + 1].ToLower() : null;
                    value = (value?.StartsWith("-") ?? false) ? null : value;

                    argDictionary.Add(arg, value);
                }
            }

            return argDictionary;
        }
    }
}
