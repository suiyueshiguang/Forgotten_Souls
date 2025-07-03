using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    //使用AES加密
    private bool encryptData;
    private byte[] cipher;
    private byte[] iv;

    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    //这里的代码实在不懂就看微软
    public void Save(GameData _data)
    {
        //拼接得到绝对路径
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //将数据转为json格式
            string dataToStore = JsonUtility.ToJson(_data, true);

            if (encryptData)
            {
                dataToStore = EncryptData(dataToStore);
            }

            //使用 using 语句确保 FileStream 在使用完毕后被正确释放，避免非托管资源占用过多内存
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"尝试将数据保存在路径为{fullPath}中,错误如下:\n{e}");
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";

                //读文件,写到dataToLoad
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (encryptData)
                {
                    dataToLoad = DecryptData(dataToLoad);
                }

                //将json格式解析成GameData
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("尝试将数据读取，路径为" + fullPath + "中,错误如下:\n" + e);
            }
        }

        return loadData;
    }

    public void Delete()
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    public void UpdateCipherIV(byte[] _cipher, byte[] _iv)
    {
        cipher = _cipher;
        iv = _iv;
    }

    /// <summary>
    /// 对存档进行AES加密
    /// </summary>
    /// <param name="_data">需要加密的信息</param>
    /// <returns>已完成加密的信息</returns>
    private string EncryptData(string _data)
    {
        byte[] encrypted;

        using (Aes aes = Aes.Create())
        {
            aes.Key = cipher;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            //为系统内存提供流式的读写操作
            using (MemoryStream memoryStream = new MemoryStream())
            {
                //定义将数据流链接到加密转换的流
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    //用于以特定编码将字符写入流
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(_data);
                    }
                    encrypted = memoryStream.ToArray();
                }
            }
        }

        //将加密后的数据转为base64
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// 对存档进行AES解密
    /// </summary>
    /// <param name="_data">需要解密的信息</param>
    /// <returns>已完成解密的信息</returns>
    private string DecryptData(string _data)
    {
        //从base64转为字节数组
        byte[] encryptedData = Convert.FromBase64String(_data);
        string resultData = "";

        using (Aes aes = Aes.Create())
        {
            aes.Key = cipher;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            //为系统内存提供流式的读写操作
            using (MemoryStream memoryStream = new MemoryStream(encryptedData))
            {
                //定义将数据流链接到解密转换的流
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    //用于以特定编码将字符写入流
                    using (StreamReader streamReader = new StreamReader(cryptoStream))
                    {
                        resultData = streamReader.ReadToEnd();
                    }
                }
            }
        }

        return resultData;
    }
}
