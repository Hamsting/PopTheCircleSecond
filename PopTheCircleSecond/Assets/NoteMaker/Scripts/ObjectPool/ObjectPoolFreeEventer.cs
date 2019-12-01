using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    /// <summary>
    /// 오브젝트 풀링을 돕는 이벤트 컴포넌트
    /// </summary>
    public class ObjectPoolFreeEventer : MonoBehaviour
    {
        /// <summary>
        /// 오브젝트 풀링에 등록한다.
        /// </summary>
        public void Free()
        {
            ObjectPoolManager.Instance.Free(this.gameObject);
        }

    }
}