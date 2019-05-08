using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        RUN,
        ATTACK
    }

    //先ほど作成したJoystick
    [SerializeField]
    private Joystick _joystick = null;

    //private Animator animator;
    private STATE _state = STATE.IDLE;

    private float _moveSpeed = 5.0f;
    private Animator _animator;

    private void Awake()
    {
        TouchEventHandler.Instance.onTap += OnTap;
    }

    void Instance_OnTap()
    {
    }


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {

     

        Move();
    }

    // Update is called once per frame
    void Update()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            _state = STATE.RUN;
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            _state = STATE.IDLE;
        }
        else if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            _state = STATE.ATTACK;
        }
    }

    void Move()
    {
        //カメラの方向から、x-z平面の単位のベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        //入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * _joystick.Position.y + Camera.main.transform.right * _joystick.Position.x;

        //移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        GetComponent<Rigidbody>().velocity = moveForward * _moveSpeed + new Vector3(0, GetComponent<Rigidbody>().velocity.y, 0);

        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveForward);
        }
          
        _animator.SetBool("isRun", !(_joystick.Position.x == 0f && _joystick.Position.y == 0f));
    }

    void OnTap()
    {
        _animator.SetTrigger("pullAttack");
    }

    public void Hit()
    {
    }
}
