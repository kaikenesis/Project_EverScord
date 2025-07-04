﻿/*
work by adamman
*/
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace AdammanWorkSpace
{
    //CircleInfo Outline by adamman
    [AddComponentMenu("UI/Adamman/CircleInfo Outline"), RequireComponent(typeof(Graphic))]
    public class CircleInfoOutline : ModifiedShadow
    {
        [SerializeField]
        int m_circleCount = 2; // circle count for outline
        [SerializeField]
        int m_firstSample = 4; // first sample count for outline
        [SerializeField]
        int m_sampleIncrement = 2; // sample increment for outline

#if UNITY_EDITOR
        //only for editor mode to refresh
        protected override void OnValidate()
        {
            base.OnValidate();
            circleCount = m_circleCount;
            firstSample = m_firstSample;
            sampleIncrement = m_sampleIncrement;
        }
#endif
        // circle count for outline
        public int circleCount
        {
            get
            {
                return m_circleCount;
            }

            set
            {
                m_circleCount = Mathf.Max(value, 1);
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // first sample count for outline
        public int firstSample
        {
            get
            {
                return m_firstSample;
            }

            set
            {
                m_firstSample = Mathf.Max(value, 2);
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // sample increment for outline
        public int sampleIncrement
        {
            get
            {
                return m_sampleIncrement;
            }

            set
            {
                m_sampleIncrement = Mathf.Max(value, 1);
                if (graphic != null)
                    graphic.SetVerticesDirty();
            }
        }

        // Modify the vertices.
        public override void ModifyVertices(List<UIVertex> verts)
        {
            if (!IsActive())
                return;
            if (m_circleCount > 5)
            {
                m_circleCount = 5;
            }

            if (m_firstSample > 5)
            {
                m_firstSample = 5;
            }

            if (m_sampleIncrement > 5)
            {
                m_sampleIncrement = 5;
            }

            var total = (m_firstSample * 2 + m_sampleIncrement * (m_circleCount - 1)) * m_circleCount / 2;
            var neededCapacity = verts.Count * (total + 1);
            if (verts.Capacity < neededCapacity)
                verts.Capacity = neededCapacity;
            var original = verts.Count;
            var count = 0;
            var sampleCount = m_firstSample;
            var dx = effectDistance.x / circleCount;
            var dy = effectDistance.y / circleCount;
            for (int i = 1; i <= m_circleCount; i++)
            {
                var rx = dx * i;
                var ry = dy * i;
                var radStep = 2 * Mathf.PI / sampleCount;
                var rad = (i % 2) * radStep * 0.5f;
                for (int j = 0; j < sampleCount; j++)
                {
                    var next = count + original;
                    ApplyShadow(verts, effectColor, count, next, rx * Mathf.Cos(rad), ry * Mathf.Sin(rad));
                    count = next;
                    rad += radStep;
                }
                sampleCount += m_sampleIncrement;
            }
        }
    }

}