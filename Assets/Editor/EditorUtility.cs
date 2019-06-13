using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class EditorUtility
{
    /// <summary>
    /// 以IO方式进行图片加载
    /// </summary>
    public static Texture2D LoadTextureByIO(string path, int width, int height)
    {
        string filePath = Application.dataPath + "/" + path;    //拼图片路径
        double startTime = (double)Time.time;
        //创建文件读取流
        FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        //创建文件长度缓冲区
        byte[] bytes = new byte[fileStream.Length]; // 文件长度缓冲怎么设置为好？
        //读取文件
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        //释放文件读取流
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;

        //创建Texture —— TODO: Texture2D 怎样适配文件本身的像素大小？
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);

        startTime = (double)Time.time - startTime;
        Debug.Log("IO加载用时:" + startTime);

        return texture;

    }

}
