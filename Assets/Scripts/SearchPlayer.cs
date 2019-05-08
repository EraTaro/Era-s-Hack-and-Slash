using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //　敵キャラクターの状態を取得
            EnemyController.EnemyState state = GetComponentInParent<EnemyController>().GetState();
            //　敵キャラクターが追いかける状態でなければ追いかける設定に変更
            if (state != EnemyController.EnemyState.Run)
            {
                Debug.Log("プレイヤー発見");
                GetComponentInParent<EnemyController>().SetState("Run", other.transform);
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player")
        {
            Debug.Log("見失う");
            GetComponentInParent<EnemyController>().SetState("Idle");
        }
    }

}
