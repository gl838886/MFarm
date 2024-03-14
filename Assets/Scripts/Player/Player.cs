using DG.Tweening;
using Mfarm.Save;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Player : MonoBehaviour, ISaveable
{
    private Rigidbody2D rb;

    public float speed;

    private float inputX;
    private float inputY;

    private bool isMoving;

    private Vector2 movementInput;

    private bool inputDisable; //在场景切换时，玩家不能动 这里其实时can't input

    //工具动画判断
    private float mouseX;
    private float mouseY;
    private bool useTool;

    private Animator[] animators;

    public Vector3 playerStartPosition;

    public string GUID => GetComponent<DataGUID>().GUID;

    private void OnEnable()
    {
        EventHandler.MoveToPositionEvent += OnMoveToPositionEvent;
        EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.MouseClickEvent += OnMouseClickEvent;
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        EventHandler.StartNewGame += OnStartNewGame;
        EventHandler.EndCurrentGame += OnEndCurrentGame;
    }

    private void OnDisable()
    {
        EventHandler.MoveToPositionEvent -= OnMoveToPositionEvent;
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.MouseClickEvent -= OnMouseClickEvent;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGame -= OnStartNewGame;
        EventHandler.EndCurrentGame -= OnEndCurrentGame;
    }


    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch(gameState)
        {
            case GameState.GamePause:
                inputDisable = true;
                break;
            case GameState.GamePlay:
                inputDisable = false; 
                break;
        }
    }

    private void OnAfterLoadSceneEvent()
    {
        inputDisable = false;
    }

    private void OnBeforeUnLoadSceneEvent()
    {
        inputDisable = true;
    }

    
    private void OnMoveToPositionEvent(Vector3 position)
    {
        rb.transform.position = position;
    }


    private void OnStartNewGame(int index)
    {
        inputDisable = true; //等到加载场景后才可以播放
        transform.position = playerStartPosition;
    }

    private void OnEndCurrentGame()
    {
        inputDisable = false;
    }

    private void OnMouseClickEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //执行动画
        if (itemDetails.itemType != ItemType.Seed && itemDetails.itemType != ItemType.Furniture && itemDetails.itemType != ItemType.Commodity)
        {
            mouseX = mouseWorldPos.x - transform.position.x;
            mouseY = mouseWorldPos.y - transform.position.y - 0.85f;
            if (Mathf.Abs(mouseX) - Mathf.Abs(mouseY) > 0)
            {
                mouseY = 0;
            }
            else
            {
                mouseX = 0;
            }
            
            StartCoroutine(UseToolRoutine(mouseWorldPos, itemDetails));
            
        }
        else
        {
            //扔种子之类的就直接走下面
            EventHandler.CallExecuteActionAftreAnimation(mouseWorldPos, itemDetails);
        }
        
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true; //锁定键盘
        yield return null; //确保上面执行完毕再往下走
        foreach(var anim in animators)
        {
            anim.SetTrigger("useTool");
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f); //在动画播到锄地时，执行锄地效果

        EventHandler.CallExecuteActionAftreAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);
        useTool = false;
        inputDisable = false; //锁定键盘
    }

  

    private void Awake()
    {
        rb= GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();    
    }

    /**********************————**********************/
    /**********************周期函数**********************/
    /**********************————**********************/
    void Update()
    {
        if(!inputDisable)
            playerInput();
        else 
            isMoving= false;
        SwitchAnimation();
        rb.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void FixedUpdate()
    {
        if (!inputDisable)
            playerMovement();     
    }
    private void playerInput()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(KeyCode.RightShift)) //加速跑
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }

        //斜方向根号2=1.4>1，会导致斜方向速度较快
        //这里进行判断后修改
        if (inputX != 0 && inputY != 0)
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }
        
        //进行合并
        movementInput =new Vector2(inputX, inputY);
        //判断是否移动
        isMoving = movementInput != Vector2.zero;
    }

    private void playerMovement()
    {
        rb.MovePosition(rb.position + movementInput * speed * Time.deltaTime);
    }

    private void  SwitchAnimation()
    {
        foreach(var anim in animators)
        {
            anim.SetBool("IsMoving", isMoving);
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving)
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));
        return saveData;
    }

    public void RestoreGameData(GameSaveData data)
    {
        var targetPosition = data.characterPosDict[this.name].ToVector3();
        transform.position= targetPosition;
    }
}
