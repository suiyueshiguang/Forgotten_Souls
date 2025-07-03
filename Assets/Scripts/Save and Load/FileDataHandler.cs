using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class FileDataHandler
{
    private string dataDirPath = "";
    private string dataFileName = "";

    //ʹ��AES����
    private bool encryptData;
    private byte[] cipher;
    private byte[] iv;

    public FileDataHandler(string _dataDirPath, string _dataFileName, bool _encryptData)
    {
        dataDirPath = _dataDirPath;
        dataFileName = _dataFileName;
        encryptData = _encryptData;
    }

    //����Ĵ���ʵ�ڲ����Ϳ�΢��
    public void Save(GameData _data)
    {
        //ƴ�ӵõ�����·��
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //������תΪjson��ʽ
            string dataToStore = JsonUtility.ToJson(_data, true);

            if (encryptData)
            {
                dataToStore = EncryptData(dataToStore);
            }

            //ʹ�� using ���ȷ�� FileStream ��ʹ����Ϻ���ȷ�ͷţ�������й���Դռ�ù����ڴ�
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
            Debug.LogError($"���Խ����ݱ�����·��Ϊ{fullPath}��,��������:\n{e}");
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

                //���ļ�,д��dataToLoad
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

                //��json��ʽ������GameData
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("���Խ����ݶ�ȡ��·��Ϊ" + fullPath + "��,��������:\n" + e);
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
    /// �Դ浵����AES����
    /// </summary>
    /// <param name="_data">��Ҫ���ܵ���Ϣ</param>
    /// <returns>����ɼ��ܵ���Ϣ</returns>
    private string EncryptData(string _data)
    {
        byte[] encrypted;

        using (Aes aes = Aes.Create())
        {
            aes.Key = cipher;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            //Ϊϵͳ�ڴ��ṩ��ʽ�Ķ�д����
            using (MemoryStream memoryStream = new MemoryStream())
            {
                //���彫���������ӵ�����ת������
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    //�������ض����뽫�ַ�д����
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(_data);
                    }
                    encrypted = memoryStream.ToArray();
                }
            }
        }

        //�����ܺ������תΪbase64
        return Convert.ToBase64String(encrypted);
    }

    /// <summary>
    /// �Դ浵����AES����
    /// </summary>
    /// <param name="_data">��Ҫ���ܵ���Ϣ</param>
    /// <returns>����ɽ��ܵ���Ϣ</returns>
    private string DecryptData(string _data)
    {
        //��base64תΪ�ֽ�����
        byte[] encryptedData = Convert.FromBase64String(_data);
        string resultData = "";

        using (Aes aes = Aes.Create())
        {
            aes.Key = cipher;
            aes.IV = iv;

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            //Ϊϵͳ�ڴ��ṩ��ʽ�Ķ�д����
            using (MemoryStream memoryStream = new MemoryStream(encryptedData))
            {
                //���彫���������ӵ�����ת������
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    //�������ض����뽫�ַ�д����
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
