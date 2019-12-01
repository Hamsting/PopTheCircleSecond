using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.Game
{
    /// <summary>
    /// 충돌체를 이용한 오브젝트 풀링 이벤트 컴포넌트
    /// </summary>
    public class ObjectPoolWithCollider : ObjectPoolFreeEventer
    {
        #region 2DCollider
        private void OnCollisionEnter2D(Collision2D collision)
        {
            Free();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Free();
        }
        #endregion

    }
}