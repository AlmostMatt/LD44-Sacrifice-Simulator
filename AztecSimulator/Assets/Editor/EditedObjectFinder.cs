using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditedObjectFinder {
    // Workaround taken from 
    // https://issuetracker.unity3d.com/issues/scene-is-instantly-marked-dirty-when-opened-if-any-object-is-affected-by-both-a-contentsizefitter-and-verticallayoutgroup

    static EditedObjectFinder()
    {
        Undo.postprocessModifications += OnPostProcessModifications;
    }

    private static UndoPropertyModification[] OnPostProcessModifications(UndoPropertyModification[] propertyModifications)
    {
        // I only care about the edits that happen when I stop playing.
        if (EditorApplication.isPlaying)
        {
            return propertyModifications;
        }
        Debug.LogWarning(string.Format("EditedObjectFinder:  {0} objects were edited automatically.", propertyModifications.Length));
        for (int i = 0; i < propertyModifications.Length; i++)
        {
            if (!(propertyModifications[i].currentValue.target is Transform))
            {
                continue;
            }
            Transform obj = (Transform)propertyModifications[i].currentValue.target;
            string propertyName = propertyModifications[i].currentValue.propertyPath;
            string oldValue = propertyModifications[i].previousValue.value;
            string newValue = propertyModifications[i].currentValue.value;
            string objectName = obj.name;
            while (obj.parent != null)
            {
                obj = obj.parent;
                objectName = obj.name + "/" + objectName;
            }
            Debug.LogWarning(string.Format("{0} : {1} = {2} -> {3}", objectName, propertyName, oldValue, newValue));
            // TODO: either undo the change, or disable/enable the relevant ScrollRect component
        }
        return propertyModifications;
    }
} 