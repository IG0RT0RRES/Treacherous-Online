using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class SaveDados : MonoBehaviour
{
    public static string Nickname;
    private Login _login;

    private void Awake()
    {
        _login = FindObjectOfType(typeof(Login)) as Login;
    }

    public void Savar()
    {
        if (File.Exists(Application.persistentDataPath + _login.Name))
        {
            _login.Info = "this user is not available";
            _login.Clear();
        }
        else if (!File.Exists(Application.persistentDataPath + _login.Name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Create(Application.persistentDataPath + _login.Name);

            SalvaDados dados = new SalvaDados();
            dados.Name = _login.Name;
            dados.PassWord = _login.Password;
            bf.Serialize(fs, dados);
            fs.Close();
            _login.Info = "Created successfully!";
            _login.Clear();
        }
    }


    public void DeletaConta()
    {
        if (File.Exists(Application.persistentDataPath + _login.Name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + _login.Name, FileMode.Open);
            SalvaDados dados = (SalvaDados)bf.Deserialize(fs);

            if (dados.PassWord == _login.Password)
            {
                fs.Close();
                File.Delete(Application.persistentDataPath + _login.Name);
                _login.Info = "Account successfully deleted!";
                _login.Clear();
            }
            else
            {
                _login.Info = "Wrong Password, could not be deleted !";
                _login.Clear();
            }

            fs.Close();
        }
        else
        {
            _login.Info = "Does not exist!";
            _login.Clear();
        }
    }

    public void LoadDados()
    {
        if (File.Exists(Application.persistentDataPath + _login.Name))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = File.Open(Application.persistentDataPath + _login.Name, FileMode.Open);
            SalvaDados dados = (SalvaDados)bf.Deserialize(fs);

            if (dados.PassWord == _login.Password)
            {
                fs.Close();
                Nickname = dados.Name;
                SceneManager.LoadScene(1);
            }
            else
            {
                _login.Info = "Wrong Password !";
                _login.Clear();
            }

            fs.Close();
        }
        else
        {
            _login.Info = "Name or Password does not exist, check that they are correct.";
            _login.Clear();
        }
    }
}

[Serializable]
class SalvaDados
{
    public string Name;
    public string PassWord;
}