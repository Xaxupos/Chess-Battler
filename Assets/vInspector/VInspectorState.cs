#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Type = System.Type;
using static VInspector.Libs.VUtils;
using static VInspector.Libs.VGUI;



namespace VInspector
{
    [FilePath("Library/vInspector State.asset", FilePathAttribute.Location.ProjectFolder)]
    public class VInspectorState : ScriptableSingleton<VInspectorState>
    {

        public SerializableDictionary<string, AttributesStateContainer> attributesStateContainers_bySceneGuid = new();

        public SerializableDictionary<GlobalID, AttributesState> attributesStates_byAssetGlobalId = new();


        [System.Serializable]
        public class AttributesStateContainer
        {
            public SerializableDictionary<int, AttributesState> unsavedAttributesStates_byInstanceId = new();
            public SerializableDictionary<int, AttributesState> savedAttributesStates_byInstanceId = new();

            public SerializableDictionary<GlobalID, AttributesState> savedAttributesStates_byGlobalId = new();

        }

        [System.Serializable]
        public class AttributesState
        {
            public SerializableDictionary<string, int> selectedSubtabIndexes_byTabPath = new();
            public SerializableDictionary<string, bool> isExpandeds_byFoldoutPath = new();
            public SerializableDictionary<string, bool> isExpandeds_byButtonPath = new();

        }






        public SerializableDictionary<int, ItemState> itemStates_byItemId = new();

        [System.Serializable]
        public class ItemState
        {
            public string _name;
            public string sceneGameObjectIconName;
        }






        public static void Clear()
        {
            instance.attributesStates_byAssetGlobalId.Clear();
            instance.attributesStateContainers_bySceneGuid.Clear();
            instance.attributesStates_byAssetGlobalId.Clear();
            instance.itemStates_byItemId.Clear();

        }

        public static void Save() => instance.Save(true);

    }
}
#endif