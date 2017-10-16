﻿using System;
using UnityEditor;

namespace UnityEngine.Graphing
{
    public class SerializableGraphObject : ScriptableObject, IGraphObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        SerializationHelper.JSONSerializedElement m_SerializedGraph;

        IGraph m_Graph;
        IGraph m_DeserializedGraph;

        public IGraph graph
        {
            get { return m_Graph; }
            set
            {
                if (m_Graph != null)
                    m_Graph.owner = null;
                m_Graph = value;
                if (m_Graph != null)
                    m_Graph.owner = this;
            }
        }

        public void RegisterCompleteObjectUndo(string name)
        {
            Undo.RegisterCompleteObjectUndo(this, name);
        }

        public void OnBeforeSerialize()
        {
            m_SerializedGraph = SerializationHelper.Serialize(graph);
        }

        public void OnAfterDeserialize()
        {
            var deserializedGraph = SerializationHelper.Deserialize<IGraph>(m_SerializedGraph, null);
            if (graph == null)
                graph = m_DeserializedGraph;
            else
                m_DeserializedGraph = deserializedGraph; // graph.ReplaceWith(m_DeserializedGraph);
        }

        void OnEnable()
        {
            Undo.undoRedoPerformed += UndoRedoPerformed;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= UndoRedoPerformed;
        }

        void UndoRedoPerformed()
        {
            if (m_DeserializedGraph != null)
            {
                graph.ReplaceWith(m_DeserializedGraph);
                m_DeserializedGraph = null;
            }
        }
    }
}