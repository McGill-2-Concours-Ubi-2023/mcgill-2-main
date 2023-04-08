using System;
using System.IO;
using TMPro;
using UnityEngine;

public class TerminalManager : MonoBehaviour
{
    private class GenericUnityTextWriter : System.IO.TextWriter
    {
        private readonly Action<string> m_Write;

        public GenericUnityTextWriter(Action<string> write)
        {
            m_Write = write;
        }

        public override void Write(char value)
        {
            m_Write(value.ToString());
        }

        public override void Write(string value)
        {
            m_Write(value);
        }

        public override void WriteLine(string value)
        {
            m_Write(value + Environment.NewLine);
        }

        public override System.Text.Encoding Encoding => System.Text.Encoding.UTF8;
    }
    
    private NukataLisp.Interp m_Interpreter;
    public TMP_InputField InputField;

    private async void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(transform.root);
        m_Interpreter = await NukataLisp.MakeInterp();
        m_Interpreter.Def("GameObject/Find", 1,
        args =>
        {
            string s = args[0].ToString();
            return GameObject.Find(s);
        });
        m_Interpreter.Def("Console/Hide", 0,
        _ =>
        {
            gameObject.SetActive(false);
            return null;
        });
        m_Interpreter.COut = new GenericUnityTextWriter(Debug.Log);
        
        InputField.onSubmit.AddListener(async textCmd =>
        {
            try
            {
                object result = await NukataLisp.Run(m_Interpreter, new StringReader(textCmd));
                Debug.Log(result);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                throw;
            }
            finally
            {
                InputField.text = "";
            }
        });
    }
}