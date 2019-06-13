using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Entry : MonoBehaviour
{
    public Canvas template_canvas;
    public Camera template_camera;

    // Start is called before the first frame update
    void Start()
    {
        CreateCanvasAndCamera();
        //TestNewGameObjectOperation();
    }


    /**
     * 2019/5/30：
     * 该函数充分证明了 —— 刚创建出来的canvas大小是与屏幕分辨率一致的，且 pixel上为 1：1的关系
     * 即便改动了canvasScaler也不能幸免
     */ 
    void CreateCanvasAndCamera() {

        //Copy Canvas//
        Canvas _canvas;

        _canvas = new GameObject("test_canvas").AddComponent<Canvas>();
        _canvas.transform.SetParent(this.transform);    //加到 Entry 脚本所挂 gameObject 底下

        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.pixelPerfect = template_canvas.pixelPerfect;
        _canvas.worldCamera = template_canvas.worldCamera;
        _canvas.enabled = false;

        Debug.Log("刚copy参数的canvas 的 Rect 为：" + _canvas.transform.GetComponent<RectTransform>().rect.width + "x" + _canvas.transform.GetComponent<RectTransform>().rect.height);

        //生成相机//
        Camera _camera = new GameObject("test_camera").AddComponent<Camera>();
        _camera.enabled = false;
        _camera.transform.SetParent(_canvas.transform.parent, false);
#if UNITY_EDITOR
        _camera.transform.SetSiblingIndex(_canvas.transform.GetSiblingIndex() + 1);
#endif
        _camera.transform.position = Vector3.down * 100f;
        _camera.clearFlags = CameraClearFlags.Depth;
        _camera.cullingMask = 32;
        _camera.farClipPlane = 200;
        _camera.orthographic = template_camera.orthographic;
        _camera.orthographicSize = template_camera.orthographicSize;
        _camera.useOcclusionCulling = false;
        _camera.allowHDR = false;
        _camera.allowMSAA = false;
        _camera.allowDynamicResolution = false;
        _canvas.worldCamera = _camera;  //绑定渲染相机

        //copy scale//
        CanvasScaler _canvasScale = _canvas.gameObject.GetComponent<CanvasScaler>();
        if (_canvasScale == null)
            _canvasScale = _canvas.gameObject.AddComponent<CanvasScaler>();

        CanvasScaler template_canvasScaler = template_canvas.GetComponent<CanvasScaler>();

        _canvasScale.uiScaleMode = template_canvasScaler.uiScaleMode;
        _canvasScale.referenceResolution = template_canvasScaler.referenceResolution;
        _canvasScale.matchWidthOrHeight = template_canvasScaler.matchWidthOrHeight;
        _canvasScale.referencePixelsPerUnit = template_canvasScaler.referencePixelsPerUnit;

        Debug.Log("copy了scaler参数的canvas 的 Rect 为：" + _canvas.transform.GetComponent<RectTransform>().rect.width + "x" + _canvas.transform.GetComponent<RectTransform>().rect.height);

        //GraphicRaycaster//
        GraphicRaycaster _graphicRaycaster = _canvas.gameObject.GetComponent<GraphicRaycaster>();
        if (_graphicRaycaster == null)
            _graphicRaycaster = _canvas.gameObject.AddComponent<GraphicRaycaster>();
        _graphicRaycaster.enabled = false;
    }

    void TestNewGameObjectOperation()
    {
        Canvas _go = new GameObject("test_go").AddComponent<Canvas>();  // new GameObject() 单独测试 —— 默认是从场景顶层建立新节点        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
