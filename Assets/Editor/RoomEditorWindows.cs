using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomEditorWindows : EditorWindow
{
    [MenuItem("Window/Custom Editor Window")]
    public static void ShowWindow()
    {
        RoomEditorWindows window = EditorWindow.GetWindow<RoomEditorWindows>("Custom Window");
        window.Show();
    }


    private void OnGUI()
    {
        if (GUILayout.Button("Move OBJ"))
        {
            MoveOBJToGridCenter();
        }

    }

    private void MoveOBJToGridCenter()
    {
        // 获取场景中所有障碍物对象
        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("OBJ");

        foreach (GameObject obj in allObstacles)
        {
            // 执行障碍物的移动函数
            obj.GetComponent<ObjectSet>().SetPos();
        }
    }
}
