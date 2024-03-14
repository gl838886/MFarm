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

    private bool inputDisable; //�ڳ����л�ʱ����Ҳ��ܶ� ������ʵʱcan't input

    //���߶����ж�
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
        inputDisable = true; //�ȵ����س�����ſ��Բ���
        transform.position = playerStartPosition;
    }

    private void OnEndCurrentGame()
    {
        inputDisable = false;
    }

    private void OnMouseClickEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        //ִ�ж���
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
            //������֮��ľ�ֱ��������
            EventHandler.CallExecuteActionAftreAnimation(mouseWorldPos, itemDetails);
        }
        
    }

    private IEnumerator UseToolRoutine(Vector3 mouseWorldPos, ItemDetails itemDetails)
    {
        useTool = true;
        inputDisable = true; //��������
        yield return null; //ȷ������ִ�������������
        foreach(var anim in animators)
        {
            anim.SetTrigger("useTool");
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        yield return new WaitForSeconds(0.45f); //�ڶ�����������ʱ��ִ�г���Ч��

        EventHandler.CallExecuteActionAftreAnimation(mouseWorldPos, itemDetails);
        yield return new WaitForSeconds(0.25f);
        useTool = false;
        inputDisable = false; //��������
    }

  

    private void Awake()
    {
        rb= GetComponent<Rigidbody2D>();
        animators = GetComponentsInChildren<Animator>();    
    }

    /**********************��������**********************/
    /**********************���ں���**********************/
    /**********************��������**********************/
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

        if (Input.GetKey(KeyCode.RightShift)) //������
        {
            inputX *= 0.5f;
            inputY *= 0.5f;
        }

        //б�������2=1.4>1���ᵼ��б�����ٶȽϿ�
        //��������жϺ��޸�
        if (inputX != 0 && inputY != 0)
        {
            inputX *= 0.6f;
            inputY *= 0.6f;
        }
        
        //���кϲ�
        movementInput =new Vector2(inputX, inputY);
        //�ж��Ƿ��ƶ�
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
