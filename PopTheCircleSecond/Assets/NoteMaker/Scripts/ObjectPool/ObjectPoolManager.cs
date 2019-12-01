using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PopTheCircle.NoteEditor
{
    /// <summary>
    /// 게임 내 오브젝트를 풀링하는 매니저
    /// </summary>
    public class ObjectPoolManager : Singleton<ObjectPoolManager>
    {
        /// <summary>
        /// 기본 생성 수
        /// </summary>
        [SerializeField]
        private int defaultAmount = 10;
        /// <summary>
        /// 오브젝트 풀링 리스트
        /// </summary>
        [SerializeField]
        private GameObject[] poolList;
        /// <summary>
        /// 풀링 생성 수 리스트
        /// </summary>
        [SerializeField]
        private int[] poolAmount;

        private Dictionary<string, ObjectPool> objectPoolList = new Dictionary<string, ObjectPool>();

        protected override void Awake()
        {
            base.Awake();
            InitObjectPool();
        }

        /// <summary>
        /// 초기 오브젝트 풀링 생성
        /// </summary>
        void InitObjectPool()
        {
            for (int i = 0; i < poolList.Length; ++i)
            {
                ObjectPool objectPool = new ObjectPool();
                objectPool.source = poolList[i];
                objectPoolList[poolList[i].name] = objectPool;

                GameObject folder = new GameObject();
                folder.name = poolList[i].name;
                folder.transform.parent = this.transform;
                objectPool.folder = folder;

                int amount = defaultAmount;

                if (poolAmount.Length > i && poolAmount[i] != 0)
                {
                    amount = poolAmount[i];
                }

                for (int j = 0; j < amount; ++j)
                {
                    GameObject inst = Instantiate(objectPool.source);
                    inst.SetActive(false);
                    inst.transform.parent = folder.transform;
                    objectPool.unusedList.Add(inst);

                    //yield return new WaitForEndOfFrame();
                }
                objectPool.maxAmount = amount;
            }
        }

        /// <summary>
        /// 비활성화된 오브젝트를 가져온다.
        /// </summary>
        public GameObject Get(string name, bool isAutoActive = false)
        {
            if (!objectPoolList.ContainsKey(name))
            {
                Debug.LogError("[ObjectPoolManager] Can't Find ObjectPool : " + name);
                return null;
            }

            ObjectPool pool = objectPoolList[name];
            if (pool.unusedList.Count > 0)
            {
                GameObject obj = pool.unusedList[0];
                pool.unusedList.RemoveAt(0);
                if (isAutoActive)
                    obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(pool.source);

                ++pool.maxAmount;
                // print(name + " / Pool Size" + pool.maxAmount);
                return obj;
            }
        }

        /// <summary>
        /// 비활성화된 오브젝트를 가져온다.
        /// </summary>
        public GameObject Get(string name, Vector3 position, Quaternion rotation, bool isAutoActive = false)
        {
            if (!objectPoolList.ContainsKey(name))
            {
                Debug.LogError("[ObjectPoolManager] Can't Find ObjectPool : " + name);
                return null;
            }

            ObjectPool pool = objectPoolList[name];
            if (pool.unusedList.Count > 0)
            {
                GameObject obj = pool.unusedList[0];
                pool.unusedList.RemoveAt(0);

                obj.transform.position = position;
                obj.transform.rotation = rotation;
                if (isAutoActive)
                    obj.SetActive(true);
                return obj;
            }
            else
            {
                GameObject obj = Instantiate(pool.source);
                obj.transform.position = position;
                obj.transform.rotation = rotation;

                ++pool.maxAmount;
                Debug.LogWarning("[ObjectPoolManager]  " + name + " / Pool Size " + pool.maxAmount);

                return obj;
            }
        }

        /// <summary>
        /// 풀링 리스트에 등록한다.
        /// </summary>
        public void Free(GameObject obj)
        {
            string keyName = obj.name.Replace("(Clone)", "");
            if (!objectPoolList.ContainsKey(keyName))
            {
                Debug.LogError("[ObjectPoolManager] Can't Find ObjectPool : " + keyName);
                return;
            }
            ObjectPool pool = objectPoolList[keyName];
            obj.transform.parent = pool.folder.transform;
            obj.transform.position = new Vector3(-999f, -999f, 0f);
            pool.unusedList.Add(obj);
            obj.SetActive(false);
        }

    }
}

