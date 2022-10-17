using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;


namespace ClientTemplate
{
    public interface ISerializeManager : IManager
    {
        void SaveData();
        SaveData LoadData();
    }
    
    [System.Serializable]
    public class SaveData
    {
        //여러가지 타입이 가능하지만, Dictionary는 안된다.
        [SerializeField] private string characterName;
        [SerializeField] private int level;
        [SerializeField] private float exp;
        [SerializeField] private List<string> itemsName;
        [SerializeField] private AAAAA testAAAAA;
 
        //외부 Json 라이브러리가 아닌 JsonUtility를 쓰면 Vector3도 저장할 수 있다.
        [SerializeField] private Vector3 lastPosition;
 
        //생성자
        public SaveData(string t_characterName, int t_level, float t_exp, List<string> t_itemsName, Vector3 t_lastPosition)
        {
            characterName = t_characterName;
            level = t_level;
            exp = t_exp;
 
            //일일이 값을 복사하는게 깔끔하다.
            itemsName = new List<string>();
            foreach(var n in t_itemsName)
            {
                itemsName.Add(n);
            }
 
            lastPosition = new Vector3(t_lastPosition.x, t_lastPosition.y, t_lastPosition.z);

            testAAAAA = new AAAAA();
        }
    }

    [System.Serializable]
    public class AAAAA
    {
        public List<string> testData = new List<string>() { "ABCDE", "BCDEA", "CDEAB", "DEABC", "EBACD"};
    }

    public class SerializeManager : ISerializeManager
    {
        private static readonly string privateKey = "asdy7g9oayiwherulkfg7o4qa2akljhhgo279834";
        
        private FileStream fs;
        private string FileName { get; set; }
        private string FilePath { get; set; } = $"{Application.persistentDataPath}\\save";
        
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

        public void SaveData()
        {
            //원래라면 플레이어 정보나 인벤토리 등에서 긁어모아야 할 정보들.
            SaveData sd = new SaveData(
                "Glick", 
                10, 
                0.1f, 
                new List<string> { "Portion", "Dagger" }, 
                new Vector3(1, 2, 3));
 
            string jsonString = DataToJson(sd);
            string encryptString = Encrypt(jsonString);
            SaveFile(encryptString);
        }

        public SaveData LoadData()
        {
            //파일이 존재하는지부터 체크.
            if(!File.Exists(FilePath))
            {
                Debug.Log("Save file does not exist!");
                return null;
            }
 
            string encryptData = LoadFile(FilePath);
            string decryptData = Decrypt(encryptData);
 
            LogManager.Log(LogManager.LogType.DEFAULT, decryptData);
 
            SaveData sd = JsonToData(decryptData);
            return sd;
        }

        private void SaveFile(string jsonData)
        {
            using (FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
            {
                //파일로 저장할 수 있게 바이트화
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
 
                //bytes의 내용물을 0 ~ max 길이까지 fs에 복사
                fs.Write(bytes, 0, bytes.Length);
            }
        }
        
        private string LoadFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                //파일을 바이트화 했을 때 담을 변수를 제작
                byte[] bytes = new byte[(int)fs.Length];
 
                //파일스트림으로 부터 바이트 추출
                fs.Read(bytes, 0, (int)fs.Length);
 
                //추출한 바이트를 json string으로 인코딩
                string jsonString = System.Text.Encoding.UTF8.GetString(bytes);
                return jsonString;
            }
        }
        
        private string DataToJson(SaveData sd)
        {
            string jsonData = JsonUtility.ToJson(sd);
            return jsonData;
        }
        
        private SaveData JsonToData(string jsonData)
        {
            SaveData sd = JsonUtility.FromJson<SaveData>(jsonData);
            return sd;
        }
        
        private string Encrypt(string data)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
            RijndaelManaged rm = CreateRijndaelManaged();
            ICryptoTransform ct = rm.CreateEncryptor();
            byte[] results = ct.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Convert.ToBase64String(results, 0, results.Length);
        }
        
        private string Decrypt(string data)
        {
            byte[] bytes = System.Convert.FromBase64String(data);
            RijndaelManaged rm = CreateRijndaelManaged();
            ICryptoTransform ct = rm.CreateDecryptor();
            byte[] resultArray = ct.TransformFinalBlock(bytes, 0, bytes.Length);
            return System.Text.Encoding.UTF8.GetString(resultArray);
        }
        
        private static RijndaelManaged CreateRijndaelManaged()
        {
            byte[] keyArray = System.Text.Encoding.UTF8.GetBytes(privateKey);
            RijndaelManaged result = new RijndaelManaged();
 
            byte[] newKeysArray = new byte[16];
            System.Array.Copy(keyArray, 0, newKeysArray, 0, 16);
 
            result.Key = newKeysArray;
            result.Mode = CipherMode.ECB;
            result.Padding = PaddingMode.PKCS7;
            return result;
        }
    }
}
