using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Effects
{
    public static class EffectControl
    {
        public static ParticleSystem[] SetEffectParticles(GameObject effect, bool shouldPlay)
        {
            // Effect will be destroyed due to particle stop action mode
            
            if (effect == null)
                return null;

            ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();
            SetEffectParticles(particles, shouldPlay);

            return particles;
        }

        public static void SetEffectParticles(ParticleSystem particle, bool shouldPlay)
        {
            SetEffectParticles(new[] { particle }, shouldPlay);
        }

        public static void SetEffectParticles(ParticleSystem[] particles, bool shouldPlay)
        {
            if (particles == null)
                return;
            
            for (int i = 0; i < particles.Length; i++)
            {
                if (!shouldPlay)
                    particles[i].Stop();

                else
                {
                    particles[i].Clear();
                    particles[i].Play();
                }
            }
        }
    }

}
