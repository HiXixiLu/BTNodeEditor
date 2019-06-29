using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class CustomFontUtility : EditorWindow
{
    /* 首先得使用 Sprite Editor 切割 以Sprite为保存模式 的贴图资源 （  ::>_<::  手写代码的能力多么薄弱）
    * 1. 贴图资源保存为Sprite，Sprite Mode 设置为 Multiple
    * 2. 选中贴图，打开 Sprite Editor , 使用 Slice 功能进行切割, 切割 Pivot 目前暂定为左下角(重要)，其他选项使用默认值，每个Sprite大小可以在Editor里单调
    * 3. 将切割的每个 Sprite 的 Name属性 设置为对应的 ASCII 索引 
    * 4. 打开菜单项 - 拖入字体贴图 - 输入字体名字 - 选择文件保存路径 - "创建"
    */
    [MenuItem("工具/创建 CustomFont")]
    public static void Open()
    {
        GetWindow<CustomFontUtility>("创建字体");
    }
    private Texture2D tex;
    private string fontName;
    private string fontPath;

    // 创建悬浮窗口
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        GUILayout.Label("字体图片：");
        tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), true);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("字体名称：");
        fontName = EditorGUILayout.TextField(fontName);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(string.IsNullOrEmpty(fontPath) ? "选择路径" : fontPath))
        {
            fontPath = EditorUtility.OpenFolderPanel("字体路径", Application.dataPath, ""); //打开一个选择文件的 Folder Explorer
            if (string.IsNullOrEmpty(fontPath))
            {
                Debug.Log("取消选择路径");
            }
            else
            {
                fontPath = fontPath.Replace(Application.dataPath, "") + "/";
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建"))
        {
            Create();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void Create()
    {
        if (tex == null)
        {
            Debug.LogWarning("图片为空！");
            return;
        }
        if (string.IsNullOrEmpty(fontPath))
        {
            Debug.LogWarning("字体路径为空！");
            return;
        }
        if (fontName == null)
        {
            Debug.LogWarning("请输入字体名称！");
            return;
        }
        else
        {
            if (File.Exists(Application.dataPath + fontPath + ".fontsettings"))
            {
                Debug.LogError("已存在同名字体文件！");
                return;
            }
            if (File.Exists(Application.dataPath + fontPath + ".mat"))
            {
                Debug.LogError("已存在同名材质文件！");
                return;
            }
        }

        string selectionPath = AssetDatabase.GetAssetPath(tex);
        if (selectionPath.Contains("/Resources/"))
        {
            string selectionExt = Path.GetExtension(selectionPath);
            if (selectionExt.Length == 0)
            {
                Debug.LogError("");
            }
            string fontPathName = fontPath + fontName + ".fontsettings";
            string matPathName = fontPath + fontName + ".mat";

            // 开始各项计算
            float lineSpace = 0.1f;
            string loadPath = selectionPath.Replace(selectionExt, "").Substring(selectionPath.IndexOf("/Resources/") + "/Resources/".Length);
            Sprite[] subSprites = Resources.LoadAll<Sprite>(loadPath);

            if (subSprites.Length > 0)
            {

                Material mat = new Material(Shader.Find("GUI/Text Shader"));    // 选择 shader 类型
                mat.SetTexture("_MainTex", tex);
                Font m_myFont = new Font();
                m_myFont.material = mat;

                CharacterInfo[] characterInfos = new CharacterInfo[subSprites.Length];
                for (int i = 0; i < subSprites.Length; i++)
                {
                    if (subSprites[i].rect.height > lineSpace)
                    {
                        lineSpace = subSprites[i].rect.height;
                    }
                }
                for (int i = 0; i < subSprites.Length; i++)
                {
                    Sprite sp = subSprites[i];
                    CharacterInfo info = new CharacterInfo();   // 管理从贴图中渲染字母的类

                    try
                    {
                        info.index = System.Convert.ToInt32(sp.name);   // sp.name 转化为ASCII 码 index
                    }
                    catch
                    {
                        Debug.LogError("创建失败，sprite名称错误");
                        return;
                    }

                    Rect rect = sp.rect;    // sp: 切割好的单个精灵图
                    //float pivot = sp.pivot.y / rect.height - 0.5f;
                    //if (pivot > 0)
                    //{
                    //    pivot = -lineSpace / 2 - sp.pivot.y;
                    //}
                    //else if (pivot < 0)
                    //{
                    //    pivot = -lineSpace / 2 + rect.height - sp.pivot.y;
                    //}
                    //else
                    //{
                    //    pivot = -lineSpace / 2;
                    //}

                    //int offsetY = (int)(pivot + (lineSpace - rect.height) / 2);
                    int offsetY = (int)rect.height / 2;
                    info.uvBottomLeft = new Vector2((float)rect.x / tex.width, (float)(rect.y) / tex.height);
                    info.uvBottomRight = new Vector2((float)(rect.x + rect.width) / tex.width, (float)(rect.y) / tex.height);
                    info.uvTopLeft = new Vector2((float)rect.x / tex.width, (float)(rect.y + rect.height) / tex.height);
                    info.uvTopRight = new Vector2((float)(rect.x + rect.width) / tex.width, (float)(rect.y + rect.height) / tex.height);

                    // 以下为屏幕视口坐标系，原点为 top-left
                    info.minX = 0;
                    info.minY = -(int)rect.height + offsetY;
                    info.maxX = (int)rect.width;
                    info.maxY = offsetY;

                    info.advance = (int)rect.width;
                    characterInfos[i] = info;
                }

                AssetDatabase.CreateAsset(mat, "Assets" + matPathName);
                AssetDatabase.CreateAsset(m_myFont, "Assets" + fontPathName);
                m_myFont.characterInfo = characterInfos;
                EditorUtility.SetDirty(m_myFont);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();//刷新资源
                Debug.Log("创建字体成功");
            }
            else
            {
                Debug.LogError("图集错误");
            }
        }
        else
        {
            Debug.LogError("创建失败，所选图片不存在！");
        }
    }
}
