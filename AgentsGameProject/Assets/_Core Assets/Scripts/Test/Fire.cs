using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    float _elapsed;
    float _timer = 0.25f;

    float _fireRadius = 2f;

    public GameMaterial Owner;

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, _fireRadius);
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;

        if(_elapsed > _timer)
        {
            if (Owner.Amount > 0f)
            {
                RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, _fireRadius, Camera.main.transform.forward);
                foreach (RaycastHit2D hit in hits)
                {
                    if (hit.transform.GetComponent<Flammable>() != null && hit.transform.GetComponent<Flammable>()._burning == false)
                        hit.transform.GetComponent<Flammable>().Burn();
                }
            }

            else if (Owner.Amount <= 0f)
            {
                Destroy(Owner.gameObject);

                Destroy(gameObject);
            }

            _elapsed = 0f;
        }
        
    }
}
