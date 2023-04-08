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
    
    private static class LispBindings
    {
        public static object GameObject_Find(object[] args)
        {
            string s = args[0].ToString();
            return GameObject.Find(s);
        }
        
        public static object GameObject_GetTransform(object[] args)
        {
            return ((GameObject) args[0]).transform;
        }
        
        public static object Transform_GetPosition(object[] args)
        {
            return ((Transform) args[0]).position;
        }
        
        public static object Transform_SetPosition(object[] args)
        {
            ((Transform) args[0]).position = (Vector3) args[1];
            return null;
        }
        
        public static object Transform_GetRotation(object[] args)
        {
            return ((Transform) args[0]).rotation;
        }
        
        public static object Transform_SetRotation(object[] args)
        {
            ((Transform) args[0]).rotation = (Quaternion) args[1];
            return null;
        }
        
        public static object Transform_GetScale(object[] args)
        {
            return ((Transform) args[0]).localScale;
        }
        
        public static object Transform_SetScale(object[] args)
        {
            ((Transform) args[0]).localScale = (Vector3) args[1];
            return null;
        }
        
        public static object Transform_GetParent(object[] args)
        {
            return ((Transform) args[0]).parent;
        }
        
        public static object Transform_SetParent(object[] args)
        {
            ((Transform) args[0]).SetParent((Transform) args[1]);
            return null;
        }
        
        public static object Transform_GetChild(object[] args)
        {
            return ((Transform) args[0]).GetChild((int) args[1]);
        }
        
        public static object Transform_GetChildCount(object[] args)
        {
            return ((Transform) args[0]).childCount;
        }
        
        public static object Console_Hide(object[] args)
        {
            GameObject.Find("Terminal").SetActive(false);
            return null;
        }
        
        public static object GameObject_FindWithTag(object[] args)
        {
            return GameObject.FindWithTag(args[0].ToString());
        }
        
        public static object Object_ToString(object[] args)
        {
            return args[0].ToString();
        }
        
        // Vector3
        public static object Vector3_GetX(object[] args)
        {
            return ((Vector3) args[0]).x;
        }
        
        public static object Vector3_GetY(object[] args)
        {
            return ((Vector3) args[0]).y;
        }
        
        public static object Vector3_GetZ(object[] args)
        {
            return ((Vector3) args[0]).z;
        }
        
        public static object Vector3_SetX(object[] args)
        {
            Vector3 v = (Vector3) args[0];
            v.x = (float) args[1];
            return v;
        }
        
        public static object Vector3_SetY(object[] args)
        {
            Vector3 v = (Vector3) args[0];
            v.y = (float) args[1];
            return v;
        }
        
        public static object Vector3_SetZ(object[] args)
        {
            Vector3 v = (Vector3) args[0];
            v.z = (float) args[1];
            return v;
        }
        
        public static object Vector3_New(object[] args)
        {
            return new Vector3(ToFloat(args[0]), ToFloat(args[1]), ToFloat(args[2]));
        }
        
        private static float ToFloat(object o)
        {
            if (o is IConvertible c)
            {
                return c.ToSingle(null);
            }
            return (float) o;
        }
    }

    private async void Awake()
    {
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(transform.root);
        m_Interpreter = await NukataLisp.MakeInterp();
        m_Interpreter.Def("GameObject/Find", 1, LispBindings.GameObject_Find);
        m_Interpreter.Def("Console/Hide", 0, LispBindings.Console_Hide);
        m_Interpreter.Def("GameObject/FindWithTag", 1, LispBindings.GameObject_FindWithTag);
        m_Interpreter.Def("Object/ToString", 1, LispBindings.Object_ToString);
        m_Interpreter.Def("GameObject/GetTransform", 1, LispBindings.GameObject_GetTransform);
        m_Interpreter.Def("Transform/GetPosition", 1, LispBindings.Transform_GetPosition);
        m_Interpreter.Def("Transform/SetPosition", 2, LispBindings.Transform_SetPosition);
        m_Interpreter.Def("Transform/GetRotation", 1, LispBindings.Transform_GetRotation);
        m_Interpreter.Def("Transform/SetRotation", 2, LispBindings.Transform_SetRotation);
        m_Interpreter.Def("Transform/GetScale", 1, LispBindings.Transform_GetScale);
        m_Interpreter.Def("Transform/SetScale", 2, LispBindings.Transform_SetScale);
        m_Interpreter.Def("Transform/GetParent", 1, LispBindings.Transform_GetParent);
        m_Interpreter.Def("Transform/SetParent", 2, LispBindings.Transform_SetParent);
        m_Interpreter.Def("Transform/GetChild", 2, LispBindings.Transform_GetChild);
        m_Interpreter.Def("Transform/GetChildCount", 1, LispBindings.Transform_GetChildCount);
        m_Interpreter.Def("Vector3/GetX", 1, LispBindings.Vector3_GetX);
        m_Interpreter.Def("Vector3/GetY", 1, LispBindings.Vector3_GetY);
        m_Interpreter.Def("Vector3/GetZ", 1, LispBindings.Vector3_GetZ);
        m_Interpreter.Def("Vector3/SetX", 2, LispBindings.Vector3_SetX);
        m_Interpreter.Def("Vector3/SetY", 2, LispBindings.Vector3_SetY);
        m_Interpreter.Def("Vector3/SetZ", 2, LispBindings.Vector3_SetZ);
        m_Interpreter.Def("Vector3/New", 3, LispBindings.Vector3_New);

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