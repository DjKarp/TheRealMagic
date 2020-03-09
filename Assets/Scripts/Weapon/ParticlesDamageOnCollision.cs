using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesDamageOnCollision : MonoBehaviour
{

    public float damage = 4;
    
    private bool isDamage = false;

    private void Awake()
    {


    }

    private void OnParticleCollision(GameObject other)
    {

        if (other.CompareTag("Enemy") | other.CompareTag("Player"))
        {

            if (!isDamage)
            {

                isDamage = true;
                other.GetComponent<Pawn>().TakeDamage(damage);

            }

        }
        else if (other.CompareTag("DestructionObject"))
        {

            Destroy(other);

        }

    }

}
