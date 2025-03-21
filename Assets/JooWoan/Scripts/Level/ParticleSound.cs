using UnityEngine;

namespace EverScord
{
    public class ParticleSound : MonoBehaviour
    {
        [SerializeField] private AudioSource emitSound;
        [SerializeField] private ParticleSystem effect;

        private int numberOfParticles;

        void Start()
        {
            numberOfParticles = 0;
        }

        void Update()
        {
            int count = effect.particleCount;

            // Particle has died
            // if (count < numberOfParticles)
            //     emitSound.Play();
            
            // Particle has been born
            if (count > numberOfParticles)
                emitSound.Play();
            
            numberOfParticles = count; 
        }
    }
}
