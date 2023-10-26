using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClientTemplate
{
    namespace DataManufactureInfo
    {
        public delegate IParseOutput ParseDelegate(IParseInput input, IParseHelper helper);
        
        public enum ParseType
        {
            None,
            Test,

            
        }

        public interface IParseOutput {}

        public interface IParseInput {}
    
        public interface IParseHelper {}

        public interface IParseDelegateContainer
        {
            void Add(ParseType type, ParseDelegate action);
            ParseDelegate GetDelegate(ParseType type);
        }

        public class ParseDelegateContainer : IParseDelegateContainer
        {
            private Dictionary<ParseType, ParseDelegate> DelegateContainer { get; set; }

            public ParseDelegateContainer()
            {
                DelegateContainer = new Dictionary<ParseType, ParseDelegate>();
            }

            public void Add(ParseType type, ParseDelegate action)
            {
                if (DelegateContainer.ContainsKey(type) == false)
                {
                    DelegateContainer.Add(type, action);
                }
            }

            public ParseDelegate GetDelegate(ParseType type)
            {
                if (DelegateContainer.ContainsKey(type) == true)
                {
                    return DelegateContainer[type];
                }

                return null;
            }
        }
    }
}