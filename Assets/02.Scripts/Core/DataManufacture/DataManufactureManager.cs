using System.Collections;
using System.Collections.Generic;
using ClientTemplate.DataManufactureInfo;
using UnityEngine;

namespace ClientTemplate
{
    public interface IDataManufactureManager : IManager
    {
        void SetDelegateContainer(IParseDelegateContainer container);
        IParseOutput Parse(ParseType type, IParseInput inputData, IParseHelper helper = null);
    }

    public partial class DataManufactureManager : IDataManufactureManager
    {
        private IParseDelegateContainer _myDelegateContainer;

        public void Init()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_INIT, "Data Manufacture Manager");
            _myDelegateContainer.Add(ParseType.PlayerInfo, ParsePlayerInfo);
        }

        public void Release()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RELEASE, "Data Manufacture Manager");
            _myDelegateContainer = null;
        }

        public void ReSet()
        {
            LogManager.Log(LogManager.LogType.CONTROLLER_RESET, "Data Manufacture Manager");
            Release();
            Init();
        }

        /// <summary>
        /// _myDelegateContainer setter
        /// </summary>
        /// <param name="container"></param>
        public void SetDelegateContainer(IParseDelegateContainer container)
        {
            _myDelegateContainer = container;
        }

        /// <summary>
        /// ParseType, IParseInput, 그리고 필요하다면 IParseHelper를 파라미터로 넣으면 그에 맞는
        /// delegate를 호출해 데이터를 파싱하고 결과물 IParseOutput을 리턴하는 함수
        /// </summary>
        /// <param name="type"></param>
        /// <param name="inputData"></param>
        /// <param name="helper"></param>
        /// <returns></returns>
        public IParseOutput Parse(ParseType type, IParseInput inputData, IParseHelper helper = null)
        {
            ParseDelegate action = _myDelegateContainer.GetDelegate(type);
            if (action != null)
            {
                return action(inputData, helper);
            }
            return null;
        }
    }
}
