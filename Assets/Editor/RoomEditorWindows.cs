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
        // ��ȡ�����������ϰ������
        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("OBJ");

        foreach (GameObject obj in allObstacles)
        {
            // ִ���ϰ�����ƶ�����
            obj.GetComponent<ObjectSet>().SetPos();
        }
    }
}
