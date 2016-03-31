using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityHelpers;
using UnityHelpers.GUI;

[CustomEditor(typeof(MusicManager))]
public class MusicManagerInspector : Editor
{
    private MusicManager Manager { get { return target as MusicManager; } }

    private GUIStyle _listStyle;
    private GUIStyle _dropBoxStyle;
    private float _lineHeight;

    private Vector2 _listScroll;

    private float ScrollViewHeight
    {
        get { return (Manager.TracksCount > 10 ? 10 : Manager.TracksCount)*_lineHeight; }
    }

    private void OnEnable()
    {
        _lineHeight = EditorGUIUtility.singleLineHeight;
        _listStyle = ColorHelpers.MakeBackgroudnStyle(new Color(0.1f, 0.1f, 0.1f, 0.5f));

        _dropBoxStyle = new GUIStyle
        {
            normal = {background = ColorHelpers.MakeTex(new Color(0.5f, 0.5f, 0.5f, 0.5f))},
            fontSize = 14,
            alignment = TextAnchor.MiddleCenter,
            margin = new RectOffset(5, 5, 5, 5)
        };
    }

    public override void OnInspectorGUI()
    {
        Undo.RecordObject(Manager, "Music manager");
        serializedObject.Update();

        PlayControls();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Shuffle"));

        EditorGUIExtentions.DropZone<AudioClip>(PrefabType.None, list => Manager.Tracks.AddRange(list.Except(Manager.Tracks)), _dropBoxStyle);
        TracksList();

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(Manager);
    }

    private void TracksList()
    {
        using (new ScrollviewBlock(ref _listScroll, GUILayout.MaxHeight(ScrollViewHeight), GUILayout.ExpandHeight(false)))
        {
            using (new VerticalBlock(_listStyle, GUILayout.ExpandHeight(false)))
            {
                for (var i = 0; i < Manager.TracksCount; i++)
                {
                    using (new HorizontalBlock())
                    {
                        GUILayout.Label(Manager.Tracks[i].name);
                        GUILayout.FlexibleSpace();

                        if(GUILayout.Button("Select", EditorStyles.miniButtonLeft, GUILayout.ExpandWidth(false)))
                            Selection.activeObject = Manager.Tracks[i];

                        if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.ExpandWidth(false)))
                            Manager.Tracks.RemoveAt(i);
                    }
                }
            }
        }
    }

    private void PlayControls()
    {
        if(!Application.isPlaying)
            return;
        
    }
}
