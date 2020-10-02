using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YYMEditor
{
    public class SceneTools : EditorWindow
    {
        const string WindowName = "场景工具";
        [MenuItem(EditorConst.MenuRoot + WindowName)]
        public static void Open()
        {
            GetWindow<SceneTools>(WindowName);
        }

         public string namePerfix = "";
         public int startIdx = 0;

        private void OnGUI()
        {
            namePerfix = EditorGUILayout.TextField("前缀", namePerfix);
            startIdx = EditorGUILayout.IntField("起始编号", startIdx);
            if (GUILayout.Button("重命名"))
            {
                var gos = Selection.gameObjects.OrderBy((_go)=>_go.transform.GetSiblingIndex());
                Undo.RecordObjects(gos.ToArray(), "rename_yym");
                int i = startIdx;
                foreach (var go in gos)
                {
                    go.name = namePerfix + i.ToString();
                    i = i + 1;
                }
            }

            if (GUILayout.Button("按名排序"))
            {
                var gos = Selection.gameObjects.OrderBy((_go)=>_go.name);
                var i = 0;
                foreach (var go in gos)
                {
                    go.transform.SetSiblingIndex(i);
                    i = i + 1;
                }
            }
        }
    }
}
