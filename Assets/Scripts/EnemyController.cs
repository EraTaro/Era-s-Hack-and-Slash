using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public enum EnemyState
    {
        Walk,
        Run,
        Idle,
        Attack,
        Freeze
    };

    EnemyState _enemyState;
    Transform _playerTransform;

    //　敵キャラクターの状態変更メソッド
    public void SetState(string mode, Transform obj = null)
    {
        if (mode == "walk")
        {
            elapsedTime = 0f;
            _enemyState = EnemyState.Walk;
            CreateRandomPosition();
        }
        else if (mode == "Run")
        {
            _enemyState = EnemyState.Run;
            //　待機状態から追いかける場合もあるのでOff
            //　追いかける対象をセット
            _playerTransform = obj;
        }
        else if (mode == "Idle")
        {
            elapsedTime = 0f;
            _enemyState = EnemyState.Idle;
            velocity = Vector3.zero;
            _animator.SetFloat("Speed", 0f);
        }
    }

    public EnemyState GetState()
    {
        return _enemyState;
    }

    private Animator _animator;

    //初期位置
    private Vector3 startPosition;

    private CharacterController enemyController;

    //　目的地
    private Vector3 targetPosition;
    //　歩くスピード
    [SerializeField]
    private float walkSpeed = 1.0f;
    //　速度
    private Vector3 velocity;
    //　移動方向
    private Vector3 direction;
    //　到着フラグ

    //　待ち時間
    [SerializeField]
    private float waitTime = 5f;
    //　経過時間
    private float elapsedTime;

    // Use this for initialization
    void Start()
    {
        //　初期位置を設定
        startPosition = transform.position;
        SetDestination(transform.position);

        enemyController = GetComponent<CharacterController>();
        CreateRandomPosition();
        velocity = Vector3.zero;
        elapsedTime = 0f;

        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //見回り処理
        if (_enemyState == EnemyState.Walk || _enemyState == EnemyState.Run) 
        {
            if (_enemyState == EnemyState.Run)
            {
                SetDestination(_playerTransform.position);
                _animator.SetFloat("Speed", 5.0f);
                walkSpeed = 3.0f;
            }
            else
            {
                _animator.SetFloat("Speed", 3.0f);
                walkSpeed = 1.0f;
            }

            if (enemyController.isGrounded)
            {
                velocity = Vector3.zero;
                direction = (targetPosition - transform.position).normalized;
                transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));
                velocity = direction * walkSpeed;
                Debug.Log(targetPosition);
            }
            velocity.y += Physics.gravity.y;
            enemyController.Move(velocity * Time.deltaTime);

            //　目的地に到着したかどうかの判定
            if (Vector3.Distance(transform.position, targetPosition) < 1f)
            {
                SetState("Idle");
                _animator.SetFloat("Speed", 0.0f);
            }
        }
        else
        {
            elapsedTime += Time.deltaTime;

            //　待ち時間を越えたら次の目的地を設定
            if (elapsedTime > waitTime)
            {
                CreateRandomPosition();
                targetPosition = GetDestination();
                elapsedTime = 0f;
            }
            Debug.Log(elapsedTime);
            velocity.y += Physics.gravity.y * Time.deltaTime;
            enemyController.Move(velocity * Time.deltaTime);
            _animator.SetFloat("Speed", 3.0f);
            walkSpeed = 1.0f;
        }
    }

    //　ランダムな位置の作成
    public void CreateRandomPosition()
    {
        //　ランダムなVector2の値を得る
        var randDestination = Random.insideUnitCircle * 8;
        //　現在地にランダムな位置を足して目的地とする
        SetDestination(startPosition + new Vector3(randDestination.x, transform.position.y, randDestination.y));
    }

    //　目的地を設定する
    public void SetDestination(Vector3 position)
    {
        targetPosition = position;
    }

    //　目的地を取得する
    public Vector3 GetDestination()
    {
        return targetPosition;
    }
}
