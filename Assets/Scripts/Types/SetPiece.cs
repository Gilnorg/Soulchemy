using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPiece : MonoBehaviour {

	public virtual void Func(Entity entity)
    {
        print(entity.name);
    }

}
